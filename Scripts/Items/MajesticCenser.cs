using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.ItemAPI;
using Alexandria.Misc;

namespace Oddments
{
    public class MajesticCenser : PlayerItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(MajesticCenser))
        {
            Name = "Majestic Censer",
            Quality = ItemQuality.EXCLUDED,
            Cooldown = 100,
            PostInitAction = item =>
            {
                item.RemovePickupFromLootTables();
            }

        };

        public override void DoEffect(PlayerController user)
        {
            Vector2 vector2 = m_cachedBlinkPosition;
            LootEngine.DoDefaultPurplePoof(vector2);
            AIActor actor = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(EnemyGUID), vector2.ToIntVector2(), user.CurrentRoom);

            GameActorCharmEffect charm = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultPermanentCharmEffect;
            charm.TintColor = new Color(0.2f, 0.2f, 0.2f, 1f);

            actor.AddPermanentCharm(charm);
            StartCoroutine(ItemBuilder.HandleDuration(this, Duration, user, null));
            actor.StartCoroutine(EnemyCoroutine(actor.healthHaver));

            EnemyGUID = null;
            Destroy(icon);
            base.DoEffect(user);
        }
        public static readonly float Duration = 13f;
        public static readonly float ParticlieTick = 0.25f;

        public IEnumerator EnemyCoroutine(HealthHaver haver)
        {
            float dur = 0f;
            float particleTick = 0f;
            while (dur < Duration)
            {
                if (haver == null) { yield break; }
                dur += BraveTime.DeltaTime;
                if (dur > particleTick)
                {
                    particleTick = dur + ParticlieTick;
                    Vector2 unitBottomLeft = haver.specRigidbody.HitboxPixelCollider.UnitBottomLeft;
                    Vector2 unitTopRight = haver.specRigidbody.HitboxPixelCollider.UnitTopRight;
                    GlobalSparksDoer.DoRandomParticleBurst(1, unitBottomLeft, unitTopRight, new Vector3(1f, 1f, 0f), 120f, 0.75f, null, null, null, GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
                }
                yield return null;
            }
            if (haver)
            {
                haver.PreventAllDamage = false;
                haver.ApplyDamage(100000f, Vector2.zero, "Heaven's Door, remove this mans ability to cum!");
            }
            yield break;
        }

        public override bool CanBeUsed(PlayerController user)
        {
            if (string.IsNullOrEmpty(EnemyGUID) || !CanSpawnEnemyAtPosition(m_cachedBlinkPosition)) { return false; }
            return base.CanBeUsed(user);
        }

        public void DamageEnemies(float dmg, bool fatal, HealthHaver enemies)
        {
            if (this && this.LastOwner != null && !this.IsOnCooldown && fatal
                && string.IsNullOrEmpty(EnemyGUID) && !enemies.IsBoss && enemies && enemies.aiActor
                && enemies.aiActor.GetResistanceForEffectType(EffectResistanceType.Charm) < 1
                && enemies.sprite && enemies.sprite.collection && enemies.spriteAnimator)
            {
                EnemyGUID = enemies.aiActor.EnemyGuid;

                if (icon)
                {
                    Destroy(icon);
                }
                icon = new GameObject("Majestic Censer Icon");
                tk2dSprite sprite = tk2dSprite.AddComponent(icon, enemies.sprite.collection, enemies.sprite.spriteId);
                m_cachedOffset = (Vector2)(enemies.specRigidbody.HitboxPixelCollider.Offset + (enemies.specRigidbody.HitboxPixelCollider.Dimensions / 2)) / 16;

                sprite.usesOverrideMaterial = true;
                sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/HighPriestAfterImage");
                sprite.renderer.material.SetFloat("_EmissivePower", 100f);
                sprite.renderer.material.SetFloat("_Opacity", 0.6f);
                sprite.renderer.material.SetFloat("_AllColorsToggle", 1f);
                sprite.renderer.material.SetColor("_DashColor", new Color(0.5f, 0f, 0f));
                tk2dSpriteAnimator animator = sprite.gameObject.AddComponent<tk2dSpriteAnimator>();
                animator.Library = enemies.spriteAnimator.Library;
                animator.Play();

                UpdateTarget();
            }
        }
        public bool CanSpawnEnemyAtPosition(Vector2 vector)
        {
            /*also stolen from expand XOXO*/
            if (LastOwner.CurrentRoom == null)
            {
                return false;
            }

            bool flag = true;
            CellData cellData = GameManager.Instance.Dungeon.data[vector.ToIntVector2(VectorConversions.Floor)];
            if (cellData == null) { return false; }
            RoomHandler nearestRoom = cellData.nearestRoom;
            if (cellData.type != CellType.FLOOR && cellData.type != CellType.PIT) { flag = false; }
            if (nearestRoom != LastOwner.CurrentRoom) { flag = false; }
            if (cellData.isExitCell) { flag = false; }
            if (nearestRoom.visibility == RoomHandler.VisibilityStatus.OBSCURED || nearestRoom.visibility == RoomHandler.VisibilityStatus.REOBSCURED) { flag = false; }
            return flag;
        }

        public void UpdateTarget()
        {
            /*stolen from expand luvs n kisses*/
            m_cachedBlinkPosition = new Vector2();
            bool IsKeyboardAndMouse = BraveInput.GetInstanceForPlayer(LastOwner.PlayerIDX).IsKeyboardAndMouse(false);
            if (IsKeyboardAndMouse)
            {
                m_cachedBlinkPosition = LastOwner.unadjustedAimPoint.XY();
            }
            else
            {
                if (LastOwner.m_activeActions != null) { m_cachedBlinkPosition = LastOwner.specRigidbody.UnitCenter + LastOwner.m_activeActions.Aim.Vector.normalized; }
            }
            m_cachedBlinkPosition = BraveMathCollege.ClampToBounds(m_cachedBlinkPosition, GameManager.Instance.MainCameraController.MinVisiblePoint, GameManager.Instance.MainCameraController.MaxVisiblePoint);
            bool canSpawn = CanSpawnEnemyAtPosition(m_cachedBlinkPosition);
            icon.GetComponent<tk2dSprite>().renderer.enabled = canSpawn;
            icon.transform.position = m_cachedBlinkPosition - m_cachedOffset;
        }
        private Vector2 m_cachedBlinkPosition;
        private Vector2 m_cachedOffset;
        public override void Update()
        {
            base.Update();
            if (LastOwner)
            {
                if (icon)
                {
                    UpdateTarget();
                }
            }
        }


        public override void Pickup(PlayerController player)
        {
            player.OnAnyEnemyReceivedDamage += DamageEnemies;
            base.Pickup(player);
        }

        public override void OnPreDrop(PlayerController user)
        {
            user.OnAnyEnemyReceivedDamage -= DamageEnemies;
            base.OnPreDrop(user);
        }
        
        protected GameObject icon;
        public string EnemyGUID;
    }
}
