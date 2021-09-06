using System.Collections.Generic;
using Jay.Randomization;


namespace Jay.Roulette
{
    public class Wheel
    {
       private readonly IRandomizer _randomizer;
        
        public Wheel(IRandomizer randomizer)
        {
            _randomizer = randomizer;
        }

        public IEnumerable<Pocket> Spin()
        {
            var rand = _randomizer;
            ulong r;
            byte slice;
            do {
                r = rand.ULong();
                // Slices of 6 bits (64 / 6 = 10.66_), so we only get 10 (lose 4 bits)
                for (var i = 0; i < 10; i++, r >>= 6)
                {
                    slice = (byte) (r & 0b00111111);
                    if (slice <= 37)
                    {
                        yield return NotSafe.As<byte, Pocket>(slice);
                    }
                }
            } while (true);
        }
    }
}