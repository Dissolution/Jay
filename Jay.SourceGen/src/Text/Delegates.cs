namespace Jay.SourceGen.Text;

/// <summary>
/// <b>C</b>ode <b>B</b>uilder <b>A</b>ction<br/>
/// <see cref="Action{T}">Action&lt;CodeBuilder&gt;</see>
/// </summary>
/// <param name="codeBuilder">The <see cref="CodeBuilder"/> to act upon</param>
public delegate void CBA(CodeBuilder codeBuilder);

/// <summary>
/// <b>C</b>ode <b>B</b>uilder <b>V</b>alue <b>A</b>ction<br/>
/// <see cref="Action{T, T}">Action&lt;CodeBuilder, T&gt;</see>
/// </summary>
/// <typeparam name="T">The <see cref="Type"/> of <paramref name="value"/></typeparam>
/// <param name="codeBuilder">The <see cref="CodeBuilder"/> to act upon</param>
/// <param name="value">The <typeparamref name="T"/> value to use with the <see cref="CodeBuilder"/></param>
public delegate void CBVA<in T>(CodeBuilder codeBuilder, T value);

/// <summary>
/// <b>C</b>ode <b>B</b>uilder <b>V</b>alue <b>P</b>redicate<br/>
/// <see cref="Func{T, T, T}">Func&lt;CodeBuilder, T, bool&gt;</see>
/// </summary>
public delegate bool CBVP<in T>(CodeBuilder codeBuilder, [AllowNull, NotNullWhen(true)] T value);
