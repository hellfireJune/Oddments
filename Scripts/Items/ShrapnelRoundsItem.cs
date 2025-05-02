using Alexandria.ItemAPI;
using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    internal class ShrapnelRoundsItem : OddProjectileModifierItem
    {
        public static Projectile GenericAKProjectile;

        public static OddItemTemplate template = new OddItemTemplate(typeof(ShrapnelRoundsItem))
        {
            Name = "Packed Shells",
            Description = "Big Beautiful Bullets",
            LongDescription = "Bullets release additional bullets as a shotgun fire when near enemies\n\nShells that have bullets inside of them, presumably with even more shells inside those bullets.",
            SpriteResource = $"{Module.SPRITE_PATH}/packedshells.png",
            Quality = ItemQuality.B,
            PostInitAction = item =>
            {
                var proj = Instantiate<Projectile>((PickupObjectDatabase.GetById(15) as Gun).DefaultModule.projectiles[0]);
                proj.gameObject.SetActive(false);
                proj.baseData.range /= 3;
                proj.baseData.damage = 0;
                proj.baseData.force = 0;
                //proj.baseData.speed = 0;
                GenericAKProjectile = proj;
                FakePrefab.MarkAsFakePrefab(proj.gameObject);
                DontDestroyOnLoad(proj);

                item.MakeBulletMod();

                item.AddPassiveStatModifier(PlayerStats.StatType.PlayerBulletScale, 1.3333f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            }
        };

        public override bool ApplyBulletEffect(Projectile projectile)
        {
            projectile.gameObject.AddComponent<BurstInMidairProjectile>();
            return true;
        }

    }

    public class BurstInMidairProjectile : BraveBehaviour
    {
        public int minBonusProjectiles = 0;
        public int maxBonusProjectiles = 2;

        public SpawnProjModifier spawnProjModifier;

        public void Start()
        {
            if (projectile && !spawnProjModifier)
            {
                if (projectile.GetComponent<SpawnProjModifier>() != null)
                {
                    Destroy(this);
                    return;
                }

                var spawnMod = projectile.gameObject.AddComponent<SpawnProjModifier>();
                spawnMod.numberToSpawnOnCollison = 4;
                spawnMod.collisionSpawnStyle = SpawnProjModifier.CollisionSpawnStyle.FLAK_BURST;
                spawnMod.SpawnedProjectilesInheritAppearance = true;
                spawnMod.SpawnedProjectilesInheritData = true;
                spawnMod.projectileToSpawnOnCollision = ShrapnelRoundsItem.GenericAKProjectile;
                spawnMod.SpawnedProjectileScaleModifier = 0.5f;

                spawnProjModifier = spawnMod;
            }
        }

        float BurstRadius = 4.6f;
        float BurstAngle = 18;

        public void Update()
        {

            RoomHandler absoluteRoomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(projectile.LastPosition.IntXY(VectorConversions.Floor));
            List<AIActor> activeEnemies = absoluteRoomFromPosition.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);

            //Debug.Log("doing thing");
            if (activeEnemies == null || activeEnemies.Count == 0)
            {
                return;
            }
            //Debug.Log("yes enemies");

            Vector2 b = (!base.sprite) ? base.transform.position.XY() : base.sprite.WorldCenter;
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                AIActor aiactor = activeEnemies[i];
                if (aiactor && aiactor.IsWorthShootingAt && !aiactor.IsGone && aiactor.specRigidbody)
                {
                    var hitbox = aiactor.specRigidbody.HitboxPixelCollider;
                    Vector2 pos = BraveMathCollege.ClosestPointOnRectangle(b, hitbox.UnitBottomLeft, hitbox.UnitDimensions);
                    Vector2 vector3 = pos - b;
                    float sqrMagnitude = vector3.magnitude;
                    Debug.Log(sqrMagnitude);
                    if (sqrMagnitude > BurstRadius)
                    {
                        continue;
                    }
                    float angle = vector3.ToAngle();
                    float projAngle = projectile.LastVelocity.ToAngle();
                    float nangle = angle - projAngle;

                    if (-BurstAngle < nangle && nangle < BurstAngle)
                    {
                        KillSelf();
                        return;
                    }
                }
            }
        }

        public void KillSelf()
        {
            //spawnProjModifier.spawnProjectilesOnCollision = true;

            int bonuses = UnityEngine.Random.Range(minBonusProjectiles, maxBonusProjectiles + 1);
            CopiedFlakStuff(bonuses);
            //projectile.DieInAir();
            Destroy(spawnProjModifier);
            Destroy(this);
        }

        public void CopiedFlakStuff(int numToSpawn)
        {

            int num = UnityEngine.Random.Range(0, 20);
            Vector2 unitBottomLeft = spawnProjModifier.m_srb.UnitBottomLeft;
            Vector2 unitTopRight = spawnProjModifier.m_srb.UnitTopRight;
            for (int i = 0; i < numToSpawn; i++)
            {
                Projectile proj = (!spawnProjModifier.UsesMultipleCollisionSpawnProjectiles) ? spawnProjModifier.projectileToSpawnOnCollision : spawnProjModifier.collisionSpawnProjectiles[UnityEngine.Random.Range(0, spawnProjModifier.collisionSpawnProjectiles.Length)];
                float num2 = 15f - BraveMathCollege.GetLowDiscrepancyRandom(i + num) * 30f;
                float zRotation = BraveMathCollege.Atan2Degrees(projectile.LastVelocity.normalized) + num2;
                Vector2 vector = new Vector2(UnityEngine.Random.Range(unitBottomLeft.x, unitTopRight.x), UnityEngine.Random.Range(unitBottomLeft.y, unitTopRight.y));
                spawnProjModifier.SpawnProjectile(proj, vector.ToVector3ZUp(base.transform.position.z), zRotation);
            }
        }
    }
}
