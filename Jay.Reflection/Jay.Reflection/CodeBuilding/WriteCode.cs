namespace Jay.Reflection.CodeBuilding;

public delegate void WriteCode<in T>(T? value, CodeBuilder code);