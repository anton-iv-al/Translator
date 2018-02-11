using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslateClient2.Translator
{
    public interface ITranslator {
        Task<TranslateResult> Translate(string input);
    }
}
