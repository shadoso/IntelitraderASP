using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelitraderChallenge.Models
{
    public class CipherFile
    {
        public int Displacement { get; set; }
        public HttpPostedFileBase File { get; set; }

    }
}