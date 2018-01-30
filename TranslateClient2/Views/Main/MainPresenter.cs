using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TranslateClient2.Views.Main {
    public class MainPresenter : Presenter<MainWindow> {
        public MainPresenter(MainWindow view) : base(view) { }

        public void OnLoaded() {
//            _view.WindowState = WindowState.Minimized;
            _view.Visibility = Visibility.Collapsed;
        }

        public void InputChanged(string text) {
            _view.TranslatedText = text;
        }
    }
}