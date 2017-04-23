using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OcrCorrect
{
    public class ReplaceClient : IDisposable
    {

        public void Dispose()
        {
        }

        internal Task<string[]> ReplaceAsync(string[] lines)
        {
            var newLines = new List<string>();
            foreach(var line in lines)
            {
                newLines.Add(line.Replace(" ag ", " as "));
            }

            return Task.FromResult(newLines.ToArray());
        }
    }
}
