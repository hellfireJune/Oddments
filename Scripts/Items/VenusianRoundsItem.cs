using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public class VenusianRoundsItem : PassiveItem
    {
        public static GameObject fireTrail;
             
        public static OddItemTemplate template = new OddItemTemplate(typeof(VenusianRoundsItem))
        {
            PostInitAction = item =>
            {
                Projectile proj = ((Gun) PickupObjectDatabase.GetById(621)).DefaultModule.projectiles[0];
                
            }
        };
    }
}
