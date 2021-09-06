using System;

namespace Jay
{
    /// <summary>
    /// An <see cref="Action{T}"/> that works on a <see langword="ref"/> <paramref name="value"/>
    /// </summary>
    /// <typeparam name="T">The type of value being referenced</typeparam>
    public delegate void RefAction<T>(ref T value);
}