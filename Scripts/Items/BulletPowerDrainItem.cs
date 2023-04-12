using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using Dungeonator;
using UnityEngine;
using Alexandria.PrefabAPI;
using Alexandria.Misc;

namespace Oddments
{
    public class BulletPowerDrainItem : CustomChargeTypeItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(BulletPowerDrainItem))
        {
            PostInitAction = item =>
            {
                BulletPowerDrainItem item2 = (BulletPowerDrainItem)item;
                item2.specialCooldown = 25f;
                
                shader = Module.oddBundle.LoadAsset<Shader>("RadialJamShaderTest.shader");
                radiator = (GameObject)ResourceCache.Acquire("Global VFX/HeatIndicator");
                /*Destroy(radiator.GetComponent<HeatIndicatorController>());
                radiator.AddComponent<JamIndicatorController>();
                radiator.GetOrAddComponent<MeshRenderer>().material = new Material(shader);*/
                //radiator.GetComponent<MeshRenderer>().material.mainTexture = ResourceExtractor.GetTextureFromResource($"{Module.ASSEMBLY_NAME}/Resources/example_item_sprite.png");
                item.RemovePickupFromLootTables();
            }
        };

        public static Shader shader;
        public static GameObject radiator;
        private GameObject radiatorreal;

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            currentDMGUp += 0.1f;
            CacheDamage();
        }

        public override void Update()
        {
            base.Update();
            if (LastOwner)
            {
                RoomHandler room = LastOwner.CurrentRoom;
                if (room != lastRoom)
                {
                    lastRoom = room;
                    currentDMGUp = 0f;
                    CacheDamage();
                }

                if (LastOwner.IsInCombat && !LastOwner.IsDodgeRolling)
                {

                    for (int i = 0; i < StaticReferenceManager.AllProjectiles.Count; i++)
                    {
                        Projectile projectile = StaticReferenceManager.AllProjectiles[i];
                        if (projectile && projectile.Owner is AIActor)
                        {
                            float sqrMagnitude = (projectile.transform.position.XY() - LastOwner.CenterPosition).sqrMagnitude;
                            if (sqrMagnitude < CheckRadius
                                && !projectile.IsBlackBullet)
                            {
                                projectile.BecomeBlackBullet();
                                GlobalSparksDoer.DoRandomParticleBurst(10, projectile.specRigidbody.UnitTopLeft, projectile.specRigidbody.UnitBottomRight, Vector3.up, 30f, 0.75f, systemType: GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
                                this.RemainingSpecialCooldown -= 1;
                                break;
                            }
                        }
                    }
                }
                if (radiatorreal == null)
                {
                    radiatorreal = Instantiate(radiator, base.LastOwner.CenterPosition.ToVector3ZisY(0), Quaternion.identity, LastOwner.transform);
                    HeatIndicatorController indicator = radiatorreal.GetComponent<HeatIndicatorController>();
                    indicator.CurrentRadius = CheckRadius / 2;
                    indicator.CurrentColor = Color.black;
                    indicator.IsFire = false;
                }
            }
        }

        protected void CacheDamage()
        {
            this.RemoveStat(PlayerStats.StatType.Damage);
            this.AddStat(PlayerStats.StatType.Damage, currentDMGUp);
            LastOwner.stats.RecalculateStats(LastOwner);
        }

        protected RoomHandler lastRoom;
        protected float currentDMGUp;

        protected float CheckRadius = 5f;

        /*private class BulletChecker : MonoBehaviour
        {

        }

        public class JamIndicatorController : MonoBehaviour
        {

            public void Awake()
            {
                this.m_radiusID = Shader.PropertyToID("_Radius");
                this.m_centerID = Shader.PropertyToID("_WorldCenter");
                this.m_fogID = Shader.PropertyToID("_FogStrength");
                this.m_opacityID = Shader.PropertyToID("_Opacity");
                this.m_materialInst = base.GetComponent<MeshRenderer>().material;
            }
            public float CurrentRadius = 5f;
            public float FogStrength = 0.5f;
            public float Opacity = 0.2f;
            public void LateUpdate()
            {
                base.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
                base.transform.localScale = new Vector3(this.CurrentRadius * 2f, this.CurrentRadius * 2f * Mathf.Sqrt(2f), 1f);
                Vector3 position = base.transform.position;
                this.m_materialInst.SetVector(this.m_centerID, new Vector4(position.x, position.y, position.z, 0f));
                this.m_materialInst.SetFloat(this.m_radiusID, this.CurrentRadius);
                this.m_materialInst.SetFloat(this.m_fogID, this.FogStrength);
                this.m_materialInst.SetFloat(this.m_opacityID, Opacity);
            }
            private Material m_materialInst;
            private int m_radiusID;
            private int m_centerID;
            private int m_fogID;
            private int m_opacityID;
        }*/
    }
}
