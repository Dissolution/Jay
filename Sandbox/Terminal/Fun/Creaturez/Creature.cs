using System;
using Jay.Text;
using Jay;
using Jay.Randomization;

namespace Jay.Sandbox.Fun.Creaturez
{
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

        public Genome CreateGenome(IRandomizer random)
        {
            var str = random.Boolean() ? Mother.Strength : Father.Strength;
            var dex = random.Boolean() ? Mother.Dexterity : Father.Dexterity;
            var sta = random.Boolean() ? Mother.Stamina : Father.Stamina;
            var @int = random.Boolean() ? Mother.Intelligence : Father.Intelligence;
            var wis = random.Boolean() ? Mother.Wisdom : Father.Wisdom;
            var cha = random.Boolean() ? Mother.Charisma : Father.Charisma;
            return new Genome(str, dex, sta, @int, wis, cha);
        }

        private readonly TextBuilder _builder = TextBuilder.Rent();
        private readonly string _separator = "   ";

        /// <inheritdoc />
        public override string ToString()
        {
            _builder.Clear();
            _builder.AppendAlign("Strength", Alignment.Center, 13).Append(_separator)
                    .AppendAlign("Dexterity", Alignment.Center, 13).Append(_separator)
                    .AppendAlign("Stamina", Alignment.Center, 13).Append(_separator)
                    .AppendAlign("Intelligence", Alignment.Center, 13).Append(_separator)
                    .AppendAlign("Wisdom", Alignment.Center, 13).Append(_separator)
                    .AppendAlign("Charisma", Alignment.Center, 13).AppendNewLine();
            _builder.Append($"{Mother.Strength} | {Father.Strength}").Append(_separator)
                    .Append($"{Mother.Dexterity} | {Father.Dexterity}").Append(_separator)
                    .Append($"{Mother.Stamina} | {Father.Stamina}").Append(_separator)
                    .Append($"{Mother.Intelligence} | {Father.Intelligence}").Append(_separator)
                    .Append($"{Mother.Wisdom} | {Father.Wisdom}").Append(_separator)
                    .Append($"{Mother.Charisma} | {Father.Charisma}").AppendNewLine();
            _builder.AppendAlign($"{Strength,5:#0.00}", Alignment.Center, 13).Append(_separator)
                    .AppendAlign($"{Dexterity,5:#0.00}", Alignment.Center, 13).Append(_separator)
                    .AppendAlign($"{Stamina,5:#0.00}", Alignment.Center, 13).Append(_separator)
                    .AppendAlign($"{Intelligence,5:#0.00}", Alignment.Center, 13).Append(_separator)
                    .AppendAlign($"{Wisdom,5:#0.00}", Alignment.Center, 13).Append(_separator)
                    .AppendAlign($"{Charisma,5:#0.00}", Alignment.Center, 13).AppendNewLine();
            _builder.Append($"Age: {Age,2}  Gen: {Generation,8}").AppendNewLine();
            return _builder;
        }
    }
}