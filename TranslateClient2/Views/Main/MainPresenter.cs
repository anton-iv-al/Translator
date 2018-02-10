using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TranslateClient2.Views.Main {
    public class MainPresenter : Presenter<MainWindow> {
        private readonly InputTimeSmoother _smoother = new InputTimeSmoother();

        public MainPresenter(MainWindow view) : base(view) { }

        public void OnLoaded() {
//            _view.WindowState = WindowState.Minimized;
//            _view.Visibility = Visibility.Collapsed;
        }

        public async Task InputChanged(string input) {
            string smoothedInput = await _smoother.OnRealInputChanged(input);
            if (smoothedInput != null) await Translate(smoothedInput);
        }

        private async Task Translate(string text) {
            try {
                string translated = await AppContext.Instance.Translator.Translate(text);
                _view.TranslatedText = translated;
            }
            catch (HttpRequestException e) {
                _view.TranslatedText = e.Message;
            }
        }
    }
}