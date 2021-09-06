using System;
using System.Collections.Generic;
using Jay.Randomization;

namespace Jay.Sandbox.Fun.Creaturez
{
    public class CreatureComparer : IComparer<Creature>
    {
        private readonly IRandomizer _random;

        public CreatureComparer(IRandomizer random)
        {
            _random = random;
        }

        /// <inheritdoc />
        public int Compare(Creature? x, Creature? y)
        {
            var xSta = x.Stamina;
            var ySta = y.Stamina;
            
            var xDex = (int) Math.Round(x.Dexterity);
            var yDex = (int) Math.Round(y.Dexterity);

            var timer = 0;
            while (true)
            {
                timer++;

                if (xSta <= 0 && ySta <= 0)
                    return 0;       //tie
                if (xSta <= 0)
                    return -1;       //y wins, x < y
                if (ySta <= 0)
                    return 1;      //x wins, y < x

                if (timer % xDex == 0)
                {
                    //X hits
                    ySta -= x.Strength;
                }

                if (timer % yDex == 0)
                {
                    //Y hits
                    xSta -= y.Strength;
                }
            }
        }
    }
}