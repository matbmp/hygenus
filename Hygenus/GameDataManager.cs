using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Hygenus
{
    public class GameDataManager
    {
        string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"levels\");

        public Level getLevel(string name)
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(Level),
                new List<Type> { typeof(FinishLine), typeof(PlayerEntity), typeof(HyperPolygonCollider) });
            Stream stream = new FileStream(name, FileMode.Open, FileAccess.Read);
            return (Level)dcs.ReadObject(stream);
        }
        public string[] getAllLevels()
        {
            Directory.CreateDirectory(path);
            return Directory.GetFiles(path);
        }
        public void saveLevel(Level level)
        {
            DataContractSerializer dcs = new DataContractSerializer(typeof(Level),
                new List<Type> { typeof(FinishLine), typeof(PlayerEntity), typeof(HyperPolygonCollider) });
            Stream stream = new FileStream(Path.Combine(path, level.Name), FileMode.Create);
            dcs.WriteObject(stream, level);
        }
    }
}
