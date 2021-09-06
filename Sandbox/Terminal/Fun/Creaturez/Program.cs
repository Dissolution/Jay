using System;
using System.Collections.Generic;
using System.Linq;
using Jay.Randomization;

namespace Jay.Sandbox.Fun.Creaturez
{
    public class Program
    {
        private static readonly IRandomizer _random = new Xoshiro256StarStarRandomizer();
        private static int _generation = 0;

        private const int MAX_AGE = 100_000;

        private static int _counter = 0;

        public static void Do()
        {
            var creatures = new List<Creature>();
            for (var i =0 ;i < 20; i++)
                creatures.Add(RandomCreature());

            var comparer = new CreatureComparer(_random);

           // var prevHigh = 0d;
            while (true)
            {
                //Wait.For(100);
                
                //Everyone ages
                creatures.ForEach(c => c.Age++);

                //Get best 4
                var best4 = creatures.OrderByDescending(comparer).Take(4).ToList();
                creatures.Clear();
                creatures.AddRange(best4);
                _counter++;
                if (_counter%250 == 0)
                    Display(creatures);
                
                //a + b
                creatures.Add(Breed(creatures[0], creatures[1]));
                //a + c
                creatures.Add(Breed(creatures[0], creatures[2]));
                //a + d
                creatures.Add(Breed(creatures[0], creatures[3]));
                //b + c
                creatures.Add(Breed(creatures[1], creatures[2]));
                //b + d
                creatures.Add(Breed(creatures[1], creatures[3]));
                //c + d
                creatures.Add(Breed(creatures[2], creatures[3]));

                ////A random
                //var rand = RandomCreature();
                ////a + r
                //creatures.Add(Breed(creatures[0], rand));
                ////b + r
                //creatures.Add(Breed(creatures[1], rand));
                ////c + r
                //creatures.Add(Breed(creatures[2], rand));
                ////d + r
                //creatures.Add(Breed(creatures[3], rand));
                creatures.Add(RandomCreature());
                creatures.Add(RandomCreature());

                //Age out
                for (var i = 0; i < 4; i++)
                {
                    if (creatures[i].Age > MAX_AGE)
                        creatures[i] = RandomCreature();
                }
            }
        }

        private static Stat RandomStat(int maxValue, double maxMulti)
        {
            if (_random.Boolean())
            {
                return new Stat(_random.Between(1, maxValue));
            }
            else
            {
                var iValue = _random.Between(13.100d, (int) (maxMulti * 100d));
                return new Stat(iValue / 100d);
            }
        }

        private static Genome RandomGenome()
        {
            var str = RandomStat(75,4d);
            var dex = RandomStat(1000,2.5d);
            var sta = RandomStat(100,5d);
            var ntl = RandomStat(10,3d);
            var wis = RandomStat(10,3d);
            var cha = RandomStat(10,3d);
            return new Genome(str, dex, sta, ntl, wis, cha);
        }

        private static Creature RandomCreature()
        {
            return new Creature(RandomGenome(), RandomGenome(), _generation++);
        }

        private static void Display(IEnumerable<Creature> creatures)
        {
            Console.Clear();
            foreach (var creature in creatures)
                Console.WriteLine(creature);
            Console.WriteLine();
        }

        private static Creature Breed(Creature a, Creature b)
        {
            var aGenome = a.CreateGenome(_random);
            var bGenome = b.CreateGenome(_random);
            return new Creature(aGenome, bGenome, _generation++);
        }
    }
}
