using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GlobalHotKey;

namespace TranslateClient2.Views.Main {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly MainPresenter _presenter;

        private HotKey _translateHotKey;

        public MainWindow() {
            InitializeComponent();
            RegisterHotkeys();
            _presenter = new MainPresenter(this);
        }

        private void RegisterHotkeys() {
            _translateHotKey = AppContext.Instance.HotKeyManager.Register(Key.T, ModifierKeys.Alt);
            AppContext.Instance.HotKeyManager.KeyPressed += HotkeyEventHandler;
        }

        private async void HotkeyEventHandler(object sender, KeyPressedEventArgs e) {
            if (e.HotKey.Equals(_translateHotKey)) await _presenter.OnTranslateHotKey();
        }

        public string TranslatedText {
            get => TranslatedControl.Text;
            set => TranslatedControl.Text = value;
        }

        public string InputText {
            get => InputControl.Text;
            set => InputControl.Text = value;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            _presenter.OnLoaded();
        }

        private async void InputControl_TextChanged(object sender, TextChangedEventArgs e) {
            await _presenter.InputChanged((sender as TextBox).Text);
        }

        private void TrayIcon_OnTrayMouseDoubleClick(object sender, RoutedEventArgs e) {
            _presenter.OnTrayMouseDoubleClick();
        }

        private void TrayMenuOpen_OnClick(object sender, RoutedEventArgs e) {
            _presenter.OnTrayMenuOpenClick();
        }

        private void TrayMenuExit_OnClick(object sender, RoutedEventArgs e) {
            _presenter.OnTrayMenuExitClick();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            e.Cancel = true;

            _presenter.OnWindowCloseButton();
        }
    }
}