using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Engine;

namespace Hygenus
{

    [DataContract]
    public class Level
    {
        [DataMember]
        public string Name;
        [DataMember]
        public List<Entity> Entities;
        [DataMember]
        public FinishLine FinishLine;
        [DataMember]
        public PlayerEntity Player;

        public Level(string name, Scene scene, FinishLine finish)
        {
            this.Name = name;
            this.Entities = scene.Entities;
            this.FinishLine = finish;
        }
    }
}
