using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OcrCorrect.Models;

namespace OcrCorrect
{
    public class OcrClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly HttpRequestMessage _message;

        public OcrClient(byte[] bytes, string key)
        {
            _httpClient = new HttpClient();

            _message = new HttpRequestMessage(HttpMethod.Post, "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr");
            _message.Headers.Add("Ocp-Apim-Subscription-Key", key);
            _message.Content = new ByteArrayContent(bytes);
            _message.Content.Headers.Add("Content-Type", "application/octet-stream");
        }

        public async Task<string[]> GetLinesAsync(IFormFile file)
        {
            var response = await _httpClient.SendAsync(_message);

            var json = await response.Content.ReadAsStringAsync();
            var ocrResponse = JsonConvert.DeserializeObject<OcrResponse>(json);

            var lines = new List<string>();
            foreach(var region in ocrResponse.regions)
            {
                foreach(var line in region.lines)
                {
                    lines.Add(string.Join(" ", line.words.Select(x => x.text).ToArray()));
                }
            }

            return lines.ToArray();
        }
        public void Dispose()
        {
            _message.Dispose();
            _httpClient.Dispose();
        }
    }
}
