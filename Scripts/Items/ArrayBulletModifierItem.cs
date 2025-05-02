using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public class ArrayBulletModifierItem : PassiveItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(ArrayBulletModifierItem))
        {
            Name = "Array Bullets",
            Description = "Matrishells",
        };
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.GetComponent<JEventsComponent>().PostProcessProjectileMod += Player_PostProcessProjectile;
        }


        private float m_activateChance = 1f;

        private void Player_PostProcessProjectile(Projectile arg1, Gun arg2, ProjectileModule arg3)
        {
            if (arg3 == null || Owner == null || !arg1 || !arg1.sprite) { return; }

            if (arg1.gameObject.GetComponent<ArrayBulletMod>()) { return; }

            if (UnityEngine.Random.value < m_activateChance)
            {
                arg1.baseData.damage *= m_miniDamage;
                arg1.RuntimeUpdateScale(m_miniScale);

                Projectile baseProj = Instantiate(arg3.GetCurrentProjectile());
                for (int i = 1; i <= 4; i++)
                {
                    ETGModConsole.Log($"Successfully added bonus projectile: {i}");
                    Projectile proj = VolleyUtility.ShootSingleProjectile(baseProj, arg1.sprite.WorldCenter, arg1.specRigidbody.Velocity.ToAngle(), false, Owner);
                    proj.transform.position = arg1.transform.position;
                    proj.transform.rotation = arg1.transform.rotation;
                    ArrayBulletMod mod = proj.gameObject.AddComponent<ArrayBulletMod>();
                    mod.Initialize(arg1, i, m_miniDamage, m_miniScale);
                    Owner.DoPostProcessProjectile(proj);
                } 
            }
        }

        public float m_miniDamage = 0.3f;
        public float m_miniScale = 0.5f;

        private class ArrayBulletMod : BraveBehaviour
        {
            public Projectile bulletOnTop;

            public int idx = 0;


            public void Initialize(Projectile theGoat, int newIdx, float miniDamage, float miniScale)
            {
                bulletOnTop = theGoat;
                idx = newIdx;

                if (newIdx > 1 && newIdx < 4)
                {
                    projectile.baseData.damage *= miniDamage;
                    projectile.RuntimeUpdateScale(miniScale);
                }
            }

            public void FixedUpdate()
            {
                Debug.Log("projectile real");
                if (bulletOnTop != null && bulletOnTop.specRigidbody && bulletOnTop.gameObject)
                {
                    //specRigidbody.Velocity = bulletOnTop.specRigidbody.Velocity;

                    int baseIdx = idx - 2;
                    if (baseIdx <= 0)
                    {
                        baseIdx--;
                    }
                    Debug.Log($"everything exists, {baseIdx}");
                    Vector3 offset = (Vector3)bulletOnTop.specRigidbody.Velocity.Rotate(90).normalized*30;
                    //transform.position = bulletOnTop.transform.position + offset*baseIdx;
                    //specRigidbody.Velocity = Vector3.zero;
                }
            }
        }
    }
}
