using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BlackMagic
{
    //TRAIT ABSTRACT CLASS DEPRECATED
    public abstract class Trait<T> : Trait where T : Entity
    {
        public new T parent;
        public Trait(T parent, byte priority = defaultPriority) : base(parent, priority) { this.parent = parent; }
    }

    public abstract class Trait : TUpdates
    {
        public Entity parent;
        public bool isActive = true;
        public const byte defaultPriority = 100;
        public byte priority { get; protected set; } = defaultPriority;
        public int Priority { get => priority; }

        public Trait(Entity parent, byte priority = defaultPriority)
        {
            this.parent = parent;
            this.priority = priority;
        }

        public abstract void Update(GameTime gameTime);

        public void Update() => Update(Globals.gt);
    }

    public interface TUpdates
    {
        int Priority { get; }
        void Update();
    }

    public interface TDraws
    {
        void Draw();
    }

    public interface TFixedUpdate
    {
        byte priority { get; }
        void FixedUpdate();
    }

    public interface TDrawsRippleInfluence
    {
        void DrawRippleInfluence();
    }
}
