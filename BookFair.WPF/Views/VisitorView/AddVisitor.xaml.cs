using BookFair.Core.Controllers;
using BookFair.Core.Models;
using BookFair.WPF.DTO;
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
using System.Windows.Shapes;

namespace BookFair.WPF.Views.VisitorView
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    
    public partial class AddVisitor : Window
    {
        private readonly VisitorController _visitorController;
        private readonly AddressController _addressController;

        public VisitorDTO Visitor { get; set; }
        public AddressDTO Address { get; set; }

        public AddVisitor(VisitorController visitorController, AddressController addressController)
        {
            InitializeComponent();

            _visitorController = visitorController ?? throw new ArgumentNullException(nameof(visitorController));
            _addressController = addressController ?? throw new ArgumentNullException(nameof(addressController));

            Visitor = new VisitorDTO();
            Address = new AddressDTO();

            DataContext = this;

            // inicijalni enable
            Loaded += (_, __) => UpdateConfirm();
        }

        private void OnAnyTextChanged(object sender, RoutedEventArgs e) => UpdateConfirm();

        private void UpdateConfirm()
        {
            if (Confirm == null) return;

            bool visitorOk = string.IsNullOrEmpty(Visitor?.IsValid);
            bool addressOk = string.IsNullOrEmpty(Address?.IsValid);

            Confirm.IsEnabled = visitorOk && addressOk;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Visitor.IsValid) || !string.IsNullOrEmpty(Address.IsValid))
            {
                var sErr = string.IsNullOrEmpty(Visitor.IsValid) ? "" : Visitor.IsValid + "\n";
                var aErr = string.IsNullOrEmpty(Address.IsValid) ? "" : Address.IsValid;
                MessageBox.Show(sErr + aErr, Properties.Resources.Msg_ValidationErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var addr = _addressController.AddAddress(Address.ToAddress());
            var visitor = Visitor.ToVisitor();
            visitor.Address = addr;

            _visitorController.AddVisitor(visitor);
            DialogResult = true;
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
