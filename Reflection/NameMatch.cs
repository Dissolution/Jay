using Jay.Text;

namespace Jay.Reflection
{
    public readonly struct NameMatch : IRenderable
    {
        public static implicit operator NameMatch(string name) => new NameMatch(name, MatchType.Exact);
        public static implicit operator NameMatch((string Name, MatchType MatchType) tuple) => new NameMatch(tuple.Name, tuple.MatchType);

        public static bool operator ==(NameMatch x, NameMatch y) => x.Equals(y);
        public static bool operator !=(NameMatch x, NameMatch y) => !x.Equals(y);
        
        public static readonly NameMatch Any = new NameMatch(null, MatchType.IgnoreCase);
        
        public readonly string? Name;
        public readonly MatchType MatchType;

        public NameMatch(string? name)
        {
            this.Name = name;
            this.MatchType = MatchType.Exact;
        }
        public NameMatch(string? name, MatchType matchType)
        {
            this.Name = name;
            this.MatchType = matchType;
        }

        public bool Matches(string? name)
        {
            if (Name is null) return true;
            if (string.IsNullOrWhiteSpace(name)) return false;
            if (MatchType.HasFlag(MatchType.IgnoreCase))
            {
                return string.Equals(Name, name, StringComparison.OrdinalIgnoreCase);
            }
            return string.Equals(Name, name);
        }
        
        public bool Equals(NameMatch nameMatch)
        {
            return MatchType == nameMatch.MatchType &&
                   Matches(nameMatch.Name);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is NameMatch nameMatch) return Equals(nameMatch);
            if (obj is string name) return Matches(name);
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hasher = new HashCode();
            if (MatchType.HasFlag(MatchType.IgnoreCase))
            {
                hasher.Add(Name, StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                hasher.Add(Name, StringComparer.Ordinal);
            }
            hasher.Add(MatchType);
            return hasher.ToHashCode();
        }

        public override string ToString()
        {
            return IRenderable.Render(this);
        }

        public void Render(ref StringHandler handler)
        {
            if (Name is null)
            {
                handler.Append('*');
            }
            else
            {
                if (this.MatchType.HasFlag<MatchType>(MatchType.EndsWith))
                {
                    handler.Append('*');
                }

                if (this.MatchType.HasFlag<MatchType>(MatchType.IgnoreCase))
                {
                    handler.Append(Name!.ToUpper());
                }
                else
                {
                    handler.Append(this.Name!);
                }

                if (this.MatchType.HasFlag<MatchType>(MatchType.BeginsWith))
                {
                    handler.Append('*');
                }
            }
        }
    }
}