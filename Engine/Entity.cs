using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    /// <summary>
    /// Klasa której obiekty dodajemy do Scene, można do niej podczepić elementy które wymagają auktualizacji(Updatables)
    /// oraz elementy wymagające wyświetlania na ekranie(Renderables).
    /// </summary>
    [DataContract(IsReference = true)]
    public class Entity
    {
        private static int id;

        public Scene scene;
        [DataMember]
        public Transformation transformation;
        [DataMember]
        public Vector2 velocity;
        [DataMember]
        public float angularVelocity;
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public List<Component> Components;
        public List<IUpdatable> Updatables = new List<IUpdatable>();
        private List<IRenderable> Renderables = new List<IRenderable>();
        protected int tick;

        public Entity(string name = null)
        {
            if (name == null) Name = $"entity{id++}";
            else Name = name;
            transformation = new Transformation();
            Components = new List<Component>();
        }
        public virtual void Update()
        {
            foreach(IUpdatable updatable in Updatables)
            {
                updatable.Update();
            }
            DynamicsProvider dynamics = scene.DynamicsProvider;
            dynamics.ApplyVelocity(transformation, velocity);
            dynamics.ApplyAngularVelocity(transformation, angularVelocity);
            tick++;
        }

        public void ApplyImpulse(Vector2 impulse, Vector2 contactVector)
        {
            DynamicsProvider dynamics = scene.DynamicsProvider;
            dynamics.ApplyImpulse(transformation, impulse, contactVector, ref velocity, ref angularVelocity);
        }
        public void ApplyImpulse2(Vector2 impulse, Vector2 contactVector)
        {
            impulse = Vector2.Transform(impulse, (transformation.Gyration));
            DynamicsProvider dynamics = scene.DynamicsProvider;
            dynamics.ApplyVelocity(transformation, impulse);
            velocity += impulse;
            angularVelocity += Math2d.Cross(contactVector, impulse);
        }

        public void Render(HyperColorEffect graphics)
        {
            foreach(IRenderable renderable in Renderables)
            {
                renderable.Render(graphics);
            }
        }

        public void AddComponent(Component component)
        {
            component.Entity = this;
            if (component is IUpdatable)
            {
                Updatables.Add(component as IUpdatable);
            }
            else if (component is IRenderable)
            {
                Renderables.Add(component as IRenderable);
            }
            else throw new ArgumentException("Component does not provide any functionality(IUpdatable/IRenderable)");
            Components.Add(component);
            component.OnAddedToEntity();
        }

        public void OnAddedToScene()
        {
            
        }

        [OnDeserialized]
        private void onDeserialized(StreamingContext c)
        {
            Updatables = new List<IUpdatable>();
            Renderables = new List<IRenderable>();
            foreach(Component component in Components)
            {
                component.Entity = this;
                if (component is IUpdatable)
                {
                    Updatables.Add(component as IUpdatable);
                }
                else if (component is IRenderable)
                {
                    Renderables.Add(component as IRenderable);
                }
                component.OnAddedToEntity();
            }
        }
    }
}
