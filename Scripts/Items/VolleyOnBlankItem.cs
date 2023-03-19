using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.Misc;
using UnityEngine;

namespace Oddments
{
    public class VolleyOnBlankItem : BlankModificationItem
    { 
        public static ItemTemplatePlusVolley template = new ItemTemplatePlusVolley(typeof(VolleyOnBlankItem))
        {
            Name = "Sawblade Ammolet",

            Projectile = ((Gun)PickupObjectDatabase.GetById(341)).DefaultModule.projectiles[0],
            ProjectilesToFire = 8,
            IsRadial = true,
        };
        public static ItemTemplatePlusVolley template2 = new ItemTemplatePlusVolley(typeof(VolleyOnBlankItem))
        {
            Name = "Rocket Ouroboros",

            Projectile = ((Gun)PickupObjectDatabase.GetById(129)).DefaultModule.projectiles[0],
            ProjectilesToFire = 3,
            PreAddModuleAction = module =>
            {
                module.angleVariance = 360f;
            },
            PostInitAction = item =>
            {
                VolleyOnBlankItem voleyItem = (VolleyOnBlankItem)item;
                voleyItem.IsRocketOuroboros = true;
            }
        };
        public static ItemTemplatePlusVolley template3 = new ItemTemplatePlusVolley(typeof(VolleyOnBlankItem))
        {
            Name = "Yari Ammolet",

            Projectile = ((Gun)PickupObjectDatabase.GetById(16)).DefaultModule.projectiles[0],
            ProjectilesToFire = 16,
            IsRadial = true,
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.GetExtComp().OnBlankModificationItemProcessed += FireOnBlank;
        }
        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            player.GetExtComp().OnBlankModificationItemProcessed -= FireOnBlank;
        }

        private void FireOnBlank(PlayerController arg1, SilencerInstance arg2, Vector2 arg3, BlankModificationItem arg4)
        {
            if (arg4 == this)
            {
                ProjectileVolleyData volley = JuneLib.RegeneratingVolleyModifiers.OnTheGoModifyVolley(ThingToFire, arg1);
                VolleyUtility.FireVolley(volley, arg3, Vector2.zero, arg1);
            }
        }

        public bool IsRocketOuroboros = false;
        public ProjectileVolleyData ThingToFire;
    }
}
