using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDiary;

internal class DiaryTrieNode
{
    [JsonProperty("c")]
    public char Character { get; private set; }
    [JsonProperty("d")]
    public int Depth { get; private set; }
    [JsonProperty("n")]
    public List<DiaryTrieNode> Children { get; }
    [JsonProperty("s")]
    public List<string> SourceList { get; }

    public DiaryTrieNode(char character, int depth)
    {
        Character = character;
        Depth = depth;
        Children = new List<DiaryTrieNode>();
        SourceList = new List<string>();
    }

    public DiaryTrieNode SetChildCharacter(char character, string source)
    {
        var child = GetChild(character);
        if (child == null)
        {
            child = new DiaryTrieNode(character, Depth + 1);
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