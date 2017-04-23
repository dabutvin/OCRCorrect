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
                    using (var replaceClient = new ReplaceClient())
                    using (var spellCheckClient = new SpellCheckClient(spellCheckKey))
                    {
                        var lines = await ocrClient.GetLinesAsync(file);
                        var replacedLines = await replaceClient.ReplaceAsync(lines);
                        var checkedLines = await spellCheckClient.CorrectAsync(replacedLines);

                        var model = new RenderedModel
                        {
                            Lines = lines,
                            CorrectedLines = checkedLines,
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
