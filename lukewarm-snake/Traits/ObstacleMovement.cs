using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using Microsoft.Xna.Framework;

namespace lukewarm_snake
{
    public class ObstacleMovement : TUpdates
    {
        private Entity parent;
        public Vector2 Heading;

        public static float MinTimeMod { get => Globals.MinTimeMod; }

        public int Priority => Trait.defaultPriority;

        public ObstacleMovement(Entity parent, Vector2 heading)
        {
            this.parent = parent;
            Heading = heading;
        }

        public void Update()
        {
            parent.DeltaPos = Heading * (Globals.TimeMod < MinTimeMod ? MinTimeMod : Globals.TimeMod);

            if (((new Vector2(Globals.Camera.Width, Globals.Camera.Height) / 2f) - parent.Pos).Length() > SpawnManager.ObstacleSpawnDist)
            {
                parent.GetTrait<ObstacleRenderer>().DisposeIndividual();
                parent.exists = false;
            }
        }
    }
}
