using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Engine
{
    [DataContract]
    [KnownType(typeof(PolygonCollider))]
    [KnownType(typeof(PolygonRenderer))]
    public abstract class Component
    {
        private static int id = 0;
        public Entity Entity;
        [DataMember]
        private string name;
        public string Name { get => $"{Entity.Name}.{name}"; set { name = value; } }
        [DataMember]
        public Transformation localTransformation { get; set; }

        public Component()
        {
            name = $"{id++}";
        }
        public virtual void OnAddedToEntity()
        {

        }
    }
}
