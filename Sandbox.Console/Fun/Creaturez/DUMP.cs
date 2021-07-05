/*
using System;
using Jay;
using Jay.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fun
{
    class Program
    {
        private static readonly Random _random = new Random();
        private static int _generation = 0;

        private const int MAX_AGE = 100_000;

        private static int _counter = 0;

        static void Do()
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
            if (_random.Next(2) == 1)
            {
                return new Stat(_random.Next(1, maxValue));
            }
            else
            {
                var iValue = _random.Next(100, (int) (maxMulti * 100d));
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

    public class CreatureComparer : IComparer<Creature>
    {
        private readonly Random _random;

        public CreatureComparer(Random random)
        {
            _random = random;
        }

        /// <inheritdoc />
        public int Compare(Creature x, Creature y)
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

    public readonly struct Stat
    {
        private const int PLACES = 2;

        public int? Value { get; }
        public double? Multiplier { get; }

        public Stat(int value)
        {
            this.Value = value;
            this.Multiplier = null;
        }

        public Stat(double multiplier)
        {
            this.Value = null;
            this.Multiplier = Math.Round(multiplier, PLACES);
        }

        public static double Apply(Stat a, Stat b)
        {
            if (a.Value.TryGetValue(out var aValue))
            {
                if (b.Value.TryGetValue(out var bValue))
                {
                    return Math.Max(aValue, bValue);        //Biggest value stat
                }
                else if (b.Multiplier.TryGetValue(out var bMulti))
                {
                    return Math.Round(aValue * bMulti, PLACES); //Multiply
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else if (a.Multiplier.TryGetValue(out var aMulti))
            {
                if (b.Value.TryGetValue(out var bValue))
                {
                    return Math.Round(aMulti * bValue, PLACES); //Multiply
                }
                else if (b.Multiplier.TryGetValue(out var bMulti))
                {
                    return Math.Round(1d, PLACES);      //Multiplicative identity = 1
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Value.HasValue ? $"{Value,3+PLACES:##.00}" : $"{Multiplier,2+PLACES:##.00}x";
        }
    }

    public class Genome
    {
        public Stat Strength { get; }
        public Stat Dexterity { get; }
        public Stat Stamina { get; }
        public Stat Intelligence { get; }
        public Stat Wisdom { get; }
        public Stat Charisma { get; }

        public Genome(Stat str, Stat dex, Stat sta, Stat @int, Stat wis, Stat cha)
        {
            this.Strength = str;
            this.Dexterity = dex;
            this.Stamina = sta;
            this.Intelligence = @int;
            this.Wisdom = wis;
            this.Charisma = cha;
        }
    }

    public class Creature
    {
        public Genome Mother { get; }
        public Genome Father { get; }

        public double Strength { get; }
        public double Dexterity { get; }
        public double Stamina { get; }
        public double Intelligence { get; }
        public double Wisdom { get; }
        public double Charisma { get; }

        public int Age { get; set; }

        public int Generation { get; }

        public Creature(Genome mother, Genome father, int gen)
        {
            this.Mother = mother;
            this.Father = father;
            this.Generation = gen;

            this.Strength = Stat.Apply(Mother.Strength, Father.Strength);
            this.Dexterity = Stat.Apply(Mother.Dexterity, Father.Dexterity);
            this.Stamina = Stat.Apply(Mother.Stamina, Father.Stamina);
            this.Intelligence = Stat.Apply(Mother.Intelligence, Father.Intelligence);
            this.Wisdom = Stat.Apply(Mother.Wisdom, Father.Wisdom);
            this.Charisma = Stat.Apply(Mother.Charisma, Father.Charisma);
            //this.Value = Strength + Dexterity + Stamina + Intelligence + Wisdom + Charisma;

            this.Age = 0;
        }

        public Genome CreateGenome(Random random)
        {
            var str = random.AnyBool() ? Mother.Strength : Father.Strength;
            var dex = random.AnyBool() ? Mother.Dexterity : Father.Dexterity;
            var sta = random.AnyBool() ? Mother.Stamina : Father.Stamina;
            var @int = random.AnyBool() ? Mother.Intelligence : Father.Intelligence;
            var wis = random.AnyBool() ? Mother.Wisdom : Father.Wisdom;
            var cha = random.AnyBool() ? Mother.Charisma : Father.Charisma;
            return new Genome(str, dex, sta, @int, wis, cha);
        }

        private readonly TextBuilder _builder = new TextBuilder(256);
        private readonly string _separator = "   ";

        /// <inheritdoc />
        public override string ToString()
        {
            _builder.Clear();
            _builder.AppendAlign("Strength", 13, Alignment.Center).Append(_separator)
                .AppendAlign("Dexterity", 13, Alignment.Center).Append(_separator)
                .AppendAlign("Stamina", 13, Alignment.Center).Append(_separator)
                .AppendAlign("Intelligence", 13, Alignment.Center).Append(_separator)
                .AppendAlign("Wisdom", 13, Alignment.Center).Append(_separator)
                .AppendAlign("Charisma", 13, Alignment.Center).AppendLine();
            _builder.Append($"{Mother.Strength} | {Father.Strength}").Append(_separator)
                .Append($"{Mother.Dexterity} | {Father.Dexterity}").Append(_separator)
                .Append($"{Mother.Stamina} | {Father.Stamina}").Append(_separator)
                .Append($"{Mother.Intelligence} | {Father.Intelligence}").Append(_separator)
                .Append($"{Mother.Wisdom} | {Father.Wisdom}").Append(_separator)
                .Append($"{Mother.Charisma} | {Father.Charisma}").AppendLine();
            _builder.AppendAlign($"{Strength,5:#0.00}", 13, Alignment.Center).Append(_separator)
                .AppendAlign($"{Dexterity,5:#0.00}", 13, Alignment.Center).Append(_separator)
                .AppendAlign($"{Stamina,5:#0.00}", 13, Alignment.Center).Append(_separator)
                .AppendAlign($"{Intelligence,5:#0.00}", 13, Alignment.Center).Append(_separator)
                .AppendAlign($"{Wisdom,5:#0.00}", 13, Alignment.Center).Append(_separator)
                .AppendAlign($"{Charisma,5:#0.00}", 13, Alignment.Center).AppendLine();
            _builder.Append($"Age: {Age,2}  Gen: {Generation,8}").AppendLine();
            return _builder;
        }
    }
}
*/
