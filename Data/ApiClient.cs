using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ApiClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://185.9.72.1:7778";
    private const string TokenFile = "auth_token.dat";
    private string _authToken;

    public ApiClient()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
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
            var request = new HttpRequestMessage(HttpMethod.Head, BaseUrl);
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

    public void Logout()
    {
        ClearToken();
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