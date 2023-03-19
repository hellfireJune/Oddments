using Alexandria.ItemAPI;
using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public class UnnamedTestGun : GunBehaviour
    {
        public static GunTemplate gunTemplate = new GunTemplate(typeof(UnnamedTestGun))
        {
            Name = "Unnamed Test Gun",
            PostInitAction = gun =>
            {
                gun.AddProjectileModuleFrom("ak-47", true, false);
                gun.DefaultModule.ammoCost = 1;
                gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
                gun.DefaultModule.angleVariance = 3f;
                gun.DefaultModule.cooldownTime = 0.5f;
                gun.DefaultModule.numberOfShotsInClip = 20;
                Gun gun2 = PickupObjectDatabase.GetById(151) as Gun;
                gun.muzzleFlashEffects = gun2.muzzleFlashEffects;

                Projectile projectile = Instantiate(gun.DefaultModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                DontDestroyOnLoad(projectile);
                projectile.sprite.renderer.enabled = false;
                projectile.gameObject.AddComponent<UnnamedTestProjectile>();
                projectile.specRigidbody.CollideWithOthers = false;
                projectile.specRigidbody.CollideWithTileMap = false;
                projectile.baseData.speed = 1f;
                gun.DefaultModule.projectiles[0] = projectile;
            }
        };

        public class UnnamedTestProjectile : BraveBehaviour
        {
            public float Duration = 4f;
            public float PoofInterval = 0.2f;
            public float moveSpeed = 20f;
            public float damage = 6f;
            public float angularVariance = 360f;
            public float collisionGenerosity = 15f;
            public IEnumerator HandleTheWarbling(Vector2 direction, Vector3 startPosition, Projectile projectile)
            {
                RoomHandler room = GameManager.Instance.Dungeon.GetRoomFromPosition(((Vector2)startPosition).ToIntVector2());
                direction = direction.normalized;

                Vector3 pos = startPosition;
                AIActor lastEnemy = null;
                bool skip = true;
                float elapsed = 0f;

                float poofer = PoofInterval;
                float elapsedWithoutEnemy = 0f;
                while (elapsed < Duration)
                {
                    elapsed += BraveTime.DeltaTime;
                    poofer += BraveTime.DeltaTime;

                    float speed = moveSpeed * BraveTime.DeltaTime;

                    AIActor target = (lastEnemy && !skip) ? lastEnemy : Core.GetNearestEnemyWithIgnore(room, pos, out _, lastEnemy);
                    if (target)
                    {
                        skip = false;
                        Vector2 dirToEnemy = target.Position - pos;
                        Debug.Log(dirToEnemy.ToAngle());
                        float angleDiff = dirToEnemy.ToAngle() - direction.ToAngle();
                        angleDiff = (angleDiff + 180) % 360 - 180;

                        float realVariance = angularVariance * BraveTime.DeltaTime;
                        angleDiff = Mathf.Clamp(angleDiff, -realVariance, realVariance);
                        direction = direction.Rotate(angleDiff);

                        PixelCollider hitbox = target.specRigidbody.HitboxPixelCollider;
                        if (hitbox != null)
                        {
                            Vector2 nearestPointA = BraveMathCollege.ClosestPointOnRectangle(pos, hitbox.UnitBottomLeft, hitbox.UnitDimensions);
                            float distancePointA = Vector2.Distance(nearestPointA, pos);
                            Vector3 posToBeAt = pos + (Vector3)(direction.normalized * speed);
                            Vector2 nearestPointB = BraveMathCollege.ClosestPointOnRectangle(posToBeAt, hitbox.UnitBottomLeft, hitbox.UnitDimensions);
                            float distancePointB = Vector2.Distance(nearestPointB, posToBeAt);

                            Debug.DrawLine(pos, posToBeAt);

                            if (distancePointA < speed && distancePointB < speed)
                            {
                                skip = true;

                                LootEngine.DoDefaultItemPoof(target.Position);
                                target.healthHaver.ApplyDamage(damage, Vector2.zero, "Funny Unnamed Gun");
                            }
                        }

                        lastEnemy = target;
                    } else
                    {
                        if (lastEnemy)
                        {
                            elapsedWithoutEnemy += BraveTime.DeltaTime;
                            if (elapsedWithoutEnemy > 0.341208000001f)
                            {
                                elapsedWithoutEnemy = 0;
                                lastEnemy = null;
                            }
                        } 
                    }
                    Vector2 realDirec = direction.normalized * speed;
                    //realDirec = realDirec.Rotate(Mathf.Sin(elapsed)*BraveTime.DeltaTime*5);
                    pos += (Vector3)realDirec;

                    if (poofer > PoofInterval)
                    {
                        poofer -= PoofInterval;
                        LootEngine.DoDefaultItemPoof(pos);
                    }
                    yield return null;
                }
                yield break;
            }

            private void Update()
            {
                if (!projectile) { return; }
                GameManager.Instance.StartCoroutine(HandleTheWarbling(projectile.specRigidbody?.Velocity ?? projectile.LastVelocity, projectile.specRigidbody?.UnitCenter ?? projectile.transform.position, projectile));
                Destroy(gameObject);
            }
        }
    }
}
