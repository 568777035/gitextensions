﻿using System;
using System.Xml.Serialization;

namespace GitCommands.Repository
{
    public class Repository
    {
        public enum RepositoryAnchor
        {
            MostRecent,
            LessRecent,
            None
        }


        public Repository()
        {
            Anchor = RepositoryAnchor.None;
        }

        public Repository(string path, string description, string title)
            : this()
        {
            Path = path;
            Description = description;
            Title = title;
            RepositoryType = RepositoryType.Repository;
        }

        public string Title { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public RepositoryAnchor Anchor { get; set; }

        [XmlIgnore]
        public bool IsRemote
        {
            get { return PathIsUrl(Path); }
        }

        [XmlIgnore]
        public RepositoryType RepositoryType { get; set; }

        public void Assign(Repository source)
        {
            if (source == null)
                return;
            Path = source.Path;
            Title = source.Title;
            Description = source.Description;
            RepositoryType = source.RepositoryType;
        }

        public override string  ToString()
        {
            return Path + " ("+Anchor+")";
        }

        public static bool PathIsUrl(string path)
        {
            return !String.IsNullOrEmpty(path) &&
                (path.StartsWith("http", StringComparison.CurrentCultureIgnoreCase) ||
                 path.StartsWith("git", StringComparison.CurrentCultureIgnoreCase) ||
                 path.StartsWith("ssh", StringComparison.CurrentCultureIgnoreCase));
        }
    }
}