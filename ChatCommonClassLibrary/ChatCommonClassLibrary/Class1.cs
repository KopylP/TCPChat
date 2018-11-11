using System;
using System.Collections.Generic;

namespace ChatCommonClassLibrary
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public User User { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public IEnumerable<Message> Messages { get; set; }
        public User()
        {
            Messages = new List<Message>();
        }
    }
    public class MessageContainerInformation
    {
        public bool IsCorrectAuthentication { get; set; }
        public IEnumerable<Message> Messages { get; set; }
        public string ErrorInformation { get; set; }

    }
    [Serializable]
    public class LoginInformation
    {
        public bool IsLogin;
        public string Password;
        public string Email;
        public string Name;
    }
    public class MessageInformation
    {
        public Message Message { get; set; }
        public User User { get; set; }
    }
}
