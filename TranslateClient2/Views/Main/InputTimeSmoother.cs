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

        private readonly Action<string> _onSmoothedInput;

        private int _delayCounter = 0;


        public InputTimeSmoother(Action<string> onSmoothedInput) {
            _onSmoothedInput = onSmoothedInput;
        }

        public async Task OnRealInputChanged(string input) {
            _currentInput = input;

            _delayCounter++;
            await Task.Delay(1000);
            _delayCounter--;

            if (_delayCounter == 0) OnTimeElapsed();
        }

        private void OnTimeElapsed() {
            if (_currentInput == _oldInput) return;

            _oldInput = _currentInput;
            _onSmoothedInput(_currentInput);
        }
    }
}
