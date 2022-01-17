﻿using Jay.Text;

namespace Jay.Dumping;

public interface IDumper
{
    bool CanDump(Type type);

    void Dump(TextBuilder text, object? value, DumpLevel level = DumpLevel.Self);
}