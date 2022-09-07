using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public class VoidFieldsTest
    {
        public static void Init()
        {
            ETGMod.AIActor.OnPostStart += OnEnemyStart;
            Hook hook = new Hook(typeof(PlayerController).GetMethod("AcquirePassiveItem", BindingFlags.Instance | BindingFlags.Public), typeof(VoidFieldsTest).GetMethod("PassiveItemEdit"));
        }

        public static void OnEnemyStart(AIActor actor)
        {
            EnemyPlayerController enemyPlayer = actor.gameObject.AddComponent<EnemyPlayerController>();
            EncounterTrackable.SuppressNextNotification = true;
            LootEngine.GivePrefabToPlayer(PickupObjectDatabase.GetById(353).gameObject, enemyPlayer.fakePlayer);
        }

        public class EnemyPlayerController : BraveBehaviour
        {
            public PlayerController fakePlayer = new PlayerController();

        }

        public static void PassiveItemEdit(PlayerController self, PassiveItem item)
        {

            AkSoundEngine.PostEvent("Play_OBJ_passive_get_01", self.gameObject);
            self.passiveItems.Add(item);
            item.transform.parent = self.GunPivot;
            item.transform.localPosition = Vector3.zero;
            item.renderer.enabled = false;
            if (item.GetComponent<DebrisObject>() != null)
            {
                UnityEngine.Object.Destroy(item.GetComponent<DebrisObject>());
            }
            if (item.GetComponent<SquishyBounceWiggler>() != null)
            {
                UnityEngine.Object.Destroy(item.GetComponent<SquishyBounceWiggler>());
            }
            GameUIRoot.Instance.AddPassiveItemToDock(item, self);
            self.stats.RecalculateStats(self, false, false);
        }
    }
}
