using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Gungeon;
using UnityEngine;
using JuneLib.Items;

namespace Oddments
{
    public class SpidAR : GunBehaviour
    {
        public static GunTemplate gunTemplate = new GunTemplate(typeof(SpidAR))
        {
            Name = "Spid-AR",
            PostInitAction = gun =>
            {

            }
        };
    }
}
