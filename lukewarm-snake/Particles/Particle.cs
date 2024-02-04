using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackMagic
{    
    public interface IParticleEmitter
    {
        Vector2 Pos { get; set; }
        bool Exists { get; }

        void Prerender();
        void Update();
        void Draw();
        void Dispose();
    }

    public abstract class ParticleEmitter<TParticle, TSetParams> : IParticleEmitter where TParticle : IParticle<TSetParams>
    {
        public TParticle[] Particles { get; protected set; }
        public const int DefaultMaxParticles = 100;

        public Vector2 Pos { get; set; } = Vector2.Zero;
        public bool Exists { get; set; } = true;
        public bool IsActive { get; set; } = true;

        //Render variables
        protected RenderTarget2D rt;

        public float Layer { get; set; } = 0f;

        public ParticleEmitter(Vector2 pos, 
            int maxParticles = DefaultMaxParticles)
        {
            Exists = true;
            Pos = pos;
            Particles = new TParticle[maxParticles];

            rt = new(Globals.spriteBatch.GraphicsDevice, Globals.Camera.Width, Globals.Camera.Height);
            InitParticles();
        }

        protected abstract void InitParticles();

        //TODO: Make particles prerender in batches to prevent repeat Begin calls
        public virtual void Prerender()
        {
            Globals.spriteBatch.GraphicsDevice.SetRenderTarget(rt);
            Globals.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            Globals.spriteBatch.Begin(SpriteSortMode.Texture, samplerState: SamplerState.PointClamp);
           
            for (int i = 0; i < Particles.Length; i++)
            {
                if (Particles[i] != null && Particles[i].IsActive)
                    Particles[i].Draw();
            }

            Globals.spriteBatch.End();
            Globals.spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        public virtual void Update()
        {
            for (int i = 0; i < Particles.Length; i++)
            {
                if (Particles[i].IsActive)
                    Particles[i].Update();
            }
        }

        public virtual void Draw()
        {
            Globals.spriteBatch.Draw(rt,
                Vector2.Zero,
                new Rectangle(0, 0, Globals.Camera.Width, Globals.Camera.Height),
                Color.White,
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                Layer);
        }

        public virtual void Dispose() =>
            rt.Dispose();
    }

    
    public interface IParticle<T>
    {
        Vector2 Pos { get; set; }
        Vector2 DrawPos { get; }
        Vector2 DeltaPos { get; }
        bool IsActive { get; }

        void Set(T setParams);
        void Update();
        void Draw();
    }

    public abstract class Particle<SetParams> : IParticle<SetParams>
    {
        public Vector2 Pos { get; set; } = Vector2.Zero;
        public Vector2 DrawPos { get; protected set; } = Vector2.Zero;
        public Vector2 DeltaPos { get; set; } = Vector2.Zero;

        protected Vector2 prevPos;

        public bool IsActive { get; set; } = true;

        public Particle(Vector2? pos = null)
        {
            Pos = DrawPos = prevPos = pos.HasValue ? pos.Value : Vector2.Zero;
            DeltaPos = Vector2.Zero;
        }

        public abstract void Set(SetParams setParams);

        public abstract void Update();

        protected void ApplyForces()
        {
            prevPos = Pos;
            Pos += DeltaPos;
        }

        public abstract void Draw();

        protected void CalcDrawPos() =>
            DrawPos = Vector2.Lerp(prevPos, Pos, Globals.ALPHA);
    }
}
