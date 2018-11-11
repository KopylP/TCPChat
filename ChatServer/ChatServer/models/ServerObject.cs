using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using ChatServer.models;
using Newtonsoft.Json;
using System.Data.Entity;
using ChatCommonClassLibrary;
class ServerObject
{
    private static int port = 8000;
    private IPEndPoint point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
    private TcpListener listener;
    private List<ClientObject> clients = new List<ClientObject>();
    private ChatContext db = new ChatContext();
    public void AddConnection(ClientObject obj)
    {
        clients.Add(obj);
    }
    public void RemoveConnection(string id)
    {
        var client = clients.FirstOrDefault(n => n.Id == id);
        if (client != null)
            clients.Remove(client);
    }
    public void Listen()
    {
        try
        {
            listener = new TcpListener(point);
            listener.Start();
            Console.WriteLine("Server have been started!");
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                ClientObject clObj = new ClientObject(client, this);
                Task.Factory.StartNew(() => clObj.Proccess());

            }
        }
        catch
        {

        }
    }
    public void SandMessageForAll(Message message)
    {
        string messageJson = JsonConvert.SerializeObject(message);
        foreach (var clObj in clients)
        {
            byte[] bmessage = UnicodeEncoding.UTF8.GetBytes(messageJson);
            try
            {
                clObj.Stream.Write(bmessage, 0, bmessage.Length);
            }
            catch
            {
                Console.WriteLine("Ошибка отправки сообщения!");
            }
        }
    }
    public void SendDataForOneUser(ClientObject clObj, MessageContainerInformation inf)
    {
        string sendMessage = JsonConvert.SerializeObject(inf);
        byte[] bmessage = UnicodeEncoding.UTF8.GetBytes(sendMessage);
        try
        {
            clObj.Stream.Write(bmessage, 0, bmessage.Length);
        }
        catch
        {
            Console.WriteLine("Ошибка отправки сообщения!");
        }
    }
    public void Disconect()
    {
        listener.Stop();
        foreach (var obj in clients)
        {
            if (obj != null)
                obj.client.Close();

        }
        Environment.Exit(0);
    }
}