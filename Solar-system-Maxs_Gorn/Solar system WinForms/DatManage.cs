using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Solar_system_WinForms
{
    class DatManage
    {
        public void SerializeXML<T>(T users, string path) where T : class
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                xml.Serialize(fs, users);
            }
        }
        public T DeserializeXML<T>(string SelectedPath) where T : class
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));
            using (FileStream fs = new FileStream(SelectedPath, FileMode.OpenOrCreate))
            {
                return (T)xml.Deserialize(fs);
            }

        }
    }
}
