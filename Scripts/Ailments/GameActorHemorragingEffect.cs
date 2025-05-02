using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JuneLib.Status;
using UnityEngine;

namespace Oddments
{
    [Serializable]
    public class GameActorHemorragingEffect : GameActorEffect
    {
        public GoopDefinition gooper;
        public override void OnEffectApplied(GameActor actor, RuntimeGameActorEffectData effectData, float partialAmount = 1)
        {
            base.OnEffectApplied(actor, effectData, partialAmount);
            shittyGradualCheckThing = 0f;
        }
        public override void EffectTick(GameActor actor, RuntimeGameActorEffectData effectData)
        {
            base.EffectTick(actor, effectData);

            this.shittyGradualCheckThing += BraveTime.DeltaTime;
            if (HeresTheTicker <= shittyGradualCheckThing)
            {
                shittyGradualCheckThing = 0f;
                PixelCollider pixelCollider = actor.specRigidbody.HitboxPixelCollider;
                Vector3 vector = pixelCollider.UnitBottomLeft.ToVector3ZisY(0f);
                Vector3 vector2 = pixelCollider.UnitTopRight.ToVector3ZisY(0f);
                if (isGreenBlood || JuneSaveManagerCore.DoPinkBlood)
                {
                    OddSparksDoer.SparksType type = !isGreenBlood ? OddSparksDoer.SparksType.DANGANRONPA_BLOOD : OddSparksDoer.SparksType.VEGETABLE_BLOOD;
                    OddSparksDoer.DoRandomParticleBurst(UnityEngine.Random.Range(4, 8), vector, vector2, Vector3.down, 90f, 0.5f, systemType: type);
                } else
                {
                    GlobalSparksDoer.DoRandomParticleBurst(UnityEngine.Random.Range(4, 8), vector, vector2, Vector3.down, 90f, 0.5f, systemType: GlobalSparksDoer.SparksType.BLOODY_BLOOD);
                }
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(gooper).AddGoopCircle(actor.CenterPosition, 1f);

                actor.healthHaver.ApplyDamage(DMGOnBleed, Vector2.zero, "get this man a bandage");
            }
        }

        public float DMGOnBleed = 1f;
        public float HeresTheTicker = 1.25f;
        public bool isGreenBlood = false;

        private float shittyGradualCheckThing;
    }
}
