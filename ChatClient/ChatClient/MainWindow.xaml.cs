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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using ChatCommonClassLibrary;

namespace ChatClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    class Data
    {
        public string Text { get; set; }
        public string Name { get; set; }
    }
    public partial class MainWindow : Window
    {
        User user;
        private LoginInformation LogInfo { get; set; }
        private SaveLoginInformation saveLoginInformation = new SaveLoginInformation("log.chat"); 
        TcpClient client = new TcpClient();
        NetworkStream stream;
        void GetMessage()
        {
            while(true)
            {
                var bytes = 0;
                var data = new byte[256];
                var builder = new StringBuilder();
                do
                {
                     bytes = stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                } while (stream.DataAvailable);
                Message message = JsonConvert.DeserializeObject<Message>(builder.ToString());
                if (message.Text != "")
                    stkMessageField.Dispatcher.Invoke(() =>
                    {
                        CreateMessageBlock(message.User.Name, message.Text);
                    });
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            MessageContainerInformation mci = null;
            do
            {
                mci = AuthenticationAndConnect(mci != null ? mci.ErrorInformation : null);
            }
            while (mci.IsCorrectAuthentication != true);
        }

        private MessageContainerInformation AuthenticationAndConnect(string ErrorInformation)
        {
            MessageContainerInformation mci = new MessageContainerInformation();
            var loginfo = saveLoginInformation.Get();
            if (loginfo != null)
            {
                mci = this.Connect(loginfo);
            }
            else
            {
                Authentication authentication = new Authentication(ErrorInformation);
                bool? result = authentication.ShowDialog();
                if (result == true && !authentication.IsOpenRegistration)
                {
                    mci = this.Connect(authentication.LogInfo);
                }
                else if (result == true && authentication.IsOpenRegistration)
                {
                    Registration reg = new Registration();
                    if (reg.ShowDialog() == true)
                    {
                        mci = this.Connect(reg.LogInfo);
                    }
                }
                else
                {
                    Application.Current.MainWindow.Close();
                    mci.IsCorrectAuthentication = true;
                }
            }
            return mci;
        }
        private MessageContainerInformation Connect(LoginInformation info)
        {
            MessageContainerInformation mci = new MessageContainerInformation();
            IPEndPoint point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            TcpClient client = new TcpClient();
            try
            {
                client.Connect(point);
                stream = client.GetStream();
                //Відправляємо дані авторизації
                string dataJSON = JsonConvert.SerializeObject(info);
                var data = Encoding.UTF8.GetBytes(dataJSON);
                stream.Write(data, 0, data.Length);
                //Отримуємо дані підтвердження 
                int bytes = 0;
                data = new byte[256];
                StringBuilder builder = new StringBuilder();
                do
                {
                    bytes = stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                } while(stream.DataAvailable);

                mci = JsonConvert.DeserializeObject<MessageContainerInformation>(builder.ToString());
                if (mci.IsCorrectAuthentication)
                {
                    foreach(var m in mci.Messages)
                    {
                        CreateMessageBlock(m.User.Name, m.Text);
                        Task.Factory.StartNew(() => GetMessage());
                        dckSend.IsEnabled = true;
                    }
                    saveLoginInformation.Save(info);
                    Task.Factory.StartNew(() => GetMessage());
                }
                else
                {
                    //stream.Close();
                    client.Close();
                }
                return mci;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, " Error");
                //stream.Close();
                client.Close();
            }
            return mci;
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string message = new TextRange(txtMessage.Document.ContentStart, txtMessage.Document.ContentEnd).Text.Trim();
            if(!message.Equals(""))
            {
                var data = Encoding.UTF8.GetBytes(message);
                try
                {
                    stream.Write(data, 0, data.Length);
                    txtMessage.Document.Blocks.Clear();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, " Error");
                }
            }
        }
        private void CreateMessageBlock(String name, String message)
        {
            Border border = new Border();
            border.Margin = new Thickness(2);
            CornerRadius corner = new CornerRadius(4.0);
            border.CornerRadius = corner;
            border.Background = Brushes.Purple;
            border.HorizontalAlignment = HorizontalAlignment.Left;
            TextBlock block = new TextBlock();
            block.MaxWidth = 400;
            block.Text = String.Format("{0}: {1}", name, message);
            block.Style = this.FindResource("txtMessageStyle") as Style;
            block.Padding = new Thickness(3);
            border.Child = block;
            stkMessageField.Children.Add(border);
        }
    }
}
