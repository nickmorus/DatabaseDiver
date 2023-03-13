using DbDiver.Business;
using DbDiver.Core;
using DbDiver.Modules.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DbDiver.Modules.Models
{
    public class Settings
    {
        public string DatabasePath { set; get; }
        public int SelectedDatabaseIdx { set; get; }
        public ObservableCollection<DbSearchParameter> Parameters { set; get; }        
        string _settingsPath = $"{Directory.GetCurrentDirectory()}\\settings.xml";
        
        public Settings(ObservableCollection<DbSearchParameter> parameters,
            string databasePath, int selectedDatabaseIdx)
        {
            Parameters = parameters;
            DatabasePath = databasePath;
            SelectedDatabaseIdx = selectedDatabaseIdx;
        }

        public Settings()
        {
            Parameters = null;
        }

        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            TextWriter writer = new StreamWriter(_settingsPath);
            serializer.Serialize(writer, this);            
        }

        public void Load()
        {
            if (File.Exists(_settingsPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                using (Stream reader = new FileStream(_settingsPath, FileMode.Open))
                {
                    Settings loadedSettings = serializer.Deserialize(reader) as Settings;
                    if (loadedSettings != null)
                    {
                        Parameters = loadedSettings.Parameters;
                        DatabasePath = loadedSettings.DatabasePath;
                        SelectedDatabaseIdx = loadedSettings.SelectedDatabaseIdx;
                    }
                }
            }
            else
            {
                throw new SettingsNotFoundException();
            }
        }

    }
}
