using HarmonyLib;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public class BottledFairyItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(BottledFairyItem))
        {
            Name = "Tissue Sample",
        };

        public override void Pickup(PlayerController player)
        {
            if (!m_pickedUpThisRun)
            {
                EasyFullHeal(player);
            }
            player.OnNewFloorLoaded += LoadNewFloor;
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            player.OnNewFloorLoaded -= LoadNewFloor;
        }

        public override void Update()
        {
            base.Update();
            if (!GameManager.Instance.IsLoadingLevel && Owner && Owner.AcceptingAnyInput && queueHeal)
            {
                queueHeal = false;
                Owner.StartCoroutine(HealCoroutine(Owner));
            }
        }



        bool queueHeal = false;

        public float wait = 1.5f;
        public IEnumerator HealCoroutine(PlayerController player)
        {
            yield return new WaitForSeconds(wait);
            EasyFullHeal(player);
            yield break;
        }

        public static void EasyFullHeal(PlayerController player)
        {
            player.healthHaver.FullHeal();
            AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", player.gameObject);
            player.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/vfx_healing_sparkles_001") as GameObject, Vector3.zero);
        }

        private void LoadNewFloor(PlayerController obj)
        {
            queueHeal = true;
        }
    }
}
