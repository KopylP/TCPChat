using System;
using System.Text;
using System.Net.Sockets;
using ChatServer.models;
using System.Data.Entity;
using System.Linq;
using Newtonsoft.Json;
using ChatCommonClassLibrary;

class ClientObject
{
    public string Email { get; set; }
    public string Id { get; private set; }
    public NetworkStream Stream { get; private set; }
    public TcpClient client { get; private set; }
    private ServerObject server;
    private ChatContext db = new ChatContext();
    public ClientObject(TcpClient client, ServerObject server)
    {
        Id = System.Guid.NewGuid().ToString();
        this.client = client;
        this.server = server;
        server.AddConnection(this);
    }
    public void Proccess()
    {
        try
        {
            this.Stream = client.GetStream();
            //Отримуємо інформацію з логіном та паролем у форматі JSON
            string lI = getMessage();
            LoginInformation logInfo = JsonConvert.DeserializeObject<LoginInformation>(lI);
            MessageContainerInformation mci = new MessageContainerInformation() { IsCorrectAuthentication = false };
            if (logInfo.IsLogin)
            {
                User user = db.Users.FirstOrDefault(n => n.Email == logInfo.Email);
                
                if (user != null && user.Password == logInfo.Password)
                {
                    mci.IsCorrectAuthentication = true;
                    mci.Messages = db.Messages.Include(n => n.User);
                    Email = user.Email;
                }
                else
                {
                    mci.IsCorrectAuthentication = false;
                    mci.ErrorInformation = "Login Or Password is incorrect";
                }
                
            }
            else
            {
                if (db.Users.Where(n => n.Email == logInfo.Email).Count() == 0)
                {
                    User user = new User() { Name = logInfo.Name, Email = logInfo.Email, Password = logInfo.Password };
                    db.Users.Add(user);
                    db.SaveChanges();
                }
                else
                {
                    mci.IsCorrectAuthentication = false;
                    mci.ErrorInformation = "User Email is Already Exist";
                }
            }
            server.SendDataForOneUser(this, mci);
            if(!mci.IsCorrectAuthentication)
            {
                client.Close();
                server.RemoveConnection(this.Id);
                return;
            }
            //
            //server.SendDataForOneUser(this);
            while (true)
            {
                try
                {
                    string messageText = String.Format($"{getMessage()}");
                    Message message = new Message() { Text = messageText };
                    User user = db.Users.FirstOrDefault(n => n.Email == this.Email);
                    if(user != null)
                    {
                        message.UserId = user.Id;
                        message.Date = DateTime.Now;
                        db.Messages.Add(message);
                        db.SaveChanges();
                        message.User = user;
                        server.SandMessageForAll(message);
                    }
                    
                    Console.WriteLine("User have written: '{0}'", message);

                }
                catch(Exception ex)
                {
                    this.Stream.Close();
                    this.client.Close();
                    this.server.RemoveConnection(this.Id);
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
        }
        catch
        {

        }
    }
    private string getMessage()
    {
        int bytes = 0;
        byte[] data = new byte[256];
        System.Text.StringBuilder builder = new StringBuilder();
        do
        {
            bytes = this.Stream.Read(data, 0, data.Length);
            builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
        }
        while (Stream.DataAvailable);
        return builder.ToString();
    }

}