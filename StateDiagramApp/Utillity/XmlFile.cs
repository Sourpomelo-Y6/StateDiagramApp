﻿using StateDiagramApp.Model;
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
        private string filename;

        public XmlFile(string filename)
        {
            this.filename = filename;
        }

        public void ReadXml(out ObservableCollection<State> states)
        {
            // Now we can read the serialized book ...  
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(ObservableCollection<State>));

            try
            {
                using (var file = new System.IO.StreamReader(filename))
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
                using (var file = System.IO.File.Create(filename))
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
