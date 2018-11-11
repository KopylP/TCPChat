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
using ChatCommonClassLibrary;
namespace ChatClient
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public LoginInformation LogInfo { get; set; } = new LoginInformation();
        public Registration()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            LogInfo.Name = txtName.Text;
            LogInfo.Password = txtPassword.Password;
            LogInfo.Email = txtEmail.Text;
            LogInfo.IsLogin = false;
            this.DialogResult = true;
        }
    }
}
