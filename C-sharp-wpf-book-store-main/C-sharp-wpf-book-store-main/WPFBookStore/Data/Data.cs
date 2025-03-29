using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace WPFBookStore.Data
{
    public class BookService
    {
        private readonly HttpClient _httpClient;

        public BookService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<Book>> GetBooksAsync(
            string title = null,
            int? genre = null,
            int? year = null,
            string author = null)
        {
            // Формируем базовый URL
            string baseUrl = "http://185.9.72.1:7778/api/v1/book/list";

            // Формируем параметры запроса
            var parameters = new List<string>();
            if (!string.IsNullOrEmpty(title))
                parameters.Add($"query={Uri.EscapeDataString(title)}");
            if (year.HasValue)
                parameters.Add($"genre={genre.Value}");
            if (year.HasValue)
                parameters.Add($"year={year.Value}");
            if (!string.IsNullOrEmpty(author))
                parameters.Add($"author={Uri.EscapeDataString(author)}");

            // Соединяем параметры в строку запроса
            string queryString = string.Join("&", parameters);
            string requestUrl = string.IsNullOrEmpty(queryString) ? baseUrl : $"{baseUrl}?{queryString}";

            Console.WriteLine(requestUrl);
            // Отправляем запрос и получаем ответ
            var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            // Читаем содержимое ответа
            var jsonResponse = await response.Content.ReadAsStringAsync();

            // Десериализуем JSON в список книг
            var books = JsonConvert.DeserializeObject<List<Book>>(jsonResponse);
            return books;
        }
    }
}