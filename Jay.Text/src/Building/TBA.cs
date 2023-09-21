namespace Jay.Text.Building;

public delegate void TBA<in TBuilder>(TBuilder builder)
    where TBuilder : TextWriter;