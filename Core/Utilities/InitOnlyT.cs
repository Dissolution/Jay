using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jay
{
    [DebuggerTypeProxy(typeof(InitOnlyDebugView<>))]
    [DebuggerDisplay("HasValue={HasValue}, Value={ValueForDebugDisplay}")]
    public class InitOnly<T>
    {
        private static readonly object DefaultFallThrough = new object();
        
        // _state, a volatile reference, is set to null after _value has been set
        private volatile object? _fallThrough;

        // _value eventually stores the lazily created value. It is valid when _state = null.
        private T? _value;

        /// <summary>Gets the value of the Lazy&lt;T&gt; for debugging display purposes.</summary>
        internal T? ValueForDebugDisplay => !HasValue ? default : _value;

          /// <summary>Gets a value indicating whether the <see cref="System.Lazy{T}"/> has been initialized.
        /// </summary>
        /// <value>true if the <see cref="System.Lazy{T}"/> instance has been initialized;
        /// otherwise, false.</value>
        /// <remarks>
        /// The initialization of a <see cref="System.Lazy{T}"/> instance may result in either
        /// a value being produced or an exception being thrown.  If an exception goes unhandled during initialization,
        /// <see cref="HasValue"/> will return false.
        /// </remarks>
        public bool HasValue => _fallThrough == null;

        /// <summary>Gets the lazily initialized value of the current <see
        /// cref="System.Lazy{T}"/>.</summary>
        /// <value>The lazily initialized value of the current <see
        /// cref="System.Lazy{T}"/>.</value>
        /// <exception cref="System.MissingMemberException">
        /// The <see cref="System.Lazy{T}"/> was initialized to use the default constructor
        /// of the type being lazily initialized, and that type does not have a public, parameterless constructor.
        /// </exception>
        /// <exception cref="System.MemberAccessException">
        /// The <see cref="System.Lazy{T}"/> was initialized to use the default constructor
        /// of the type being lazily initialized, and permissions to access the constructor were missing.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// The <see cref="System.Lazy{T}"/> was constructed with the <see cref="System.Threading.LazyThreadSafetyMode.ExecutionAndPublication"/> or
        /// <see cref="System.Threading.LazyThreadSafetyMode.None"/>  and the initialization function attempted to access <see cref="Value"/> on this instance.
        /// </exception>
        /// <remarks>
        /// If <see cref="HasValue"/> is false, accessing <see cref="Value"/> will force initialization.
        /// Please <see cref="System.Threading.LazyThreadSafetyMode"/> for more information on how <see cref="System.Lazy{T}"/> will behave if an exception is thrown
        /// from initialization delegate.
        /// </remarks>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public T Value => _fallThrough == null ? _value! : throw new InvalidOperationException();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="System.Lazy{T}"/> class that
        /// uses <typeparamref name="T"/>'s default constructor for lazy initialization.
        /// </summary>
        /// <remarks>
        /// An instance created with this constructor may be used concurrently from multiple threads.
        /// </remarks>
        public InitOnly()
        {
            _value = default;
            _fallThrough = DefaultFallThrough;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="System.Lazy{T}"/> class that
        /// uses a pre-initialized specified value.
        /// </summary>
        /// <remarks>
        /// An instance created with this constructor should be usable by multiple threads
        /// concurrently.
        /// </remarks>
        public InitOnly(T? value)
        {
            _value = value;
            _fallThrough = null;
        }

        public bool TryGetValue(out T? value)
        {
            object? fallThrough = _fallThrough;
            if (fallThrough == null)
            {
                value = _value;
                return true;
            }
            value = default;
            return false;
        }
        
        public T? GetOrAdd(T? value)
        {
            // we have to create a copy of state here, and use the copy exclusively from here on in
            // so as to ensure thread safety.
            object? fallThrough = _fallThrough;
            if (fallThrough != null)
            {
                _value = value;
                _fallThrough = null;
            }
            return Value;
        }
        
        public T? GetOrAdd(Func<T?> valueFactory)
        {
            // we have to create a copy of state here, and use the copy exclusively from here on in
            // so as to ensure thread safety.
            object? fallThrough = _fallThrough;
            if (fallThrough != null)
            {
                _value = valueFactory();
                _fallThrough = null;
            }
            return Value;
        }
        
        public T? GetOrAdd<TState>(TState state, Func<TState, T?> valueFactory)
        {
            // we have to create a copy of state here, and use the copy exclusively from here on in
            // so as to ensure thread safety.
            object? fallThrough = _fallThrough;
            if (fallThrough != null)
            {
                _value = valueFactory(state);
                _fallThrough = null;
            }
            return Value;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (HasValue && obj is T value)
            {
                return EqualityComparer<T>.Default.Equals(value, Value);
            }
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            if (HasValue && _value != null)
            {
                return _value.GetHashCode();
            }
            // ReSharper restore NonReadonlyMemberInGetHashCode
            return 0;
        }
        
        /// <summary>Creates and returns a string representation of this instance.</summary>
        /// <returns>The result of calling <see cref="object.ToString"/> on the <see
        /// cref="Value"/>.</returns>
        /// <exception cref="System.NullReferenceException">
        /// The <see cref="Value"/> is null.
        /// </exception>
        public override string? ToString()
        {
            if (HasValue)
            {
                return _value?.ToString() ?? string.Empty;
            }
            else
            {
                return "uninitialized";
            }
        }
    }

    /// <summary>A debugger view of the Lazy&lt;T&gt; to surface additional debugging properties and
    /// to ensure that the Lazy&lt;T&gt; does not become initialized if it was not already.</summary>
    internal sealed class InitOnlyDebugView<T>
    {
        // The Lazy object being viewed.
        private readonly InitOnly<T> _initOnly;

        /// <summary>Constructs a new debugger view object for the provided Lazy object.</summary>
        /// <param name="initOnly">A Lazy object to browse in the debugger.</param>
        public InitOnlyDebugView(InitOnly<T> initOnly)
        {
            _initOnly = initOnly;
        }

        public bool HasValue => _initOnly.HasValue;
        
        /// <summary>Returns the value of the Lazy object.</summary>
        public T? Value => _initOnly.ValueForDebugDisplay;
    }
}
