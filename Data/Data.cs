using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Threading;
using WPFBookStore.Models;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;

namespace WPFBookStore.Data
{
    public class BookService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BookService> _logger;

        public BookService(ILogger<BookService> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _logger.LogInformation("BookService initialized");
        }

        // Получение жанров для страницы Каталог
        public async Task<List<ClassGenres>> GetGenresAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://185.9.72.1:7778/api/v1/book/genres");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ClassGenres>>(json);
            }
            catch
            {
                return new List<ClassGenres>();
            }
        }

        public async Task<List<Book>> GetListBooksAsync
            (
            string title = null,
            int? genre = null,
            int? year = null,
            string author = null,
            CancellationToken cancellationToken = default)
        {
            // Проверяем токен отмены перед началом операции
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Starting GetBooksAsync with parameters: Title={Title}, Genre={Genre}, Year={Year}, Author={Author}",
                title, genre, year, author);

            try
            {
                // Формируем базовый URL
                string baseUrl = "http://185.9.72.1:7778/api/v1/book/list";

                // Формируем параметры запроса
                var parameters = new List<string>();
                if (!string.IsNullOrEmpty(title))
                    parameters.Add($"query={Uri.EscapeDataString(title)}");
                if (genre.HasValue)
                    parameters.Add($"genre={genre.Value}");
                if (year.HasValue)
                    parameters.Add($"year={year.Value}");
                if (!string.IsNullOrEmpty(author))
                    parameters.Add($"author={Uri.EscapeDataString(author)}");

                // Соединяем параметры в строку запроса
                string queryString = string.Join("&", parameters);
                string requestUrl = string.IsNullOrEmpty(queryString) ? baseUrl : $"{baseUrl}?{queryString}";

                _logger.LogDebug("Request URL: {RequestUrl}", requestUrl);

                // Отправляем запрос и получаем ответ
                _logger.LogInformation("Sending HTTP GET request to {RequestUrl}", requestUrl);
                var response = await _httpClient.GetAsync(requestUrl, cancellationToken);

                _logger.LogDebug("Received response with status code {StatusCode}", response.StatusCode);
                response.EnsureSuccessStatusCode();

                // Читаем содержимое ответа с учетом токена отмены
                var jsonResponse = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Received JSON response: {JsonResponse}", jsonResponse);

                // Десериализуем JSON в список книг
                var books = JsonConvert.DeserializeObject<List<Book>>(jsonResponse);
                _logger.LogInformation("Successfully deserialized {BookCount} books", books?.Count ?? 0);

                return books;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while getting books");
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization failed while processing books response");
                throw;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Book search operation was canceled by user request");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred in GetBooksAsync");
                throw;
            }

        }

        public async Task<Book> GetBookAsync(int idBook)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://185.9.72.1:7778/api/v1/book/{idBook}/");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(json);
                return JsonConvert.DeserializeObject<Book>(json);
            }
            catch 
            { 
                return new Book();
            }
        }

        //Кусок моего канала:

        public void Dispose()
        {
            _httpClient?.Dispose();
            _logger.LogInformation("BookService disposed");
        }
        public BookService() : this(NullLogger<BookService>.Instance)
        {
        }
    }
}