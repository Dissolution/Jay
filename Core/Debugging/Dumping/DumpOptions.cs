using System;

namespace Jay.Debugging
{
    
    
    
    public class DumpOptions
    {
        public static DumpOptions Default => new DumpOptions();
        
        public string? Format
        {
            get
            {
                if (Formats is null)
                    return null;
                if (Formats.Length == 0)
                    return null;
                return Formats[0];
            }
        }
        public string[]? Formats { get; }
        public IFormatProvider? FormatProvider { get; set; }

        public DumpOptions()
        {
            this.Formats = Array.Empty<string>();
            this.FormatProvider = null;
        }
        
        public DumpOptions(IFormatProvider? provider,
                           params string[] formats)
        {
            this.FormatProvider = provider;
            this.Formats = formats;
        }
    }

    public class MemberDumpOptions : DumpOptions
    {
        public new static MemberDumpOptions Default => new MemberDumpOptions(false, true, true);
        
        public bool Attributes { get; set; }
        public bool Preamble { get; set; }
        public bool InstanceType { get; set; }

        public MemberDumpOptions()
            : base()
        {
            this.Attributes = false;
            this.Preamble = false;
            this.InstanceType = false;
        }

        public MemberDumpOptions(bool attributes,
                                 bool preamble,
                                 bool instanceType)
            : base()
        {
            this.Attributes = attributes;
            this.Preamble = preamble;
            this.InstanceType = instanceType;
        }
    }
}