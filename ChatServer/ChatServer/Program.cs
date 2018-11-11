using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using ChatServer.models;
namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Task task = null;
            ServerObject server = null;
            try
            {
                server = new ServerObject();
                task = Task.Factory.StartNew(() => server.Listen());
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                server.Disconect();
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
            Task.WaitAll(task);
        }
    }
}
