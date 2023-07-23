using System.Text.Json.Serialization;

namespace ProjectDiary;

internal class DiaryTrieNode
{
    [JsonPropertyName("c")]
    public char Character { get; init; }
    [JsonPropertyName("d")]
    public int Depth { get; init; }
    [JsonPropertyName("n")]
    public List<DiaryTrieNode> Children { get; init; }
    [JsonPropertyName("s")]
    public List<string> SourceList { get; init; }

    [JsonConstructor]
    public DiaryTrieNode(char character, int depth, List<DiaryTrieNode> children, List<string> sourceList)
    {
        Character = character;
        Depth = depth;
        Children = children;
        SourceList = sourceList;
    }

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