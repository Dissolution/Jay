/*using System;
using System.Collections.Generic;
using System.Threading;
using Jay.Randomization;

namespace Jay.Sandbox.Fun.Linez
{
    public interface IPulse
    {
        
    }

    public class EnergyPulse : IPulse
    {
        private int _energy;

        public int Energy
        {
            get => _energy;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _energy = value;
            }
        }

        public EnergyPulse(int energy)
        {
            if (energy < 0)
                throw new ArgumentOutOfRangeException(nameof(energy));
            _energy = energy;
        }
    }

    public interface IPulseGenerator
    {
        IPulse Generate();
    }
    
    public class EnergyPulseGenerator : IPulseGenerator
    {
        private readonly IRandomizer _randomizer;
        
        public int MinOutput { get; set; }
        public int MaxOutput { get; set; }

        public EnergyPulseGenerator(IRandomizer randomizer)
        {
            _randomizer = randomizer;
        }
        
        /// <inheritdoc />
        public IPulse Generate()
        {
            int e = _randomizer.Between(MinOutput, MaxOutput);
            return new EnergyPulse(e);
        }
    }

    public class World
    {
        private readonly IPulseGenerator _pulseGenerator;
        private int _creatureId;

        internal IZLogger Logger { get; }
        
        public List<ICreature> Creatures { get; }
        
        public World(IPulseGenerator pulseGenerator)
        {
            _pulseGenerator = pulseGenerator;
            this.Creatures = new List<ICreature>(1024);
        }

        internal int GetNextCreatureId()
        {
            return Interlocked.Increment(ref _creatureId);
        }
    }

    public interface ICreature : IEquatable<ICreature>
    {
        int Id { get; }
        CreatureGenome Genome { get; }
        int Age { get; }
        
        int Health { get; }

        void Process(IPulse pulse);
    }

    public class CreatureGenome
    {
        public int MaxHealth { get; set; }
        // Some sort of trigger for when/how healing occurs
    }

    public enum CreatureAction
    {
        Heal,
    }
    
    public interface IZLogger
    {
        void LogCreatureAction(ICreature creature, CreatureAction action, string message, params object?[] args);
    }
    
    public class Creature : ICreature
    {
        private readonly IZLogger _logger;
        
        /// <inheritdoc />
        public int Id { get; }

        /// <inheritdoc />
        public CreatureGenome Genome { get; }

        /// <inheritdoc />
        public int Age { get; }

        /// <inheritdoc />
        public int Health { get; protected set;  }

        public int MaxHealth => Genome.MaxHealth;

        internal Creature(World world, CreatureGenome genome)
        {
            _logger = world.Logger;
            this.Id = world.GetNextCreatureId();
            this.Genome = genome;
            this.Age = 0;
            this.Health = Genome.MaxHealth;
        }

        /// <inheritdoc />
        public virtual void Process(IPulse pulse)
        {
            // TODO: Health Trigger
            var healingNeeded = this.MaxHealth - this.Health;
            if (healingNeeded > 0)
            {
                if (pulse is EnergyPulse energyPulse && energyPulse.Energy > 0)
                {
                    var heal = Math.Min(energyPulse.Energy, healingNeeded);
                    this.Health += heal;
                    energyPulse.Energy -= heal;
                    _logger.LogCreatureAction(this, CreatureAction.Heal, "Energy Pulse healed {heal}"
                }
            }
            
           
        }

        /// <inheritdoc />
        public bool Equals(ICreature? creature)
        {
            return creature != null && creature.Id == this.Id;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Creature creature && creature.Id == this.Id;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.Id;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{GetType().Name}#{Id}";
        }
    }
}*/