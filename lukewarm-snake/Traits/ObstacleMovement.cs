using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using Microsoft.Xna.Framework;
using static BlackMagic.Globals;

namespace lukewarm_snake
{
    public class ObstacleMovement : TUpdates
    {
        private Obstacle parent;
        public Vector2 Heading;

        public int Priority => Trait.defaultPriority;
        public ObstacleMovement(Obstacle parent, Vector2 heading)
        {
            this.parent = parent;
            Heading = heading;
        }

        public void Update()
        {
            parent.DeltaPos = Heading * (TimeMod < MinTimeMod ? MinTimeMod : TimeMod);

            if (((new Vector2(Globals.Camera.Width, Globals.Camera.Height) / 2f) - parent.Pos).Length() > ObstacleManager.ObstacleSpawnDist)
            {
                //parent.GetTrait<ObstacleRenderer>().DisposeIndividual();
                //parent.exists = false;
                //parent.IsActive = false;
            }
        }
    }
}
