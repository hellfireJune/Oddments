using Alexandria.PrefabAPI;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using JuneLib;
using Alexandria.Misc;

namespace Oddments
{
    public class Cellophane : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(Cellophane))
        {
            Name = "cellophane",
            Quality = ItemQuality.EXCLUDED,
            PostInitAction = item =>
            {
                GameObject cello = PrefabBuilder.BuildObject("CellophaneClone");
                cello.gameObject.AddComponent<CellophaneCloneHandle>();
                clonePrefab = cello;
                item.RemovePickupFromLootTables();
            }
        };

        public override void Pickup(PlayerController player)
        {
            ETGMod.AIActor.OnPostStart += SpawnShadows;
            base.Pickup(player);
        }

        private void SpawnShadows(AIActor obj)
        {
            if (!obj || !obj.healthHaver)
            {
                return;
            }
            for (int i = -1; i < 2; i += 2)
            {
                GameObject cello = Instantiate(clonePrefab, obj.transform);
                /*specRigidbody.HitboxPixelCollider.ManualWidth = obj.specRigidbody.HitboxPixelCollider.Width;
                specRigidbody.HitboxPixelCollider.ManualHeight = obj.specRigidbody.HitboxPixelCollider.Height;*/
                //specRigidBody.HitboxPixelCollider.RegenerateFromManual(cello.transform, obj.specRigidbody.HitboxPixelCollider.Offset, obj.specRigidbody.HitboxPixelCollider.Dimensions);
                //cello.transform.localPosition = new Vector3(obj.specRigidbody.UnitWidth * i, (obj.specRigidbody.UnitWidth) * i, 0);

                tk2dSprite.AddComponent(cello, obj.sprite.collection, obj.sprite.spriteId);

                CellophaneCloneHandle cellophane = cello.GetComponent<CellophaneCloneHandle>();
                cellophane.Mult = i;
            }
        }

        public static GameObject clonePrefab;

        /*
        protected void UpdateBlinkShadow(PlayerController Owner, Vector2 delta, bool canWarpDirectly) {
            int? m_overrideFlatColorID = ReflectionHelpers.ReflectGetField<int>(typeof(PlayerController), "m_overrideFlatColorID", Owner);
            if (m_extantBlinkShadow == null) {
                GameObject go = new GameObject("blinkshadow");
                m_extantBlinkShadow = tk2dSprite.AddComponent(go, Owner.sprite.Collection, Owner.sprite.spriteId);
                m_extantBlinkShadow.transform.position = m_cachedBlinkPosition + (Owner.sprite.transform.position.XY() - Owner.specRigidbody.UnitCenter);
                tk2dSpriteAnimator tk2dSpriteAnimator = m_extantBlinkShadow.gameObject.AddComponent<tk2dSpriteAnimator>();
                tk2dSpriteAnimator.Library = Owner.spriteAnimator.Library;     
                if (m_overrideFlatColorID.HasValue) { m_extantBlinkShadow.renderer.material.SetColor(m_overrideFlatColorID.Value, (!canWarpDirectly) ? new Color(0.4f, 0f, 0f, 1f) : new Color(0.25f, 0.25f, 0.25f, 1f)); }
                m_extantBlinkShadow.usesOverrideMaterial = true;
                m_extantBlinkShadow.FlipX = Owner.sprite.FlipX;
                m_extantBlinkShadow.FlipY = Owner.sprite.FlipY;
                Owner.OnBlinkShadowCreated?.Invoke(m_extantBlinkShadow);
            } else {
                if (delta == Vector2.zero) {
                    m_extantBlinkShadow.spriteAnimator.Stop();
                    m_extantBlinkShadow.SetSprite(Owner.sprite.Collection, Owner.sprite.spriteId);
                } else {
                    float? m_currentGunAngle = null;
                    try { m_currentGunAngle = ReflectionHelpers.ReflectGetField<float>(typeof(PlayerController), "m_currentGunAngle", Owner); } catch (Exception) { }
                    string baseAnimationName = string.Empty;
                    if (m_currentGunAngle.HasValue) {
                        baseAnimationName =  GetBaseAnimationName(Owner, delta, m_currentGunAngle.Value, false, true);
                    }
                    if (!string.IsNullOrEmpty(baseAnimationName) && !m_extantBlinkShadow.spriteAnimator.IsPlaying(baseAnimationName)) {
                        m_extantBlinkShadow.spriteAnimator.Play(baseAnimationName);
                    }
                }
                if (m_overrideFlatColorID.HasValue) { m_extantBlinkShadow.renderer.material.SetColor(m_overrideFlatColorID.Value, (!canWarpDirectly) ? new Color(0.4f, 0f, 0f, 1f) : new Color(0.25f, 0.25f, 0.25f, 1f)); }
                m_extantBlinkShadow.transform.position = m_cachedBlinkPosition + (Owner.sprite.transform.position.XY() - Owner.specRigidbody.UnitCenter);
            }
            m_extantBlinkShadow.FlipX = Owner.sprite.FlipX;
            m_extantBlinkShadow.FlipY = Owner.sprite.FlipY;
        }*/

        private class CellophaneCloneHandle : BraveBehaviour
        {
            void Start()
            {
                //HitboxMonitor.DisplayHitbox(specRigidbody);
                m_parentBody = transform.parent.GetComponent<SpeculativeRigidbody>();

                //specRigidbody.OnRigidbodyCollision += Collision;
                //sprite.renderer.material = m_parentBody.renderer.material;
                /*sprite.usesOverrideMaterial = true;
                sprite.renderer.material.color = Mult > 1 ? new Color(0.4f, 0f, 0f, 0.5f) : new Color(0f, 0.075f, 0.325f, 0.5f);*/

                sprite.renderer.enabled = false;
                sprite.usesOverrideMaterial = true;
                sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/HighPriestAfterImage");
                sprite.renderer.sharedMaterial.SetFloat("_EmissivePower", 100f);
                sprite.renderer.sharedMaterial.SetFloat("_Opacity", 0.6f);
                sprite.renderer.sharedMaterial.SetColor("_DashColor", Mult > 0 ? new Color(0.6f, 0f, 0f, 1f) : new Color(0f, 0.125f, 0.475f, 1f));
                sprite.renderer.sharedMaterial.SetFloat("_AllColorsToggle", 1f);
            }

            void Update()
            {
                if (m_parentBody == null || m_parentBody.sprite == null || sprite == null)
                {
                    return;
                }
                sprite.renderer.enabled = m_parentBody.sprite.renderer.enabled;
                Vector2 offset = new Vector2((m_parentBody.UnitWidth), (m_parentBody.UnitHeight)) * 0.6f;

                Vector2 directionToPlayer = (m_parentBody.UnitCenter - GameManager.Instance.PrimaryPlayer.specRigidbody.UnitCenter).normalized;
                offset = Vector2.Scale(offset, directionToPlayer);

                offset *= Mult;
                offset = offset.Rotate(90);
                transform.localPosition = offset;

                sprite.spriteId = m_parentBody.sprite.spriteId;
            }

            /*private void Collision(CollisionData data)
            {
                SpeculativeRigidbody otherbody = data.OtherRigidbody;
                SpeculativeRigidbody rigidbody = data.MyRigidbody;
                PixelCollider mycollider = data.MyPixelCollider;
                PixelCollider othercollider = data.OtherPixelCollider;
                //ETGModConsole.Log("oh, gloop?");
                if (otherbody.GetComponent<CellophaneCloneHandle>())
                {
                    ETGModConsole.Log("JUUUUUUNE JUUUNE YOURE CHECKING THE WRONG RIGIDBODY  JUUUUNE");
                    PhysicsEngine.SingleCollision(rigidbody, mycollider, m_parentBody, m_parentBody.PrimaryPixelCollider, PhysicsEngine.m_cwrqStepList, true);
                }
                else
                {
                    PhysicsEngine.SingleCollision(otherbody, othercollider, m_parentBody, m_parentBody.PrimaryPixelCollider, PhysicsEngine.m_cwrqStepList, true);
                }
                //PhysicsEngine.SkipCollision = true;
            }*/
            public int Mult;
            private SpeculativeRigidbody m_parentBody;
        }
    }
}
