﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProjectDiary
{
    internal class DiaryTrie
    {
        [JsonPropertyName("r")]
        public DiaryTrieNode _root { get; set; }

        public DiaryTrie()
        {
            _root = new DiaryTrieNode('.', 0, string.Empty);
        }

        public void AddWord(string text, string source)
        {
            var node = _root;
            foreach (var chr in text)
            {
                node = node.SetChildCharacter(chr, source);
            }
        }

        public void AddWord(string text, int startIndex, string source)
        {
            var node = _root;
            for (var i = startIndex; i < text.Length; i++)
            {
                var chr = text[i];
                node = node.SetChildCharacter(chr, source);
            }
        }

        public bool HasWord(string text, out List<string> sources)
        {
            var node = _root;
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
}
