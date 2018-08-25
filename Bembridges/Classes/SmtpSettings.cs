using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bembridges.Classes
{
    public class SmtpSettings
    {
        public string Server { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string From { get; set; }
    }
}
