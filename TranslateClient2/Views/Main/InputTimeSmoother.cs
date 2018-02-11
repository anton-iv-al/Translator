using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslateClient2.Views.Main
{
    class InputTimeSmoother
    {
        private string _oldInput = String.Empty;
        private string _currentInput = String.Empty;

        private int _delayCounter = 0;
        
        public async Task<String> OnRealInputChanged(string input, bool withDelay = true) {
            _currentInput = input;


            if (withDelay) {
                _delayCounter++;
                await Task.Delay(1000);
                _delayCounter--;
                if (_delayCounter != 0) return null;
            }

            if (_currentInput == _oldInput) return null;
            if (String.IsNullOrWhiteSpace(_currentInput)) return null;
            
            _oldInput = _currentInput;

            return _currentInput;
        }
    }
}
