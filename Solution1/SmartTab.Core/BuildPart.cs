using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTab.Core
{
    public class BuildPart
    {
        public int Id { get; set; }
        public int Quantity { get; set; } = 1;
        public int PcId { get; set; }
        public Product Pc { get; set; } = null!;

        public int ComponentId { get; set; }
        public Product Component { get; set; } = null!;
    }
}
