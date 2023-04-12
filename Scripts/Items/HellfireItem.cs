using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using UnityEngine;

namespace Oddments
{
    public class HellfireItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(HellfireItem))
        {
            Name = "Shellfire",
            Description = "Satan's Siren",
            PostInitAction = item =>
            {
                ShellfireExplosionBehaviour.Init();
            }
        };
        public override void Pickup(PlayerController player)
        {
            ETGMod.AIActor.OnPostStart += OnPostStart;
            player.gameObject.AddComponent<ShellfireExplosionBehaviour>();
            base.Pickup(player);
        }

        private void OnPostStart(AIActor obj)
        {
            if (obj && obj.healthHaver && obj.IsNormalEnemy && obj.CompanionOwner == null)
            {
                if (UnityEngine.Random.value < 1f)
                {
                    obj.healthHaver.minimumHealth = Math.Max(obj.healthHaver.minimumHealth, 0.01f);
                    ShellfireExplosionBehaviour shf = obj.gameObject.AddComponent<ShellfireExplosionBehaviour>();
                    shf.player = Owner;
                }
            }
        }

        public class ShellfireExplosionBehaviour : BraveBehaviour
        {
            public static void Init()
            {
                Projectile projectile = Instantiate((PickupObjectDatabase.GetById(86) as Gun).DefaultModule.projectiles[0]);
                List<string> animationPaths = new List<string>() { $"{Module.ASSEMBLY_NAME}/Resources/example_item_sprite.png" };
                BasicBeamController beam = projectile.GenerateBeamPrefab(animationPaths[0], new Vector2(12, 8), new Vector2(0, 0), animationPaths);
                beam.boneType = BasicBeamController.BeamBoneType.Straight;
                beam.interpolateStretchedBones = true;
                beam.ContinueBeamArtToWall = true;
                projectile.baseData.damage = 10f;
                projectile.baseData.speed = 25f;

                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                BeamOfHellfire = projectile;
            }
            public static Projectile BeamOfHellfire;

            public void FixedUpdate()
            {
                if (!healthHaver)
                {
                    return;
                }
                healthHaver.minimumHealth = Math.Max(healthHaver.minimumHealth, 0.01f);
                if (healthHaver.GetCurrentHealth() <= 0.011f)
                {
                    StartCoroutine(EnemyHell());
                }
            }

            public float DurInSec = 0.75f;
            public float blink = 0.2f;
            public float blinkDuration = 0.15f;
            public PlayerController player;

            private Color colorA = new Color(1, 0, 0);
            private Color colorB = new Color(1, 1, 1);

            public IEnumerator EnemyHell()
            {
                float elapsed = 0;
                float blinkElapsed = 0;
                while (elapsed < DurInSec)
                {
                    blinkElapsed += BraveTime.DeltaTime;
                    elapsed += BraveTime.DeltaTime;

                    if (blinkElapsed > blink)
                    {
                        AkSoundEngine.PostEvent("Play_OBJ_mine_beep_01", gameObject);
                        blinkElapsed -= blink;
                    }
                    float lerp = Math.Max(0, blinkDuration - blinkElapsed) / blinkDuration;
                    if (gameActor)
                    {
                        gameActor.RegisterOverrideColor(Color.Lerp(colorA, colorB, lerp), "HellfireBlinkTint");
                    }
                    yield return null;
                }
                gameActor.DeregisterOverrideColor("HellfireBlinkTint");
                for (int i = 90; i <= 360; i += 90)
                {
                    BeamController beam = BeamAPI.FreeFireBeamFromAnywhere(BeamOfHellfire, player, null, specRigidbody.UnitCenter, i, 1.5f);
                    player.DoPostProcessBeam(beam);
                }
                Exploder.DoDefaultExplosion(specRigidbody.UnitCenter, Vector2.zero);
                healthHaver.minimumHealth = 0;
                healthHaver.ApplyDamage(500f, Vector2.down, "Hellfire");
                Destroy(this);
                yield break;
            }
        }
    }
}
