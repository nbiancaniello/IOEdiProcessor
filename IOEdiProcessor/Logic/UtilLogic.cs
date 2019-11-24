using IOEdiProcessor.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOEdiProcessor.Logic
{
    public abstract class UtilLogic
    {
        private readonly static Dictionary<string, string> _parse = new Dictionary<string, string>
        {
            { "á","a" },
            { "é","e" },
            { "í","i" },
            { "ó","o" },
            { "ú","u" },
            { "ñ","n" },
            { "¼", " 1/4 "},
            { "½", " 2/4 "},
            { "¾", " 3/4 "}
        };
        public static string ParseText(string str)
        {
            foreach (KeyValuePair<string, string> entry in _parse)
            {
                str = str.Replace(entry.Key, entry.Value);
            }
            return str;
        }
    }
}
