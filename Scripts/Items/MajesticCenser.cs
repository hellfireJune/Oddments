using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public class MajesticCenser : PlayerItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(MajesticCenser))
        {
            Name = "Majestic Censer",
            Quality = ItemQuality.EXCLUDED,
        };
    }
}
