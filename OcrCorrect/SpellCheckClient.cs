using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OcrCorrect.Models;

namespace OcrCorrect
{
    public class SpellCheckClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _key;

        public SpellCheckClient(string key)
        {
            _key = key;
            _httpClient = new HttpClient();
        }

        public async Task<string> CorrectAsync(string line, string preContext = null, string postContext = null)
        {
            using (var message = new HttpRequestMessage(HttpMethod.Post, "https://api.cognitive.microsoft.com/bing/v5.0/SpellCheck"))
            {
                message.Headers.Add("Ocp-Apim-Subscription-Key", _key);
                message.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["text"] = line,
                    ["preContextText"] = preContext ?? "",
                    ["postContextText"] = postContext ?? ""
                });

                var response = await _httpClient.SendAsync(message);

                var json = await response.Content.ReadAsStringAsync();
                var spellCheckResponse = JsonConvert.DeserializeObject<SpellCheckResponse>(json);

                var correctedWords = new List<string>();

                foreach(var flaggedtoken in spellCheckResponse.flaggedTokens?.Where(x => x?.suggestions?.Any() == true))
                {
                    line = line.Replace($" {flaggedtoken.token} " , $" {flaggedtoken.suggestions.First().suggestion} ");
                }

                return line;
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
