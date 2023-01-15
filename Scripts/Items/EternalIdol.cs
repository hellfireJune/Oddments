using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using JuneLib.Items;
using UnityEngine;
using Alexandria.PrefabAPI;
using Alexandria.ItemAPI;
using Alexandria.Misc;

namespace Oddments
{
    public class EternalIdol : PassiveItem
    {
        private static readonly string chainSprite = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/mimicbait.png";

        public static ItemTemplate template = new ItemTemplate(typeof(EternalIdol))
        {
            Name = "Eternal Idol",
            Description = "The Great Chain",
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = item =>
            {
                GameObject chainObject = PrefabBuilder.BuildObject("Eternal Chain");
                int spriteID = SpriteBuilder.AddSpriteToCollection(chainSprite, ETGMod.Databases.Items.ProjectileCollection);
                tk2dTiledSprite tiledSprite = chainObject.gameObject.GetOrAddComponent<tk2dTiledSprite>();

                tiledSprite.SetSprite(ETGMod.Databases.Items.ProjectileCollection, spriteID);
                tk2dSpriteDefinition def = tiledSprite.GetCurrentSpriteDef();
                def.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleLeft);
                EternalIdolLinker.linkPrefab = chainObject;

                item.RemovePickupFromLootTables();
            }
        };

        public override void Pickup(PlayerController player)
        {
            ETGMod.AIActor.OnPostStart += LinkEnemies;
            base.Pickup(player);
        }

        public override void DisableEffect(PlayerController player)
        {
            ETGMod.AIActor.OnPostStart -= LinkEnemies;
            base.DisableEffect(player);
        }

        protected float m_radius = 75f;

        public void LinkEnemies(AIActor actor)
        {
            if (!actor.IsNormalEnemy || !actor.healthHaver || actor.healthHaver.IsBoss)
                return;
            if (actor.GetResistanceForEffectType(EffectResistanceType.Charm) >= 1)
                return;
            if (actor.ParentRoom == null)
                return;

            RoomHandler room = actor.ParentRoom;
            List<AIActor> enemies = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
            if (enemies != null && enemies.Count > 1)
            {
                List<AIActor> nearbyEnemies = enemies.Where(possibleEnemy => Vector2.Distance(possibleEnemy.CenterPosition, actor.CenterPosition) <= m_radius
                && possibleEnemy.GetResistanceForEffectType(EffectResistanceType.Charm) < 1 && actor.IsNormalEnemy && actor.healthHaver && !actor.healthHaver.IsBoss).ToList();
                nearbyEnemies.Remove(actor);

                AIActor enemy = BraveUtility.RandomElement(nearbyEnemies);

                EternalIdolLinker otherLinker = enemy.gameObject.AddComponent<EternalIdolLinker>();
                EternalIdolLinker thisLinker = actor.gameObject.AddComponent<EternalIdolLinker>();
                thisLinker.other = otherLinker;
                otherLinker.other = thisLinker;
            }
        }

        public class EternalIdolLinker : BraveBehaviour
        {
            public EternalIdolLinker other;

            public static GameObject linkPrefab;

            protected GameObject linker;

            public void Start()
            {
                framesAlive = 0;
                if (aiActor && aiActor.healthHaver)
                {
                    healthHaver.OnDeath += KillOther;
                }
            }

            private void KillOther(Vector2 obj)
            {
                if (other != null && other.healthHaver && other.healthHaver.IsAlive)
                {
                    other.healthHaver.ApplyDamage(27616^2, obj, "Eternity");
                }
            } 

            public void Update()
            {
                if (aiActor == null || other == null || other.aiActor == null)
                {
                    Destroy(this);
                    if (healthHaver)
                    {
                        healthHaver.ApplyDamage(27616 ^ 2, Vector2.zero, "Eternity");
                    }
                    return;
                }
                if (!aiActor.isActiveAndEnabled ||
                    aiAnimator.IsPlaying("spawn") || aiAnimator.IsPlaying("awaken"))
                {
                    return;
                }
                framesAlive++;
                if (linker)
                {
                    tk2dTiledSprite tiler = linker.GetComponent<tk2dTiledSprite>();
                    float mult = Mathf.Min(1f, framesAlive / timeToLink);
                    Vector2 currentDirVec = other.aiActor.CenterPosition - aiActor.CenterPosition;
                    float pixelsWide = Mathf.RoundToInt(currentDirVec.magnitude / 0.0625f);
                    tiler.dimensions = new Vector2(pixelsWide * mult, 3);

                    float currentChainSpriteAngle = BraveMathCollege.Atan2Degrees(currentDirVec);
                    linker.transform.rotation = Quaternion.Euler(0f, 0f, currentChainSpriteAngle);
                } else if (other.linker == null)
                {
                    linker = Instantiate(linkPrefab);
                    linker.transform.parent = aiActor.transform;
                    linker.transform.position = aiActor.CenterPosition.ToVector3ZUp(0f);
                    tk2dTiledSprite tiler = linker.GetComponent<tk2dTiledSprite>();
                    tiler.dimensions = Vector2.zero;
                    sprite.AttachRenderer(tiler);
                    other.sprite.AttachRenderer(tiler);
                }
            }

            private float framesAlive;

            private readonly float timeToLink = 15f;
        }
    }
}
