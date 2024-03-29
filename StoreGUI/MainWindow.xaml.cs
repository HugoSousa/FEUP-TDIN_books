﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Windows;
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
            OrderCallback callback = new OrderCallback(this);
            InstanceContext instanceContext = new InstanceContext(callback);
            _proxy = new OrderServiceClient(instanceContext);
            _proxy.Subscribe(null);
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

            int sellResult = _proxy.StoreSell(title, client, quantity);
            if (sellResult == 0)
            {
                List<string> availablePrinters = _proxy.GetAvailablePrinters().ToList();

                if (availablePrinters.Count > 0)
                {
                    PrintersWindow pw = new PrintersWindow(availablePrinters);
                    pw.Owner = this;
                    pw.ShowDialog();
                    if (pw.DialogResult == true && pw.SelectedPrinter != null)
                    {
                        UpdatePrice(null, null); //ensure the price is updated
                        Receipt receipt = new Receipt()
                        {
                            Client = client,
                            Title = title,
                            Quantity = quantity,
                            TotalPrice = Double.Parse(PriceOutput.Text, CultureInfo.InvariantCulture)
                        };
                        _proxy.PrintReceipt(pw.SelectedPrinter, receipt);
                        ErrorSell = "Sell successfull. The receipt has been printed on " + pw.SelectedPrinter + ".";
                    }
                    else
                    {
                        ErrorSell = "Sell successfull. No printing processed.";
                    }
                }
                else
                {
                    ErrorSell = "Sell successfull. There are no available printers.";
                }
                
                RefreshBooksList(null, null);
            }
            else if (sellResult == -1)
            {
                //create an order
                OrderDataWindow odw = new OrderDataWindow();
                odw.Owner = this;
                odw.ShowDialog();
                if (odw.DialogResult == true)
                {
                    int insertedOrder = _proxy.CreateOrder(title, client, odw.Email, odw.Address, quantity);
                    if (insertedOrder > 0)
                    {
                        ErrorSell = "Order sucessfull sent to the warehouse. Order id: " + insertedOrder;
                    }
                    else
                    {
                        ErrorSell = "Some error ocurred creating an order.";
                    }
                }
                else
                {
                    ErrorSell = "Order of book cancelled.";
                }  
            }
            else
            {
                ErrorSell = "There was some error processing the sell.";
            }
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

        public void RefreshBooksList(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                _books = _proxy.GetBooks();
                BooksListSell.ItemsSource = _books.DefaultView;
                BooksListStock.ItemsSource = _books.DefaultView;
            }));
        }
    }

    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class OrderCallback : IOrderServiceCallback
    {
        private readonly MainWindow _gui;

        public OrderCallback(MainWindow gui)
        {
            _gui = gui;
        }

        public void OnSuccessfullSell()
        {
            _gui.RefreshBooksList(null, null);
        }

        public void OnSucessfullStockUpdate()
        {
            _gui.RefreshBooksList(null, null);
        }

        public void OnPrint(Receipt receipt)
        {
            
        }
    }
}
