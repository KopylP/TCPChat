using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatCommonClassLibrary;
namespace ChatServer.models
{
    class ChatContext : DbContext
    {
        public ChatContext() : base("ChatDB") {}
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
