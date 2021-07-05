 /*internal enum SettingsFileMissingBehavior
    {
        Throw = 0,
        CreateDefault = 1,
    }

    public enum SaveBehavior
    {
        Explicit = 0,
        OnPropertyChange = 1,
    }
    
    public interface ISettings : INotifyPropertyChanging, INotifyPropertyChanged
    {
        SaveBehavior SaveBehavior { get; set; }
        string Path { get; }
    }

    public interface ISettingsBase<TSettings> : ISettings
        where TSettings : ISettingsBase<TSettings>, new()
    {
        
    }
    
    
    internal class Settings<T> : INotifyPropertyChanging, INotifyPropertyChanged
        where T : Settings<T>, new()
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions;

        public static T Default
        {
            get
            {
                var name = typeof(T).Name;
                var path = Path.Combine(Environment.CurrentDirectory, $"{name}.xml");
                return Task.Run(async () => await LoadAsync(path, SettingsFileMissingBehavior.CreateDefault)).Result;
            }
        }
        
        static Settings()
        {
            _jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
            {
                WriteIndented = true, 
                AllowTrailingCommas = true
            };
        }

        public static async Task<T> LoadAsync(string path,
                                              SettingsFileMissingBehavior settingsFileMissingBehavior = SettingsFileMissingBehavior.Throw,
                                              CancellationToken token = default)
        {
            T value;
            if (File.Exists(path))
            {
                await using (var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    value = await JsonSerializer.DeserializeAsync<T>(fs, _jsonSerializerOptions, token);
                    if (value != null)
                    {
                        return value;
                    }
                }
            }
            
            if (settingsFileMissingBehavior == SettingsFileMissingBehavior.Throw)
                throw new FileNotFoundException("Could not load Settings File", path);
            value = new T();
            value.FillWithDefaults();
            await SaveAsync(path, value, token);
            return value;
        }

        public static async Task SaveAsync(string path,
                                           [DisallowNull] T value,
                                           CancellationToken token = default)
        {
            await using (var fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                await JsonSerializer.SerializeAsync(fs, value, _jsonSerializerOptions, token);
            }
        }
        
        
        /// <inheritdoc />
        public event PropertyChangingEventHandler? PropertyChanging;

        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;

        protected Settings()
        {
            
        }
        
        protected virtual void FillWithDefaults() { }

        protected void OnPropertyChange([CallerMemberName] string? propertyName = null)
        {
            
        }

      
    }*/