using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public class Entity
    {
        public Scene scene;
        public string Name { get; set; }
        public List<IUpdatable> Updatables;
        private List<IRenderable> Renderables;

        public Entity(string name = "")
        {
            this.Name = name;
            this.Updatables = new List<IUpdatable>();
            this.Renderables = new List<IRenderable>();
        }
        public void Update()
        {
            foreach(IUpdatable updatable in Updatables)
            {
                updatable.Update();
            }
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
        }
    }
}
