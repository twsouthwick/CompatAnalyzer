﻿using System;
using System.IO;

namespace CompatibilityAnalyzer
{
    public class FileSystemFile : IFile
    {
        public FileSystemFile(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));

            if (!File.Exists(Path))
            {
                throw new FileNotFoundException("Could not find file", path);
            }
        }

        public string Path { get; }

        public Stream OpenRead() => File.OpenRead(Path);

        public override string ToString() => Path;

        public override bool Equals(object obj)
        {
            if (obj is FileSystemFile other)
            {
                return string.Equals(Path, other.Path, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(Path);
        }
    }
}
