using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OcrCorrect
{
    public class ReplaceClient : IDisposable
    {
        static Dictionary<string, string> ReplaceWords = new Dictionary<string, string>
        {
            ["ag"] = "as",
            ["•rag"] = "was",
            ["IEof:.her'3"] = "Mother's",
            ["Y,others"] = "Mothers",
            ["I'lat"] = "Nat",
            ["Tlirntte"] = "Minute",
            ["07rerved"] = "observed",
            ["Vintlt,eg"] = "Minutes",
            ["ps•evious"] = "previous",
            ["Å1n"] = "Ann",
            ["gat•riet"] = "Harriet",
            ["Liax"] = "Max",
            ["o?"] = "of",
            ["•rill"] = "will",
            ["Ylappy"] = "Happy",
            ["Toulg"] = "Louis",
            ["Saxn"] = "Sam",
            ["wæ•e"] = "were",
            ["ftcre"] = "There",
            ["møe+,ing"] = "meeting",
            ["Suc€estion"] = "Suggestion",
            ["vas"] = "was",
        };

        public void Dispose()
        {
        }

        internal Task<string[]> ReplaceAsync(string[] lines)
        {
            var newLines = new List<string>();
            for(var i=0; i < lines.Length; i++)
            {
                var newLine = new List<string>();
                foreach (var word in lines[i].Split(' '))
                {
                    if(ReplaceWords.Keys.Contains(word))
                    {
                        newLine.Add(ReplaceWords[word]);
                    } else
                    {
                        newLine.Add(word);
                    }
                }

                newLines.Add(string.Join(" ", newLine));
            }

            return Task.FromResult(newLines.ToArray());
        }
    }
}
