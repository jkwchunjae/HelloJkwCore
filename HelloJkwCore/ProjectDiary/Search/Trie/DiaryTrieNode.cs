using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProjectDiary
{
    internal class DiaryTrieNode
    {
        [JsonPropertyName("c")]
        public char Character { get; private set; }
        [JsonPropertyName("d")]
        public int Depth { get; private set; }
        [JsonPropertyName("n")]
        public List<DiaryTrieNode> Children { get; }
        [JsonPropertyName("s")]
        public List<string> SourceList { get; }

        public DiaryTrieNode(char character, int depth, string source)
        {
            Character = character;
            Depth = depth;
            Children = new List<DiaryTrieNode>();
            SourceList = new List<string>();
        }

        public DiaryTrieNode SetChildCharacter(char character, string source)
        {
            var child = Children.FirstOrDefault(x => x.Character == character);
            if (child == null)
            {
                child = new DiaryTrieNode(character, Depth + 1, source);
                Children.Add(child);
            }

            child?.AddSource(source);
            return child;
        }

        public DiaryTrieNode GetChild(char character)
        {
            var child = Children.FirstOrDefault(x => x.Character == character);
            return child;
        }

        private void AddSource(string source)
        {
            if (Depth >= DiaryTrieOption.SourceSaveDepth)
            {
                if (!SourceList.Contains(source))
                {
                    SourceList.Add(source);
                }
            }
        }

    }
}
