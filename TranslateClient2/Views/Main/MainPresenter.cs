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
            IsInTray = true;
        }

        public async Task OnTranslateHotKey() {
            IsInTray = false;

            if (!Clipboard.ContainsText()) return;

            string text = Clipboard.GetText();
            var task = InputChanged(text, false);

            _view.InputText = text;

            await task;
        }

        private bool IsInTray {
            get => _view.WindowState == WindowState.Minimized && _view.Visibility == Visibility.Collapsed;
            set {
                if (value) {
                    _view.WindowState = WindowState.Minimized;
                    _view.Visibility = Visibility.Collapsed;
                }
                else {
                    _view.Show();
                    _view.WindowState = WindowState.Normal;
                    _view.Visibility = Visibility.Visible;
                    _view.Activate();
                    _view.Topmost = true; // не убирать
                    _view.Topmost = false;
                    _view.Focus();
                }
            }
        }

        public void OnTrayMouseDoubleClick() {
            IsInTray = false;
        }

        public void OnTrayMenuOpenClick() {
            IsInTray = false;
        }

        public void OnTrayMenuExitClick() {
            Application.Current.Shutdown();
        }

        public async Task InputChanged(string input, bool withDelay = true) {
            string smoothedInput = await _smoother.OnRealInputChanged(input, withDelay);
            if (smoothedInput != null) await Translate(smoothedInput);
        }

        private async Task Translate(string text) {
            try {
                var translateResult = await AppContext.Instance.Translator.Translate(text);

                _view.TranslatedText = translateResult.TranslatedText;
                _view.InputLanguageText = translateResult.InputTextLanguage;
                _view.TranslatedLanguageText = translateResult.TranslatedTextLanguage;
            }
            catch (HttpRequestException e) {
                _view.TranslatedText = e.Message;
            }
        }

        public void OnWindowCloseButton() {
            IsInTray = true;
        }
    }
}