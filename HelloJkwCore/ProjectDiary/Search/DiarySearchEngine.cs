namespace ProjectDiary;

class DiarySearchEngine
{
    private static char[] seperator = new[] { ' ', '\n' };
    private DiaryTrie _trie { get; set; } = new DiaryTrie();

    private ReaderWriterLockSlim _lock = new();

    public void AddText(string text, string source)
    {
        var words = text
            .Replace('\r', ' ')
            .Replace('.', ' ')
            .Replace(',', ' ')
            .Replace('\'', ' ')
            .Replace('"', ' ')
            .Replace('!', ' ')
            .Replace('~', ' ')
            .Replace('^', ' ')
            .Replace('(', ' ')
            .Replace(')', ' ')
            .Replace('-', ' ')
            .Replace('+', ' ')
            .Replace('_', ' ')
            .Replace('=', ' ')
            .Replace('@', ' ')
            .Replace('#', ' ')
            .Replace('$', ' ')
            //.Replace('%', ' ')
            .Replace('&', ' ')
            .Replace('*', ' ')
            .Replace('{', ' ')
            .Replace('}', ' ')
            .Replace('[', ' ')
            .Replace(']', ' ')
            .Replace(':', ' ')
            .Replace(';', ' ')
            .Replace('<', ' ')
            .Replace('>', ' ')
            .Replace('?', ' ')
            .Split(DiarySearchEngine.seperator)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        using (_lock.AcquireWriterLock())
        {
            foreach (var word in words)
            {
                for (var i = 0; i < word.Length; i++)
                {
                    _trie.AddWord(word, i, source);
                }
            }
        }
    }

    public List<string> Search(string text)
    {
        using (_lock.AcquireReaderLock())
        {
            if (_trie.HasWord(text, out var source))
            {
                return source;
            }
            return null;
        }
    }

    public void SetTrie(DiaryTrie trie)
    {
        using (_lock.AcquireWriterLock())
        {
            _trie = trie;
        }
    }

    public string GetTrieJson()
    {
        using (_lock.AcquireReaderLock())
        {
            return Json.SerializeNoIndent(_trie);
        }
    }
}