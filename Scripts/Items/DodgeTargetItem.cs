using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public class DodgeTargetItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(DodgeTargetItem))
        {
            Name = "Dodge Target",
            Description = "The Best Defense",
            LongDescription = "Damaging an enemy has a chance to clear all bullets spawned by that enemy. Only works once per enemy.",
            SpriteResource = $"{Module.SPRITE_PATH}/dodgetarget.png",
            Quality = ItemQuality.B,
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnAnyEnemyReceivedDamage += KilledFoe;
        }

        float procChance = 0.1f;
        private void KilledFoe(float dmg, bool fatal, HealthHaver arg2)
        {
            if (!arg2 || !arg2.aiActor || !arg2.specRigidbody)
            {
                return;
            }
            if (arg2.gameObject.GetComponent<DodgeTargetMarker>())
            {
                return;
            }

            if (!fatal && procChance > UnityEngine.Random.value)
            {
                return;
            }
            arg2.gameObject.AddComponent<DodgeTargetMarker>();

            List<Projectile> projectiles = StaticReferenceManager.AllProjectiles.Where(projectile => projectile.Owner == arg2.gameActor).ToList();
            StartCoroutine(ClearAllBullets(projectiles, arg2.specRigidbody.UnitCenter));
        }

        float speed = 30;
        public IEnumerator ClearAllBullets(List<Projectile> projectiles, Vector2 pos)
        {
            float elapsed = 0;
            while (projectiles.Count > 0)
            {
                elapsed += BraveTime.DeltaTime;

                for (int i = 0; i < projectiles.Count; i++)
                {
                    Projectile proj = projectiles[i];
                    if (proj == null)
                    {
                        projectiles.RemoveAt(i);
                        i--;
                        continue;
                    }
                    float distance = Vector2.Distance(pos, proj.transform.PositionVector2());

                    if (distance < elapsed*speed)
                    {
                        proj.DieInAir(false, false, false, true);
                        projectiles.Remove(proj);
                        i--;
                    }
                }
                yield return null;
            }
            yield break;
        }

        private class DodgeTargetMarker : MonoBehaviour { }
    }
}
