using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace WPFBookStore.Data
{
    public class BookTextClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private const string AuthTokenFile = "auth_token.dat";

        public BookTextClient(string baseUrl)
        {
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            _httpClient = new HttpClient();
        }

        public async Task<string> GetBookTextAsync(int bookId)
        {
            try
            {
                string url = $"{_baseUrl}/v1/book/text/{bookId}/";

                // Получаем токен из файла
                string authToken = GetTokenFromFile();

                if (string.IsNullOrEmpty(authToken))
                {
                    throw new InvalidOperationException("Токен авторизации не найден или пуст");
                }

                // Добавляем заголовок авторизации
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", authToken);

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync();

                throw new HttpRequestException($"Ошибка при получении текста. Код: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                throw;
            }
            finally
            {
                // Очищаем заголовки после запроса
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
            }
        }

        private string GetTokenFromFile()
        {
            try
            {
                // Проверяем существование файла
                if (!File.Exists(AuthTokenFile))
                {
                    throw new FileNotFoundException($"Файл токена {AuthTokenFile} не найден");
                }

                // Читаем весь текст из файла
                string token = File.ReadAllText(AuthTokenFile);

                // Проверяем, что токен не пустой
                if (string.IsNullOrWhiteSpace(token))
                {
                    throw new InvalidDataException($"Файл токена {AuthTokenFile} пуст или содержит неверные данные");
                }

                return token.Trim(); // Удаляем возможные пробелы и переносы строк
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении токена: {ex.Message}");
                throw;
            }
        }

        public string[] SplitIntoPages(
            string fb2Text,
            FontFamily fontFamily,
            double fontSize,
            double containerWidth,
            double containerHeight,
            int maxCharsPerPage = 5000)
        {
            if (string.IsNullOrEmpty(fb2Text))
                return Array.Empty<string>();

            var pages = new List<string>();
            var currentPageText = new System.Text.StringBuilder();
            var typeface = new Typeface(
                fontFamily,
                FontStyles.Normal,
                FontWeights.Normal,
                FontStretches.Normal
            );

            int currentIndex = 0;
            while (currentIndex < fb2Text.Length)
            {
                int chunkSize = Math.Min(100, fb2Text.Length - currentIndex);
                string chunk = fb2Text.Substring(currentIndex, chunkSize);
                currentPageText.Append(chunk);

                var formattedText = new FormattedText(
                    currentPageText.ToString(),
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    fontSize,
                    Brushes.Black,
                    new NumberSubstitution(),
                    1
                );

                if (formattedText.Height > containerHeight || currentPageText.Length >= maxCharsPerPage)
                {
                    string currentText = currentPageText.ToString();
                    int lastSpace = currentText.LastIndexOf(' ');

                    if (lastSpace > 0)
                    {
                        pages.Add(currentText.Substring(0, lastSpace));
                        currentPageText.Clear();
                        currentPageText.Append(currentText.Substring(lastSpace + 1));
                        currentIndex -= (chunkSize - (lastSpace + 1));
                    }
                    else
                    {
                        pages.Add(currentText);
                        currentPageText.Clear();
                    }
                }

                currentIndex += chunkSize;
            }

            if (currentPageText.Length > 0)
                pages.Add(currentPageText.ToString());

            return pages.ToArray();
        }
        public string ConvertFb2ToPlainText(string fb2Content)
        {
            try
            {
                var result = new StringBuilder();
                using (var reader = new StringReader(fb2Content))
                {
                    bool inBody = false;
                    bool inParagraph = false;
                    bool inTitle = false;
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();

                        if (line.Contains("<body>"))
                        {
                            inBody = true;
                            continue;
                        }

                        if (line.Contains("</body>"))
                        {
                            inBody = false;
                            continue;
                        }

                        if (!inBody) continue;

                        if (line.Contains("<title>"))
                        {
                            inTitle = true;
                            result.AppendLine().AppendLine();
                            continue;
                        }

                        if (line.Contains("</title>"))
                        {
                            inTitle = false;
                            result.AppendLine();
                            continue;
                        }

                        if (line.Contains("<p>"))
                        {
                            inParagraph = true;
                            line = line.Replace("<p>", "").Replace("</p>", "");
                            if (!string.IsNullOrWhiteSpace(line))
                                result.AppendLine(line);
                            continue;
                        }

                        if (line.Contains("</p>"))
                        {
                            inParagraph = false;
                            line = line.Replace("<p>", "").Replace("</p>", "");
                            if (!string.IsNullOrWhiteSpace(line))
                                result.AppendLine(line);
                            continue;
                        }

                        if (line.Contains("<emphasis>"))
                        {
                            line = line.Replace("<emphasis>", "").Replace("</emphasis>", "");
                        }

                        if (line.Contains("</emphasis>"))
                        {
                            line = line.Replace("<emphasis>", "").Replace("</emphasis>", "");
                        }

                        if (line.Contains("<strong>"))
                        {
                            line = line.Replace("<strong>", "").Replace("</strong>", "");
                        }

                        if (line.Contains("</strong>"))
                        {
                            line = line.Replace("<strong>", "").Replace("</strong>", "");
                        }

                        if (inTitle || inParagraph)
                        {
                            result.AppendLine(line);
                        }
                    }
                }

                return System.Text.RegularExpressions.Regex.Replace(
                    result.ToString(),
                    @"^\s+$[\r\n]*",
                    "",
                    System.Text.RegularExpressions.RegexOptions.Multiline
                ).Trim();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка преобразования FB2: {ex.Message}");
                return fb2Content; 
            }
        }
    }
}
