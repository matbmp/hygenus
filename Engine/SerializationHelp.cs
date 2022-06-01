using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine
{
    public static class SerializationHelp
    {
        public delegate void ActionSecondOut<T1, T2>(T1 a, out T2 b);
        public static void Serialize(BinaryWriter writer, Vector3 vector)
        {
            writer.Write(vector.X);
            writer.Write(vector.Y);
            writer.Write(vector.Z);
        }
        public static void Deserialize(BinaryReader reader, out Vector3 vector)
        {
            vector = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public static void Serialize(BinaryWriter writer, Vector2 vector)
        {
            writer.Write(vector.X);
            writer.Write(vector.Y);
        }
        public static void Deserialize(BinaryReader reader, out Vector2 vector)
        {
            vector = new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }

        public static void Serialize(BinaryWriter writer, Quaternion quaternion)
        {
            writer.Write(quaternion.X);
            writer.Write(quaternion.Y);
            writer.Write(quaternion.Z);
            writer.Write(quaternion.W);
        }
        public static void Deserialize(BinaryReader reader, out Quaternion quaternion)
        {
            quaternion = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public static void SerializeArray<T>(BinaryWriter writer, T[] array, Action<BinaryWriter, T> arrayWriter)
        {
            writer.Write(array.Length);
            for(int i = 0; i< array.Length; i++)
            {
                arrayWriter(writer, array[i]);
            }
        }
        public static void DeserializeArray<T>(BinaryReader reader, out T[] array, ActionSecondOut<BinaryReader, T> arrayReader)
        {
            array = new T[reader.ReadInt32()];
            for (int i = 0; i < array.Length; i++)
            {
                arrayReader(reader, out array[i]);
            }
        }

        public static void SerializeList<T>(BinaryWriter writer, List<T> collection) where T : IBinarySerializable
        {
            writer.Write(collection.Count);
            foreach(T item in collection)
            {
                item.Serialize(writer);
            }
        }

        public static void DeserializeList<T>(BinaryReader reader, out List<T> collection) where T : IBinarySerializable
        {
            int capacity = reader.ReadInt32();
            collection = new List<T>(capacity);
            for(int i = 0; i < capacity; i++)
            {
                collection[i].Deserialize(reader);
            }
        }
    }
}
