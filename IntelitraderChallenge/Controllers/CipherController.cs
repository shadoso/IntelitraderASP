using Antlr.Runtime.Tree;
using IntelitraderChallenge.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using static System.Net.Mime.MediaTypeNames;

namespace IntelitraderChallenge.Controllers
{
    public class CipherController : Controller
    {
        private static readonly int DEFAULT_DISPLACEMENT = 4;
        private static readonly int NEGATIVE_INDEX = 0;
        private static readonly int REVERSE = -1;
        private static readonly List<string> CHARACTERS = new List<string>
{
    "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S",
    "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l",
    "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "À", "Á", "Â", "Ã", "Ä",
    "Ç", "È", "É", "Ê", "Ë", "Ì", "Í", "Î", "Ï", "Ò", "Ó", "Ô", "Õ", "Ö", "Ù", "Ú", "Û", "Ü", "à",
    "á", "â", "ã", "ä", "ç", "è", "é", "ê", "ë", "ì", "í", "î", "ï", "ò", "ó", "ô", "õ", "ö", "ù",
    "ú", "û", "ü"
};
        private static readonly int COUNT = CHARACTERS.Count;

        private const int EncodeAction = 1;
        private const int DecodeAction = 0;



        public string DecodeFile(string text, int cipherIndex)
        {
            string newText = "";
            int letterIndex;

            foreach (char letter in text)
            {
                if (CHARACTERS.Contains(letter.ToString()))
                {
                    letterIndex = CHARACTERS.IndexOf(letter.ToString());
                    letterIndex = (letterIndex + cipherIndex * REVERSE) % COUNT;

                    if (letterIndex < NEGATIVE_INDEX)
                    {
                        letterIndex = COUNT + letterIndex;
                    }

                    newText += CHARACTERS[letterIndex];

                }
                else
                {
                    newText += letter;
                }
            }

            return newText;
        }

        public string EncodeFile(string text, int cipherIndex)
        {
            string newText = "";
            int letterIndex;

            foreach (char letter in text)
            {
                if (CHARACTERS.Contains(letter.ToString()))
                {
                    letterIndex = CHARACTERS.IndexOf(letter.ToString());
                    letterIndex = (letterIndex + cipherIndex) % COUNT;

                    if (letterIndex < NEGATIVE_INDEX)
                    {
                        letterIndex = COUNT + letterIndex;
                    }

                    newText += CHARACTERS[letterIndex];

                }
                else
                {
                    newText += letter;
                }
            }

            return newText;
        }

        // GET: Cipher
        public ActionResult Index()
        {
            string validStrings = "";
            foreach (string letter in CHARACTERS)
            {
                validStrings += $"{letter} ";
            }

            ViewBag.Cipher = validStrings;

            var cipherFile = new CipherFile
            {
                Displacement = DEFAULT_DISPLACEMENT
            };

            return View(cipherFile);
        }

        [HttpPost]
        public ActionResult Download(CipherFile cipherFile, int action)
        {
            var cipherDownload = new CipherDownload
            {
                Filename = Path.GetFileNameWithoutExtension(cipherFile.File.FileName),
                FileContent = null,
                ContentType = cipherFile.File.ContentType,
            };

            string text;

            using (var streamReader = new StreamReader(cipherFile.File.InputStream))
            {
                text = streamReader.ReadToEnd();
            }

            switch (action)
            {
                case EncodeAction:
                    text = EncodeFile(text, cipherFile.Displacement);
                    cipherDownload.FileContent = Encoding.UTF8.GetBytes(text);
                    cipherDownload.Filename += "Encoded" + cipherFile.Displacement.ToString() + Path.GetExtension(cipherFile.File.FileName);

                    ViewBag.DownloadAction = "Encoded";
                    break;

                case DecodeAction:
                    text = DecodeFile(text, cipherFile.Displacement);
                    cipherDownload.FileContent = Encoding.UTF8.GetBytes(text);
                    cipherDownload.Filename += "Decoded" + cipherFile.Displacement.ToString() + Path.GetExtension(cipherFile.File.FileName);

                    ViewBag.DownloadAction = "Decoded";
                    break;
            }

            return File(cipherDownload.FileContent, cipherDownload.ContentType, cipherDownload.Filename);
        }
    }
}