using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ChatCommonClassLibrary;

namespace ChatClient
{
    class SaveLoginInformation
    {
        public string Path { get; set; }
        public SaveLoginInformation(string Path)
        {
            this.Path = Path;
        }
        public bool Save(LoginInformation info)
        {
            try
            {
                using (FileStream stream = new FileStream(Path, FileMode.CreateNew, FileAccess.Write))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, info);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public LoginInformation Get()
        {
            try
            {
                using (FileStream stream = new FileStream(Path, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return (LoginInformation)formatter.Deserialize(stream);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
