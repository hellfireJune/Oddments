using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HarmonyLib;

namespace Oddments
{
    internal class WhetStoneWaxStoneItem : PlayerItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(WhetStoneWaxStoneItem))
        {
            Name = "sharpenign stone",
        };

        public static OddItemTemplate template2 = new OddItemTemplate(typeof(WhetStoneWaxStoneItem))
        {
            Name = "cool gun waxer",
        };

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
        }

        public class SlightlyBoostComponent : MonoBehaviour
        {
            Gun gun;

            public void Start()
            {
                Gun gun = gameObject.GetComponent<Gun>();
                if (gun == null)
                {
                    Destroy(this);
                    return;
                }

                this.gun = gun;
            }

            public static List<string> powerLevel = new List<string>()
            {
                "Slightly ",
                "Moderately ",
                "Greatly ",
                "Extremely ",
            };

            public int WaxLevel = 0;

            public int SharpnessLevel = 0;
        }
    }
}
