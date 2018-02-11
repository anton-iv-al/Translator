using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslateClient2.Translator
{
    public struct TranslateResult
    {
        public string TranslatedText { get; }

        public string InputTextLanguage { get; }

        public string TranslatedTextLanguage { get; }

        public TranslateResult(string translatedText, string inputTextLanguage, string translatedTextLanguage) {
            TranslatedText = translatedText;
            InputTextLanguage = inputTextLanguage;
            TranslatedTextLanguage = translatedTextLanguage;
        }
    }
}
