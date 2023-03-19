using Dungeonator;
using System;
using System.Collections;
using UnityEngine;

namespace Oddments
{
    public class LightningModifier : BraveBehaviour
    {
        public static GameObject DefaultLightningPrefab;
        public static void Init()
        {
            { }

            ComplexProjectileModifier mod = PickupObjectDatabase.GetById(298) as ComplexProjectileModifier;
            DefaultLightningPrefab = mod.ChainLightningVFX;
        }

        public LightningModifier()
        {
            LightningPrefab = DefaultLightningPrefab;
        }
        public void Start()
        {
            if (projectile)
            {
                projectile.OnDestruction += DestroyProj;
                projectile.OnHitEnemy += HitEnemy;

                if (IsJustLightning)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                ETGModConsole.Log("projectile be null");
            }
        }

        private void HitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2 && arg2.aiActor)
            {
                m_dontJoltMe = arg2.aiActor;
            }
        }
        private AIActor m_dontJoltMe;

        public IEnumerator StartLightning(Vector3 pos, Vector2 backupVelocity)
        {
            Vector3 newPos = pos;
            RoomHandler room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(((Vector2)pos).ToIntVector2());

            int chains = 0;

            AIActor lastEnemy = null;
            tk2dTiledSprite lastLightning = null;
            while (true)
            {
                if (lastLightning && lastLightning.gameObject)
                {
                    /*tk2dSpriteAnimator animator = lastLightning.gameObject.GetComponent<tk2dSpriteAnimator>();
                    animator.PlayAndDestroyObject("");*/
                    Destroy(lastLightning.gameObject);

                }
                if (chains >= MaxChains)
                {
                    break;
                }

                AIActor enemy = Core.GetNearestEnemyWithIgnore(room, newPos, out float dist, lastEnemy ?? m_dontJoltMe);
                if (dist < MaxDistance || chains == 0)
                {
                    lastEnemy = enemy;
                    Vector3 endPos = lastEnemy?.Position ?? newPos + ((Vector3)((backupVelocity.Rotate(IsJustLightning ? 0 : 180)).normalized) * MaxDistance);
                    tk2dTiledSprite chainer = SpawnManager.SpawnVFX(LightningPrefab).GetComponent<tk2dTiledSprite>();
                    //LootEngine.DoDefaultItemPoof(endPos);

                    Vector2 length = endPos - newPos;
                    chainer.transform.position = newPos;

                    chainer.dimensions = new Vector2(Mathf.RoundToInt(length.magnitude / 0.0625f), chainer.dimensions.y);
                    float currentChainSpriteAngle = BraveMathCollege.Atan2Degrees(length);
                    chainer.transform.rotation = Quaternion.Euler(0f, 0f, currentChainSpriteAngle);
                    chainer.UpdateZDepth();

                    lastLightning = chainer;
                    chains++;
                    newPos = endPos;
                } else { break; }

                yield return new WaitForSeconds(0.05f);
            }
            yield break;
        }


        private void DestroyProj(Projectile obj)
        {
            base.OnDestroy();

            Vector3 pos = specRigidbody?.UnitCenter ?? transform.position;
            GameManager.Instance.StartCoroutine(StartLightning(pos, obj?.LastVelocity ?? new Vector2(1, 0)));
        }

        public GameObject LightningPrefab = DefaultLightningPrefab;
        public bool IsJustLightning = false;
        public float MaxDistance = 3f;
        public int MaxChains = 3;
    }
}
