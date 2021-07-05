/*
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Jay.Comparison;

namespace Jay.Settings
{
    /// <summary>
    /// The base class for any settings.
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    public abstract class OptionsBase<TSettings>
        where TSettings : OptionsBase<TSettings>, new()
    {
        /// <summary>
        /// Gets the default settings.
        /// </summary>
        public static TSettings Default => new TSettings();

        /// <summary>
        /// The cached settings.
        /// </summary>
        private readonly Dictionary<Type, Box> _settings;

        /// <summary>
        /// Construct a new, empty settings.
        /// </summary>
        public OptionsBase()
        {
            _settings = new Dictionary<Type, Box>(0);
        }
        
        /// <summary>
        /// Try to add the specified <paramref name="setting"/> to these settings.
        /// </summary>
        /// <typeparam name="TSetting">The type of setting to try to add.</typeparam>
        /// <param name="setting">The setting to try to add.</param>
        /// <returns>True if the setting did not exist and was added; otherwise, false.</returns>
        protected bool TryAdd<TSetting>(TSetting setting)
        {
            var type = typeof(TSetting);
            if (_settings.ContainsKey(type)) return false;
            _settings[type] = Box.Up<TSetting>(setting);
            return true;
        }

        /// <summary>
        /// Sets the specified <paramref name="setting"/>.
        /// </summary>
        /// <typeparam name="TSetting">The type of setting to set.</typeparam>
        /// <param name="setting">The setting to set.</param>
        protected TSettings Set<TSetting>(TSetting setting)
        {
            var type = typeof(TSetting);
            if (_settings.TryGetValue(type, out Box? box))
            {
                box!.SetValue<TSetting>(setting);
            }
            else
            {
                _settings[type] = Box.Up<TSetting>(setting);
            }
            return this as TSettings;
        }

        /// <summary>
        /// Tries to get an existing <paramref name="setting"/>.
        /// </summary>
        /// <typeparam name="TSetting">The type of setting to get.</typeparam>
        /// <param name="setting">The gotten setting if it exists; otherwise default(TSetting).</param>
        /// <returns>True if the setting exists; otherwise, false.</returns>
        public bool TryGet<TSetting>([MaybeNull] out TSetting setting)
        {
            var type = typeof(TSetting);
            if (_settings.TryGetValue(type, out Box? box) && 
                box!.TryGetValue<TSetting>(out setting))
            {
                return true;
            }
            else
            {
                setting = default!;
                return false;
            }
        }

        /// <summary>
        /// Gets an existing setting or a specified default.
        /// </summary>
        /// <typeparam name="TSetting">The type of setting to get.</typeparam>
        /// <param name="default">The setting to return if the setting type has not been set.</param>
        /// <returns>The existing setting if it was set; otherwise the specified <paramref name="default"/>.</returns>
        [return: MaybeNull]
        public TSetting GetOrDefault<TSetting>(TSetting @default)
        {
            var type = typeof(TSetting);
            if (_settings.TryGetValue(type, out Box? box) && box!.TryGetValue<TSetting>(out var existing))
            {
                return existing;
            }
            return @default;
        }

        /// <summary>
        /// Gets an existing setting or a creates a default.
        /// </summary>
        /// <typeparam name="TSetting">The type of setting to get.</typeparam>
        /// <param name="defaultFactory">The factory to create a default setting.</param>
        /// <returns>The existing setting if it was set; otherwise a created setting.</returns>
        /// <exception cref="ArgumentNullException">If the setting does not exist and <paramref name="defaultFactory"/> is null.</exception>
        [return: MaybeNull]
        public TSetting GetOrDefault<TSetting>(Func<TSetting> defaultFactory)
        {
            var type = typeof(TSetting);
            if (_settings.TryGetValue(type, out Box? box) && box!.TryGetValue<TSetting>(out var existing))
            {
                return existing;
            }
            return defaultFactory();
        }

        /// <inheritdoc />
        public override string ToString() => GetType().Name;
    }
}
*/
