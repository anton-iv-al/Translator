using System;
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

        private readonly string _detectUri = "https://translate.yandex.net/api/v1.5/tr.json/detect";

        private readonly string _tranlateKey =
            "trnsl.1.1.20180210T231505Z.997c6871d9a87bcf.16842afcd8e17a5af9b55dbfc5e66ca6dab4692f";

        private readonly string _probablyLanguages;

        private readonly string _firstLanguage = "ru";

        private readonly string _secondLanguage = "en";

        private readonly HttpClient _httpClient = new HttpClient();

        public YandexTranslator() {
            _probablyLanguages = $"{_firstLanguage},{_secondLanguage}";
        }

        public async Task<string> Translate(string text) {
            string detectedLang = await LanguageOfText(text);

            var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>() {
                {"key", _dictionaryKey},
                {"lang", TranslateDirection(detectedLang)},
                {"text", text}
            });

            string responseJson = await JsonFromGetRequest(_dictionaryUri, requestContent);

            return ParsedTranslateResponse(responseJson);
        }

        private async Task<string> LanguageOfText(string text) {
            var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>() {
                {"key", _tranlateKey},
                {"hint", _probablyLanguages},
                {"text", text}
            });

            string responseJson = await JsonFromGetRequest(_detectUri, requestContent);

            return ParsedDetectLanguageResponse(responseJson);
        }

        private async Task<string> JsonFromGetRequest(string uri, HttpContent requestContent) {
            var uriBuilder = new UriBuilder(uri) {Query = await requestContent.ReadAsStringAsync()};
            var response = await _httpClient.GetAsync(uriBuilder.Uri);

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

        private string ParsedDetectLanguageResponse(string responseJson) {
            return JObject.Parse(responseJson)["lang"].ToString();
        }

        private void EnsureSuccessStatusCode(HttpResponseMessage response, string responseJson) {
            if (response.StatusCode == HttpStatusCode.Forbidden) ClarifyForbiddenCode(responseJson);
            response.EnsureSuccessStatusCode();
        }

        private void ClarifyForbiddenCode(string responseJson) {
            dynamic errorMessage = JObject.Parse(responseJson)["message"];
            if (errorMessage == null) throw new HttpRequestException("Forbidden with unknown reason.");
            throw new HttpRequestException(errorMessage.ToString());
        }

        private string TranslateDirection(string detectedLanguage) {
            string template = "{0}-{1}";

            if (detectedLanguage == _firstLanguage) return String.Format(template, _firstLanguage, _secondLanguage);
            else return String.Format(template, detectedLanguage, _firstLanguage);
        }
    }
}