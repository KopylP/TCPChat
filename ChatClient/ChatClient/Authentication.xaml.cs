using ChatCommonClassLibrary;
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

namespace ChatClient
{
    /// <summary>
    /// Логика взаимодействия для Authentication.xaml
    /// </summary>
    public partial class Authentication : Window
    { 
        public LoginInformation LogInfo { get; set; }
        public bool IsOpenRegistration { get; set; } = false;
        public Authentication(string ErrorInfo)
        {
            InitializeComponent();
            if(ErrorInfo != null)
            {
                lblError.Content = ErrorInfo;
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtEmail.Text != null && txtPassword.Password != null)
            {
                LogInfo = new LoginInformation() { Email = txtEmail.Text, Password = txtPassword.Password, IsLogin = true };
                this.DialogResult = true;
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            IsOpenRegistration = true;
            this.DialogResult = true;
        }
    }
}
