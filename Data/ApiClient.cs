using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WPFBookStore.Data;
using WPFBookStore.Models;

public class ApiClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private const string TokenFile = "auth_token.dat";
    private string _authToken;

    public ApiClient()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(BaseApiURL.BaseUrl) };
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        LoadToken(); // Загружаем сохранённый токен при инициализации
    }

    #region Токен авторизации
    private void LoadToken()
    {
        try
        {
            if (File.Exists(TokenFile))
            {
                _authToken = File.ReadAllText(TokenFile);
                UpdateAuthorizationHeader();
            }
            else
            {
                File.Create(TokenFile);
                FileInfo fileInfo = new FileInfo(TokenFile);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token load error: {ex.Message}");
        }
    }

    private void SaveToken(string token)
    {
        try
        {
            File.WriteAllText(TokenFile, token);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token save error: {ex.Message}");
        }
    }

    private void ClearToken()
    {
        _authToken = null;
        try
        {
            File.WriteAllText(_authToken, null);
            _authToken = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token clear error: {ex.Message}");
        }
        UpdateAuthorizationHeader();
    }

    private void UpdateAuthorizationHeader()
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            !string.IsNullOrEmpty(_authToken)
                ? new AuthenticationHeaderValue("Token", _authToken)
                : null;
    }

    public string GetAuthToken() => _authToken;
    #endregion

    #region Сетевое взаимодействие
    private async Task<bool> CheckInternetConnection()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Head, BaseApiURL.BaseUrl);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async Task<HttpResponseMessage> SafeSendRequestAsync(Func<Task<HttpResponseMessage>> requestFunc)
    {
        if (!await CheckInternetConnection())
            throw new HttpRequestException("No internet connection");

        try
        {
            var response = await requestFunc();

            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized: // 401
                    ClearToken();
                    throw new UnauthorizedAccessException("Session expired");
                case HttpStatusCode.InternalServerError: // 500
                    throw new HttpRequestException("Server error");
            }

            return response;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Network error: {ex.Message}");
            throw;
        }
    }
    #endregion

    #region API методы
    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            var formData = new MultipartFormDataContent
            {
                { new StringContent(username), "username" },
                { new StringContent(password), "password" }
            };

            var response = await SafeSendRequestAsync(() =>
                _httpClient.PostAsync("/api/v1/auth/token/login/", formData));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _authToken = JsonConvert.DeserializeObject<TokenResponse>(content)?.AuthToken;

                if (!string.IsNullOrEmpty(_authToken))
                {
                    UpdateAuthorizationHeader();
                    SaveToken(_authToken);
                    return true;
                }
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> GetAccountDataAsync()
    {
        if (string.IsNullOrEmpty(_authToken))
            throw new UnauthorizedAccessException("Not authenticated");

        try
        {
            var response = await SafeSendRequestAsync(() =>
                _httpClient.GetAsync("/api/v1/account"));

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch
        {
            return null;
        }
    }

    public async Task<string> GetBooksAsync()
    {
        if (string.IsNullOrEmpty(_authToken))
            throw new UnauthorizedAccessException("Not authenticated");

        try
        {
            var response = await SafeSendRequestAsync(() =>
                _httpClient.GetAsync("/api/v1/book/list"));

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch
        {
            return null;
        }
    }

    public async void Logout()
    {
        try
        {
            // Отправляем запрос на сервер для выхода
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("/api/v1/auth/token/logout/");

                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);


                // Отправляем POST запрос на endpoint выхода
                var response = await client.PostAsync("/v1/auth/token/logout/", null);

                if (!response.IsSuccessStatusCode)
                    Console.WriteLine($"Ошибка при выходе: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Исключение при выходе: {ex.Message}");
        }
        finally
        {
            ClearToken();
            File.Delete(TokenFile);
        }
    }
    #endregion

    #region Book Operations
    public async Task<string> TakeBookAsync(int bookId)
    {
        if (string.IsNullOrEmpty(_authToken))
            throw new UnauthorizedAccessException("Not authenticated");

        try
        {
            var response = await SafeSendRequestAsync(() =>
                _httpClient.PostAsync($"/api/v1/book/take/{bookId}/", null));

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to take book: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error taking book: {ex.Message}");
            throw;
        }
    }

    public async Task<string> ReturnBookAsync(int issueId)
    {
        if (string.IsNullOrEmpty(_authToken))
            throw new UnauthorizedAccessException("Not authenticated");

        try
        {
            var response = await SafeSendRequestAsync(() =>
                _httpClient.PutAsync($"/v1/account/return/{issueId}/", null));

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to return book: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error returning book: {ex.Message}");
            throw;
        }
    }

    public async Task<List<MyBook>> GetMyBooksAsync()
    {
        if (string.IsNullOrEmpty(_authToken))
            throw new UnauthorizedAccessException("Not authenticated");

        try
        {
            var response = await SafeSendRequestAsync(() =>
                _httpClient.GetAsync("/api/v1/account/mybooks/"));

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var books = JsonConvert.DeserializeObject<List<MyBook>>(content);
            return books?
            .Where(book => book.ReturnDate == null)
            .ToList()
            ?? new List<MyBook>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting my books: {ex.Message}");
            throw;
        }
    }
    #endregion

    public void Dispose()
    {
        _httpClient?.Dispose();
    }

    private class TokenResponse
    {
        [JsonProperty("auth_token")]
        public string AuthToken { get; set; }
    }
}