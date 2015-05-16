using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using StoreGUI.OrderStore;

namespace StoreGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly OrderServiceClient _proxy;
        private DataTable _books;

        private string _errorSell;
        private string _errorStock;
        public string ErrorSell
        {
            get
            {
                return _errorSell;
            }
            set
            {
                if (_errorSell != value)
                {
                    _errorSell = value;
                    OnPropertyChanged("ErrorSell");
                }
            }
        }

        public string ErrorStock
        {
            get
            {
                return _errorStock;
            }
            set
            {
                if (_errorStock != value)
                {
                    _errorStock = value;
                    OnPropertyChanged("ErrorStock");
                }
            }
        }

        public MainWindow()
        {
            _proxy = new OrderServiceClient();
            InitializeComponent();
            DataContext = this;
            _books = _proxy.GetBooks();

            BooksListSell.ItemsSource = _books.DefaultView;
            BooksListStock.ItemsSource = _books.DefaultView;
        }

        private void SellBook(object sender, RoutedEventArgs e)
        {
            ErrorSell = "";

            int quantity;
            string title;
            string client;

            if (BooksListSell.SelectedItem == null)
            {
                ErrorSell = "Please select a book.";
                return;
            }

            DataRowView item = (DataRowView)BooksListSell.SelectedItem;
            title = (string)item.Row[0];

            client = ClientInput.Text;
            if (client == "")
            {
                ErrorSell = "Please fill the client name.";
                return;
            }

            
            if (! Int32.TryParse(QuantityInputSell.Text, out quantity) || quantity < 0)
            {
                ErrorSell = "Invalid quantity.";
                return;
            }

            if (_proxy.StoreSell(title, client, quantity) == 0)
            {
                ErrorSell = "Sell successfull. The receipt has been printed.";
                RefreshBooksList(null, null);
            }
            else
                ErrorSell = "There was some error processing the sell.";

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdatePrice(object sender, RoutedEventArgs e)
        {
            int quantity;
            float unitPrice;

            if (BooksListSell.SelectedItem == null)
            {
                return;
            }

            DataRowView item = (DataRowView)BooksListSell.SelectedItem;
            unitPrice = (float)item.Row[2];

            if (int.TryParse(QuantityInputSell.Text, out quantity))
            {
                if(quantity > 0)
                    PriceOutput.Text = (quantity*unitPrice).ToString(CultureInfo.InvariantCulture);
            }
        }

        private void AddStock(object sender, RoutedEventArgs e)
        {
            ErrorStock = "";
            int quantity;
            string title;

            if (BooksListStock.SelectedItem == null)
            {
                ErrorStock = "Please select a book.";
                return;
            }

            DataRowView item = (DataRowView)BooksListStock.SelectedItem;
            title = (string)item.Row[0];

            if (! int.TryParse(QuantityInputStock.Text, out quantity) || quantity <= 0)
            {
                ErrorStock = "Invalid quantity.";
                return;
            }

            if (_proxy.UpdateStock(title, quantity) == 0)
            {
                ErrorStock = "Stock successfully updated.";
                RefreshBooksList(null, null);
            }
            else
            {
                ErrorStock = "Some error occurred updating stock";
            }
        }

        private void RefreshBooksList(object sender, RoutedEventArgs e)
        {
            _books = _proxy.GetBooks();
            BooksListSell.ItemsSource = _books.DefaultView;
            BooksListStock.ItemsSource = _books.DefaultView;
        }
    }
}
