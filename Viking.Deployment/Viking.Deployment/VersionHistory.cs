using System;
using System.Collections.Generic;
using System.IO;

namespace Viking.Deployment
{
    public class VersionHistory
    {
        private string Filename { get; }
        private Dictionary<string, Version> LastVersions { get; } = new Dictionary<string, Version>();

        public VersionHistory(string backingFile)
        {
            Filename = backingFile;
            ReadFile();
        }

        public Version GetLatestVersion(string tag) => LastVersions.ContainsKey(tag) ? LastVersions[tag] : new Version();

        public void SetVersion(string tag, Version version)
        {
            File.AppendText($"{tag}: {version.ToString()}" + Environment.NewLine);
            LastVersions[tag] = version;
        }

        private void ReadFile()
        {
            foreach(var line in File.ReadAllLines(Filename))
            {
                var split = line.Split(':');
                if (split.Length != 2)
                    continue;
                LastVersions[split[0]] = new Version(split[1]);
            }
        }
    }
}
