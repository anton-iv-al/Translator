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

namespace TranslateClient2.Views.Main {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly MainPresenter _presenter;

        public MainWindow() {
            InitializeComponent();
            _presenter = new MainPresenter(this);
        }

        public string TranslatedText {
            get => TranslatedControl.Content as string;
            set => TranslatedControl.Content = value;
        }

        public string InputText {
            get => InputControl.Text;
            set => InputControl.Text = value;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _presenter.OnLoaded();
        }

        private async void InputControl_TextChanged(object sender, TextChangedEventArgs e) {
            await _presenter.InputChanged((sender as TextBox).Text);
        }
        
    }
}