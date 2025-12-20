using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GymReservation.Models;
using Microsoft.Extensions.Configuration;

namespace GymReservation.Services
{
    public class GeminiFitnessService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        // Güncel model
        private const string Model = "gemini-2.5-flash";

        public GeminiFitnessService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"] ?? "";

            if (!string.IsNullOrWhiteSpace(_apiKey) &&
                !_httpClient.DefaultRequestHeaders.Contains("x-goog-api-key"))
            {
                _httpClient.DefaultRequestHeaders.Add("x-goog-api-key", _apiKey);
            }
        }

        public async Task<string> GetFitnessPlanAsync(FitnessAiRequestViewModel req)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                return "Gemini API anahtarı bulunamadı. appsettings.Development.json içine 'Gemini:ApiKey' ekleyin.";

            var prompt = $@"
Kullanıcının bilgilerine dayanarak fitness + beslenme programı öner.
TÜRKÇE yaz.
Cevabı anlaşılır, madde madde ve uygulanabilir şekilde ver.

Bilgiler:
- Cinsiyet: {req.Gender}
- Yaş: {req.Age}
- Boy: {req.HeightCm} cm
- Kilo: {req.WeightKg} kg
- Hedef: {req.Goal}
- Aktivite seviyesi: {req.ActivityLevel}
- Ek bilgi: {req.AdditionalInfo}

İçerik:
1) Haftalık antrenman planı (gün gün)
2) Beslenme önerileri (örnek öğünler)
3) Dikkat edilmesi gerekenler
4) Motivasyon / kısa tavsiyeler
5) 3 ay sonunda beklenen değişim (gerçekçi, abartmadan)

Çıktıyı HTML formatında ver (ör: <h4>, <ul>, <li> kullan).
";

            var body = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // API key querystring ile
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{Model}:generateContent?key={_apiKey}";

            var response = await _httpClient.PostAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return $"Gemini API hatası: {response.StatusCode}\n{result}";

            try
            {
                using var doc = JsonDocument.Parse(result);

                var text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return text ?? "Gemini yanıtı boş döndü.";
            }
            catch
            {
                return "Gemini yanıtı işlenemedi. (JSON parse hatası)";
            }
        }
    }
}
