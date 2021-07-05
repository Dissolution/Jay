using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Jay.IO
{
    /// <summary>
    /// Utility class for working with filesystem objects.
    /// </summary>
    public static class FileSystem
    {
        /// <summary>
        /// Uses the default filesystem explorer to open the specified <paramref name="file"/>.
        /// </summary>
        /// <param name="file">The file to open.</param>
        /// <returns>A <see cref="Result"/> describing if the <paramref name="file"/> was able to be opened.</returns>
        public static Result Execute(FileInfo? file)
        {
            if (file is null)
                return new ArgumentNullException(nameof(file));
            if (!file.Exists)
                return new FileNotFoundException("The given file does not exist", file.Name);
            var psi = new ProcessStartInfo
            {
                FileName = file.FullName,
                UseShellExecute = true,
                Verb = "open",
            };
            var process = Process.Start(psi);
            if (process is null)
                return new InvalidOperationException($"Could not execute '{file.FullName}'");
            return true;
        }

        /// <summary>
        /// Uses the default filesystem explorer to open the specified <paramref name="directory"/>.
        /// </summary>
        /// <param name="directory">The directory to open.</param>
        /// <returns>A <see cref="Result"/> describing if the <paramref name="directory"/> was able to be opened.</returns>
        public static Result Execute(DirectoryInfo? directory)
        {
            if (directory is null)
                return new ArgumentNullException(nameof(directory));
            if (!directory.Exists)
                return new DirectoryNotFoundException($"The given directory does not exist: '{directory.FullName}'");
            var psi = new ProcessStartInfo
            {
                FileName = directory.FullName,
                UseShellExecute = true,
                Verb = "open",
            };
            var process = Process.Start(psi);
            if (process is null)
                return new InvalidOperationException($"Could not execute '{directory.FullName}'");
            return true;
        }

        public static Result Execute(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return new ArgumentNullException(nameof(path));
            var psi = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "open",
            };
            var process = Process.Start(psi);
            if (process is null)
                return new InvalidOperationException($"Could not execute '{path}'");
            return true;
        }
    }
}
