using System.Data;
using System.Windows;
using StoreGUI.OrderStore;

namespace StoreGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OrderServiceClient _proxy;
        private DataTable _books;

        public MainWindow()
        {
            _proxy = new OrderServiceClient();
            InitializeComponent();
            _books = _proxy.GetBooks();

            //listBox1.SelectedValuePath = "title";
            BooksList.ItemsSource = _books.DefaultView;
        }
    }
}
