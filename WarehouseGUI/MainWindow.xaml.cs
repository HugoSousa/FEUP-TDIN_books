using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Windows;
using WarehouseGUI.WarehouseService;

namespace WarehouseGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private WarehouseServiceClient _proxy;
        private DataTable _requests;

        private string _error;
        public string Error
        {
            get
            {
                return _error;
            }
            set
            {
                if (_error != value)
                {
                    _error = value;
                    OnPropertyChanged("Error");
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            MyServiceCallback callback = new MyServiceCallback(this);
            InstanceContext instanceContext = new InstanceContext(callback);
            _proxy = new WarehouseServiceClient(instanceContext);
            _proxy.Subscribe();
            _requests = _proxy.GetOpenRequests();

            RequestsList.ItemsSource = _requests.DefaultView;
        }

        public void RequestAdded(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                _requests = _proxy.GetOpenRequests();
                RequestsList.ItemsSource = _requests.DefaultView;
            }));
        }

        private void ShipRequest(object sender, RoutedEventArgs e)
        {
            Error = "";

            DataRowView item = (DataRowView)RequestsList.SelectedItem;
            int requestId = (int)item.Row[0];
            if (_proxy.ShipRequest(requestId) == 0)
            {
                _requests = _proxy.GetOpenRequests();
                RequestsList.ItemsSource = _requests.DefaultView;
                Error = "Request successfully completed.";
            }
            else
            {
                Error = "Some error happened on shipping the request.";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        ~MainWindow()
        {
            _proxy.Unsubscribe();
        }
    }

    [CallbackBehavior(ConcurrencyMode=ConcurrencyMode.Multiple, UseSynchronizationContext=false)]
    public class MyServiceCallback : IWarehouseServiceCallback
    {
        private readonly MainWindow _gui;

        public MyServiceCallback(MainWindow gui)
        {
            _gui = gui;
        }

        public void OnCallback()
        {
            Console.WriteLine("ahh");
            _gui.RequestAdded(null, null);
        }
    }
}
