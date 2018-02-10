using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TranslateClient2.Translator {
    public class YandexTranslator : ITranslator {
        private readonly string _dictionaryUri = "https://dictionary.yandex.net/api/v1/dicservice.json/lookup";

        private readonly string _dictionaryKey =
            "dict.1.1.20180128T003949Z.f58f40a5e2137f85.63c0c4ab86a4186add409a6d8928aa679fc90279";

        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<string> Translate(string input) {
            var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>() {
                {"key", _dictionaryKey},
                {"lang", "en-ru"},
                {"text", input}
            });

            string responseJson = await JsonFromPost(_dictionaryUri, requestContent);

            return ParsedTranslateResponse(responseJson);
        }

        private async Task<string> JsonFromPost(string uri, HttpContent requestContent) {
            var response = await _httpClient.PostAsync(_dictionaryUri, requestContent);

            string responseJson = await response.Content.ReadAsStringAsync();
            EnsureSuccessStatusCode(response, responseJson);

            return responseJson;
        }

        private string ParsedTranslateResponse(string responseJson) {
            dynamic tr = JObject.Parse(responseJson)["def"][0]["tr"];
            var result = new StringBuilder(tr[0]["text"].ToString());
            foreach (var syn in tr[0]["syn"]) {
                result.Append('\n' + syn["text"].ToString());
            }

            return result.ToString();
        }

        private void EnsureSuccessStatusCode(HttpResponseMessage response, string responseJson) {
            if (response.StatusCode == HttpStatusCode.Forbidden) ClarifyForbiddenCode(responseJson);
            response.EnsureSuccessStatusCode();
        }

        private void ClarifyForbiddenCode(string responseJson) {
            dynamic errorMessage = JObject.Parse(responseJson)["message"];
            if (errorMessage != null) throw new HttpRequestException(errorMessage.ToString());
        }
    }
}