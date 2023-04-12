using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public class SpawnFriendlyGrenadeItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(SpawnFriendlyGrenadeItem))
        {
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnAnyEnemyReceivedDamage += OnReceivedDamage;
        }

        private void OnReceivedDamage(float arg1, bool arg2, HealthHaver arg3)
        {
            if (arg2 && arg3 && arg3.specRigidbody && arg3.aiActor && arg3.aiActor.CompanionOwner == null
                && arg3.gameObject.GetComponent<KillMeIfImAwake>() == null
                && arg3.aiActor.EnemyGuid != guid)
            {
                Vector3 pos = arg3.specRigidbody.UnitBottomCenter;
                RoomHandler room = arg3.aiActor.ParentRoom ?? GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(((Vector2)pos).ToIntVector2());
                SpawnDumbAssGrenadeGuy(pos, room, guid);
            }
        }

        public string guid { get { return "4d37ce3d666b4ddda8039929225b7ede"; } }

        public static void SpawnDumbAssGrenadeGuy(Vector3 position, RoomHandler room, string guid)
        {

            AIActor aiactor = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(guid), position, room, true, AIActor.AwakenAnimationType.Awaken, true);
            aiactor.gameObject.AddComponent<KillMeIfImAwake>();
            aiactor.IgnoreForRoomClear = true;
            aiactor.HandleReinforcementFallIntoRoom();
        }
    }

    internal class KillMeIfImAwake : BraveBehaviour
    {
        public static readonly float WaitToDie = 0.125f;
        public float elapsed;

        void Update()
        {
            if (aiActor == null)
            {
                Destroy(this);
                return;
            }
            if (aiActor.HasBeenAwoken && aiActor.healthHaver)
            {
                elapsed += BraveTime.DeltaTime;
                if (elapsed > WaitToDie)
                {
                    aiActor.healthHaver.ApplyDamage(float.MaxValue - 27616, Vector2.zero, "grnddmg. report to june if found");
                }
            }
        }
    }
}
