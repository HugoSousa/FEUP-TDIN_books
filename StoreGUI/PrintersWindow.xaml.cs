using System.Collections.Generic;
using System.Windows;

namespace StoreGUI
{
    /// <summary>
    /// Interaction logic for PrintersWindow.xaml
    /// </summary>
    public partial class PrintersWindow : Window
    {
        public List<string> Printers { get; set; }
        public string SelectedPrinter { get; set; }

        public PrintersWindow(List<string> printers)
        {
            Printers = printers;
            this.DataContext = this;
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedPrinter = (string) PrintersBox.SelectedValue;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedPrinter = null;
            DialogResult = false;
        }
    }
}
