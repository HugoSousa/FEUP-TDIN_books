using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace StoreGUI
{
    /// <summary>
    /// Interaction logic for PrintersWindow.xaml
    /// </summary>
    public partial class PrintersWindow : Window, INotifyPropertyChanged
    {
        public List<string> Printers { get; set; }
        public string SelectedPrinter { get; set; }

        private string _errorLabel;
        public string ErrorLabel
        {
            get
            {
                return _errorLabel;
            }
            set
            {
                if (_errorLabel != value)
                {
                    _errorLabel = value;
                    OnPropertyChanged("ErrorLabel");
                }
            }
        }

        public PrintersWindow(List<string> printers)
        {
            Printers = printers;
            this.DataContext = this;
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel = "";
            SelectedPrinter = (string) PrintersBox.SelectedValue;
            if (SelectedPrinter != null)
                DialogResult = true;
            else
            {
                ErrorLabel = "Please select a printer. If you don't need a receipt, click 'Cancel'.";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedPrinter = null;
            DialogResult = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
