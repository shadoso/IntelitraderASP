using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelitraderChallenge.Models
{
    public class CipherDownload
    {
        public string Filename { get; set; }
        public byte[] FileContent { get; set; }
        public string ContentType { get; set; }
    }
}