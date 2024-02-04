using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackMagic
{
    public class ParticleHandler : Trait, TFixedUpdate, TDraws
    {
        public List<IParticleEmitter> Emitters { get; private set; }

        public ParticleHandler(Entity parent) : base(parent)
        {
            Emitters = new List<IParticleEmitter>();
            Globals.MainEntityBatch.disposeEvent += Dispose;
            //^^ Note: Should change to parent's entity batch instead of hard coded to main
        }

        public override void Update(GameTime gameTime) 
        {
            for (int i = Emitters.Count - 1; i >= 0; i--)
            {
                Emitters[i].Prerender();
                if (Emitters[i].Exists)
                    continue;
                Emitters[i].Dispose();
                Emitters.RemoveAt(i);
            }
        }

        public void FixedUpdate()
        {
            for (int i = 0; i < Emitters.Count; i++)
                Emitters[i].Update();
        }

        public void Draw()
        {
            for (int i = 0; i < Emitters.Count; i++)
                Emitters[i].Draw();
        }
        
        public void Dispose()
        {
            foreach (var emitter in Emitters)
                emitter.Dispose();
        }
    }
}
