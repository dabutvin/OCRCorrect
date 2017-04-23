using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OcrCorrect.Models;

namespace OcrCorrect.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upload(IFormFile file, string ocrKey, string spellCheckKey)
        {
            if (file?.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Seek(0, 0);

                    using (var ocrClient = new OcrClient(stream.ToArray(), ocrKey))
                    {
                        var lines = await ocrClient.GetLinesAsync(file);

                        var checkedLines = new List<string>();

                        using (var spellCheckClient = new SpellCheckClient(spellCheckKey))
                        {
                            for(var i=0; i<lines.Length; i++)
                            {
                                var preLines = Enumerable.Range(0, Math.Max(i - 1, 0))
                                    .SelectMany(x => lines[x])
                                    .ToArray();

                                var postLines = Enumerable.Range(Math.Min(i + 1, lines.Length - 1), lines.Length - i - 1)
                                    .SelectMany(x => lines[x])
                                    .ToArray();

                                checkedLines.Add(
                                    await spellCheckClient.CorrectAsync(
                                        string.Join(" ", lines[i]),
                                        string.Join(" ", preLines),
                                        string.Join(" ", postLines)));
                            }
                        }

                        var model = new RenderedModel
                        {
                            Lines = lines.Select(x => string.Join(" ", x)).ToArray(),
                            CorrectedLines = checkedLines.ToArray(),
                        };

                        return View(model);
                    }
                }
            }
            else
            {
                return NoContent();
            }
        }
        
        public IActionResult Error()
        {
            return View();
        }
    }
}
