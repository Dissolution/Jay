namespace Jay.Sandbox.Fun.Creaturez
{
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
}