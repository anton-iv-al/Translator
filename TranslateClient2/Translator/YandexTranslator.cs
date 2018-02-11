using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;
using YamlDotNet.RepresentationModel;

namespace TranslateClient2.Translator {
    public class YandexTranslator : ITranslator {
        private string _dictionaryUri;

        private string _dictionaryKey;

        private string _detectLanguageUri;

        private string _tranlateKey;

        private string _probablyLanguages;

        private string _firstLanguage;

        private string _secondLanguage;

        private readonly HttpClient _httpClient = new HttpClient();

        public YandexTranslator() {
            ParseConfig();
            _probablyLanguages = $"{_firstLanguage},{_secondLanguage}";
        }

        private void ParseConfig() {
            dynamic config = YamlHelper.ConfigFromFile("Translator/YandexTranslatorConfig.yaml");

            _dictionaryUri = config["dictionary_uri"].ToString();
            _dictionaryKey = config["dictionary_key"].ToString();
            _detectLanguageUri = config["detect_language_uri"].ToString();
            _tranlateKey = config["translate_key"].ToString();
            _firstLanguage = config["first_language"].ToString();
            _secondLanguage = config["second_language"].ToString();
        }

        public async Task<TranslateResult> Translate(string text) {
            var direction = await TranslateDirection(text);

            var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>() {
                {"key", _dictionaryKey},
                {"lang", TranslateDirectionString(direction)},
                {"text", text}
            });

            string responseJson = await JsonFromGetRequest(_dictionaryUri, requestContent);
            string translatedText = ParsedTranslateResponse(responseJson);

            return new TranslateResult(translatedText, direction.Item1, direction.Item2);
        }

        private async Task<string> LanguageOfText(string text) {
            var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>() {
                {"key", _tranlateKey},
                {"hint", _probablyLanguages},
                {"text", text}
            });

            string responseJson = await JsonFromGetRequest(_detectLanguageUri, requestContent);

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
            dynamic def = JObject.Parse(responseJson)["def"];
            if (def.Count == 0) return String.Empty;

            dynamic tr = def[0]["tr"];
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

        private string TranslateDirectionString(Tuple<string, string> direction) {
            return $"{direction.Item1}-{direction.Item2}";
        }

        private async Task<Tuple<string, string>> TranslateDirection(string text) {
            string detectedLang = await LanguageOfText(text);
            if (detectedLang == _firstLanguage) return new Tuple<string, string>(_firstLanguage, _secondLanguage);
            else return new Tuple<string, string>(detectedLang, _firstLanguage);
        }
    }
}