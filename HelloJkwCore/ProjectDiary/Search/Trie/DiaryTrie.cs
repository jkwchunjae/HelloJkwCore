using System.Text.Json.Serialization;

namespace ProjectDiary;

internal class DiaryTrie
{
    [JsonPropertyName("r")]
    public DiaryTrieNode Root { get; init; }

    [JsonConstructor]
    public DiaryTrie(DiaryTrieNode root)
    {
        Root = root;
    }

    public DiaryTrie()
    {
        Root = new DiaryTrieNode('.', 0);
    }

    public void AddWord(string text, string source)
    {
        var node = Root;
        foreach (var chr in text)
        {
            node = node.SetChildCharacter(chr, source);
        }
    }

    public void AddWord(string text, int startIndex, string source)
    {
        var node = Root;
        for (var i = startIndex; i < text.Length; i++)
        {
            var chr = text[i];
            node = node.SetChildCharacter(chr, source);
        }
    }

    public bool HasWord(string text, out List<string> sources)
    {
        var node = Root;
        foreach (var chr in text)
        {
            node = node.GetChild(chr);
            if (node == null)
                break;
        }

        if (node == null)
        {
            sources = null;
            return false;
        }
        else
        {
            sources = node.SourceList;
            return true;
        }
    }
}