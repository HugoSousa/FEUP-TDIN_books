using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using StoreGUI.Annotations;

namespace StoreGUI
{
    /// <summary>
    /// Interaction logic for OrderDataWindow.xaml
    /// </summary>
    public partial class OrderDataWindow : Window, INotifyPropertyChanged
    {
        private string _error;
        public string Email { get; private set; }
        public string Address { get; private set; }

        public string ErrorText
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
                    OnPropertyChanged("ErrorText");
                }
            }
        }

        public OrderDataWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorText = "";
            Email = EmailInput.Text;
            Address = AddressInput.Text;

            if (Address == "" || Email == "")
            {
                ErrorText = "All fields are mandatory.";
                return;
            }

            if (!new EmailAddressAttribute().IsValid(Email))
            {
                ErrorText = "Invalid email.";
                return;
            }

            DialogResult = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
