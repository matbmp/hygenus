using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine
{
    [DataContract(IsReference = true)]
    public class Transformation
    {
        [DataMember]
        public Vector2 Translation;
        [DataMember]
        public Quaternion Rotation;
        [DataMember]
        public Quaternion Gyration;
        [DataMember]
        public Vector2 Scale = new Vector2(1.0F);

        public Transformation()
        {
            Translation = Vector2.Zero;
            Rotation = Quaternion.Identity;
            Gyration = Quaternion.Identity;
        }
        public Transformation(Vector2 translation, Quaternion rotation)
        {
            Translation = translation;
            Rotation = rotation;
        }
        public Transformation(Vector2 translation)
        {
            this.Translation = translation;
            this.Rotation = Quaternion.Identity;
        }


        public void Serialize(BinaryWriter writer)
        {
            SerializationHelp.Serialize(writer, Translation);
            SerializationHelp.Serialize(writer, Rotation);
            SerializationHelp.Serialize(writer, Gyration);
            SerializationHelp.Serialize(writer, Scale);
        }
        public void Deserialize(BinaryReader reader)
        {
            SerializationHelp.Deserialize(reader, out Translation);
            SerializationHelp.Deserialize(reader, out Rotation);
            SerializationHelp.Deserialize(reader, out Gyration);
            SerializationHelp.Deserialize(reader, out Scale);
        }
    }
}
