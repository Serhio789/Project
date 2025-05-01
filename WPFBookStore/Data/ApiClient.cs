using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WPFBookStore.Data
{
    public class ApiClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://185.9.72.1:7778/api";
        public string _authToken;

        public ApiClient()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> RegisterAsync(string username, string password, string email)
        {
            var formData = new MultipartFormDataContent
        {
            { new StringContent(username), "username" },
            { new StringContent(password), "password" },
            { new StringContent(email), "email" }
        };

            try
            {
                var response = await _httpClient.PostAsync("/api/v1/auth/users/", formData);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Register error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var formData = new MultipartFormDataContent
            {
                { new StringContent(username), "username" },
                { new StringContent(password), "password" }
            };

                var response = await _httpClient.PostAsync("/api/v1/auth/token/login/", formData);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(content);
                    _authToken = tokenResponse?.AuthToken;

                    if (!string.IsNullOrEmpty(_authToken))
                    {
                        _httpClient.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Token", _authToken);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return false;
            }
        }

        public async Task<string> GetUserAccountAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/v1/account");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetUserAccount error: {ex.Message}");
                return null;
            }
        }

        public async Task<string> GetBooksAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/v1/book/list");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetBooks error: {ex.Message}");
                return null;
            }
        }

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
}
