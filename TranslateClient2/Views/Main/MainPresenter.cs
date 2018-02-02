using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TranslateClient2.Views.Main {
    public class MainPresenter : Presenter<MainWindow> {
        private readonly InputTimeSmoother _smoother;

        public MainPresenter(MainWindow view) : base(view) {
            _smoother = new InputTimeSmoother(OnSmoothedInputChanged);
        }

        public void OnLoaded() {
//            _view.WindowState = WindowState.Minimized;
//            _view.Visibility = Visibility.Collapsed;
        }

        public async Task InputChanged(string input) {
            await _smoother.OnRealInputChanged(input);
        }

        private void OnSmoothedInputChanged(string input) {
            _view.TranslatedText = input;
        }
    }
}