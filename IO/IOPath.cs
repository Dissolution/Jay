using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Jay.Collections;
using Jay.Comparison;
using Jay.Exceptions;
using Jay.Text;

namespace Jay.IO
{
    public readonly struct IOPath : IEquatable<IOPath>,
                                  IEquatable<FileSystemInfo>,
                                  IEquatable<string>,
                                  IReadOnlyList<string>, IReadOnlyCollection<string>, IEnumerable<string>
    {
        public static implicit operator IOPath(string? path) => new IOPath(path);
        public static implicit operator IOPath(FileSystemInfo? fileSystemInfo) => new IOPath(fileSystemInfo);
        public static implicit operator string(IOPath path) => path.ToString();

        public static bool operator ==(IOPath ioPath, IOPath path) => _partsComparer.Equals(ioPath._parts, path._parts);
        public static bool operator !=(IOPath ioPath, IOPath path) => !_partsComparer.Equals(ioPath._parts, path._parts);
        public static bool operator ==(IOPath ioPath, string? path) => _partsComparer.Equals(ioPath._parts, Parse(path));
        public static bool operator !=(IOPath ioPath, string? path) => !_partsComparer.Equals(ioPath._parts, Parse(path));

        public static IOPath operator /(IOPath ioPath, IOPath path) => ioPath.Append(path);
        public static IOPath operator +(IOPath ioPath, IOPath path) => ioPath.Append(path);
        public static IOPath operator /(IOPath ioPath, string? path) => ioPath.Append(path);
        public static IOPath operator +(IOPath ioPath, string? path) => ioPath.Append(path);

        private static readonly char _directorySeparator;
        private static readonly EnumerableEqualityComparer<string> _partsComparer;

        public static readonly IOPath Empty = new IOPath();
        
        static IOPath()
        {
            _directorySeparator = Path.DirectorySeparatorChar;
            _partsComparer = new EnumerableEqualityComparer<string>(StringComparer.OrdinalIgnoreCase);
        }
        
        private static IEnumerable<string> Parse(string? path)
        {
            if (path is null) yield break;
            int p = 0;
            int len = path.Length;
            int start;
            while (true)
            {
                // Consume whitespace
                while (p < len && char.IsWhiteSpace((path[p])))
                {
                    p++;
                }
                if (p >= len)
                {
                    // Nothing left to append
                    yield break;
                }
                
                var c = path[p];
                // Check for directory separator
                if (c == _directorySeparator)
                {
                    // Empty part
                    p++;
                    continue;
                }
                
                // Starting character is at this position
                start = p;
                p++;
                
                // Consume non separator, non-whitespace characters
                while (p < len && (!char.IsWhiteSpace(c = path[p]) && c != _directorySeparator))
                {
                    p++;
                }
                // if (p >= len)
                // {
                //     // Nothing left to append
                //     yield break;
                // }
                
                // Did we find anything?
                if (p - start == 0)
                {
                    // Empty part
                    p++;
                    continue;
                }
                
                // This is a part
                var part = path.Slice(start, p - start);
                yield return new string(part);
            }
        }

        private readonly string[]? _parts;

        public int Count
        {
            get
            {
                if (_parts is null) return 0;
                return _parts.Length;
            }
        }

        public string this[int index]
        {
            get
            {
                if (_parts is null || ((uint) index >= (uint) _parts.Length))
                    throw new IndexOutOfRangeException();
                return _parts[index];
            }
        }

        public bool Exists
        {
            get
            {
                if (Count > 0)
                {
                    if (IsFile(out var file))
                        return file.Exists;
                    if (IsDirectory(out var directory))
                        return directory.Exists;
                }
                return false;
            }
        }

        public IOPath Parent
        {
            get
            {
                if (_parts is null || _parts.Length <= 1)
                    return Empty;
                return new IOPath(_parts.Slice(..^1).ToArray());
            }
        }

        private IOPath(string[] parts)
        {
            _parts = parts;
        }
        
        public IOPath(string? path)
        {
            _parts = Parse(path).ToArray();
        }

        public IOPath(FileSystemInfo? fileSystemInfo)
        {
            _parts = Parse(fileSystemInfo?.FullName).ToArray();
        }
        
        public bool IsFile()
        {
            if (_parts.TryGet(^1, out var last))
            {
                for (var i = last!.Length - 1; i >= 0; i--)
                {
                    if (last[i] == '.')
                        return true;
                }
            }
            return false;
        }
        
        public bool IsFile([NotNullWhen(true)] out FileInfo? fileInfo)
        {
            if (_parts.TryGet(^1, out var last))
            {
                for (var i = last!.Length - 1; i >= 0; i--)
                {
                    if (last[i] == '.')
                    {
                        fileInfo = new FileInfo(ToString());
                        return true;
                    }
                }
            }
            fileInfo = null;
            return false;
        }

        public bool IsDirectory()
        {
            if (_parts is null || _parts.Length == 0)
            {
                return false;
            }

            string last = _parts[^1];
            for (var i = last.Length - 1; i >= 0; i--)
            {
                if (last[i] == '.')
                {
                    return false;
                }
            }
            
            return true;
        }
        
        public bool IsDirectory([NotNullWhen(true)] out DirectoryInfo? directoryInfo)
        {
            if (_parts is null || _parts.Length == 0)
            {
                directoryInfo = null;
                return false;
            }

            string last = _parts[^1];
            for (var i = last.Length - 1; i >= 0; i--)
            {
                if (last[i] == '.')
                {
                    directoryInfo = null;
                    return false;
                }
            }
            
            directoryInfo = new DirectoryInfo(ToString());
            return true;
        }

        public IOPath Append(IOPath path)
        {
            if (Count > 0)
            {
                if (path.Count > 0)
                {
                    var newParts = new string[Count + path.Count];
                    _parts!.CopyTo(newParts, 0);
                    path._parts!.CopyTo(newParts, Count);
                    return new IOPath(newParts);
                }
                else
                {
                    return this;
                }
            }
            else
            {
                if (path.Count > 0)
                {
                    return path;
                }
                else
                {
                    return Empty;
                }
            }
        }
        
        public IOPath Append(string? path)
        {
            if (Count > 0)
            {
                return new IOPath(_parts!.Concat(Parse(path)).ToArray());
            }
            else
            {
                return new IOPath(Parse(path).ToArray());
            }
        }
        
        public IOPath Append(params string?[] paths)
        {
            var parts = new List<string>();
            if (Count > 0)
                parts.AddRange(_parts!);
            foreach (var path in paths)
                parts.AddRange(Parse(path));
            return new IOPath(parts.ToArray());
        }
        
        /// <inheritdoc />
        public bool Equals(IOPath ioPath)
        {
            return _partsComparer.Equals(_parts, ioPath._parts);
        }

        /// <inheritdoc />
        public bool Equals(FileSystemInfo? fileSystemInfo)
        {
            return _partsComparer.Equals(_parts, Parse(fileSystemInfo?.FullName));
        }

        /// <inheritdoc />
        public bool Equals(string? path)
        {
            return _partsComparer.Equals(_parts, Parse(path));
        }
        
        /// <inheritdoc />
        public IEnumerator<string> GetEnumerator() => _parts.GetEnumerator<string>();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetHashCodeException.Throw<IOPath>(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return TextBuilder.Build(_parts, (builder, parts) => builder.AppendDelimit(_directorySeparator, parts));
        }
    }
}