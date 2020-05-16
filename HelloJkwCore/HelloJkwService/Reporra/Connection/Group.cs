using System;

namespace HelloJkwService.Reporra
{
    public class Group
    {
        public static readonly Group None = new Group("None");
        public static readonly Group Lobby = new Group("Lobby");

        public string Name { get; }

        public Group(string groupName)
        {
            Name = groupName;
        }

        public override bool Equals(object obj)
        {
            if (obj is Group groupName)
            {
                return Name == groupName.Name;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
