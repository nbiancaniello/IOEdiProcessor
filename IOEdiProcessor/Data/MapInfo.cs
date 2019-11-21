using System;
using System.Collections.Generic;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class MapInfo
    {
        public Header Header { get; set; }
        public Dictionary<string, int> Position { get; set; }

        public MapInfo()
        {
        }
    }
}
