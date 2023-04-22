using StateDiagramApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateDiagramApp.Utillity
{
    class XmlFile
    {
        public void ReadXml(out ObservableCollection<State> states)
        {
            // Now we can read the serialized book ...  
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(ObservableCollection<State>));

            try
            {
                using (var file = new System.IO.StreamReader("test.xml"))
                {
                    states = (ObservableCollection<State>)reader.Deserialize(file);
                }
            }
            catch
            {
                states = null;
            }

            return;
        }

        public void WriteXml(ObservableCollection<State> states)
        {
            System.Xml.Serialization.XmlSerializer writer =
                    new System.Xml.Serialization.XmlSerializer(typeof(ObservableCollection<State>));

            try
            {
                using (var file = System.IO.File.Create("test.xml"))
                {
                    writer.Serialize(file, states);
                }
            }
            catch
            {

            }
        }
    }
}
