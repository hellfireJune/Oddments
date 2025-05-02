using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public class BulletOnTheCobItem : OddProjectileModifierItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(BulletOnTheCobItem))
        {
            Name = "Bullet on the Cob",
        };
    }
}
