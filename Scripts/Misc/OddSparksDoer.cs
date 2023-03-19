using Alexandria.ItemAPI;
using System.Collections;
using UnityEngine;

namespace Oddments
{
    public static class OddSparksDoer
    {
        public static void DoSingleParticle(Vector3 position, Vector3 direction, float? startSize = null, float? startLifetime = null, Color? startColor = null, SparksType systemType = SparksType.SPARKS_ADDITIVE_DEFAULT)
        {
            ParticleSystem particleSystem = m_particles;
            switch (systemType)
            {
                case SparksType.SPARKS_ADDITIVE_DEFAULT:
                    particleSystem = m_particles;
                    break;
                case SparksType.VEGETABLE_BLOOD:
                    particleSystem = m_greenBloodParticles;
                    break;
            }
            if (particleSystem == null)
            {
                particleSystem = InitializeParticles(systemType);
            }
            if (!particleSystem.gameObject.activeSelf)
            {
                particleSystem.gameObject.SetActive(true);
            }
            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
            {
                position = position,
                velocity = direction,
                startSize = ((startSize == null) ? particleSystem.startSize : startSize.Value),
                startLifetime = ((startLifetime == null) ? particleSystem.startLifetime : startLifetime.Value),
                startColor = ((startColor == null) ? particleSystem.startColor : startColor.Value),
                randomSeed = (uint)UnityEngine.Random.Range(1, 1000)
            };
            particleSystem.Emit(emitParams, 1);
        }

        public static void DoRandomParticleBurst(int num, Vector3 minPosition, Vector3 maxPosition, Vector3 direction, float angleVariance, float magnitudeVariance, float? startSize = null, float? startLifetime = null, Color? startColor = null, SparksType systemType = OddSparksDoer.SparksType.SPARKS_ADDITIVE_DEFAULT)
        {
            for (int i = 0; i < num; i++)
            {
                Vector3 position = new Vector3(UnityEngine.Random.Range(minPosition.x, maxPosition.x), Random.Range(minPosition.y, maxPosition.y), Random.Range(minPosition.z, maxPosition.z));
                Vector3 direction2 = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-angleVariance, angleVariance)) * (direction.normalized * Random.Range(direction.magnitude - magnitudeVariance, direction.magnitude + magnitudeVariance));
                DoSingleParticle(position, direction2, startSize, startLifetime, startColor, systemType);
            }
        }

        public static void DoLinearParticleBurst(int num, Vector3 minPosition, Vector3 maxPosition, float angleVariance, float baseMagnitude, float magnitudeVariance, float? startSize = null, float? startLifetime = null, Color? startColor = null, SparksType systemType = SparksType.SPARKS_ADDITIVE_DEFAULT)
        {
            for (int i = 0; i < num; i++)
            {
                Vector3 position = Vector3.Lerp(minPosition, maxPosition, ((float)i + 1f) / (float)num);
                Vector3 vector = UnityEngine.Random.insideUnitCircle.normalized.ToVector3ZUp(0f);
                Vector3 direction = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-angleVariance, angleVariance)) * (vector.normalized * UnityEngine.Random.Range(baseMagnitude - magnitudeVariance, vector.magnitude + magnitudeVariance));
                DoSingleParticle(position, direction, startSize, startLifetime, startColor, systemType);
            }
        }

        public static void DoRadialParticleBurst(int num, Vector3 minPosition, Vector3 maxPosition, float angleVariance, float baseMagnitude, float magnitudeVariance, float? startSize = null, float? startLifetime = null, Color? startColor = null, SparksType systemType = SparksType.SPARKS_ADDITIVE_DEFAULT)
        {
            for (int i = 0; i < num; i++)
            {
                Vector3 vector = new Vector3(UnityEngine.Random.Range(minPosition.x, maxPosition.x), UnityEngine.Random.Range(minPosition.y, maxPosition.y), UnityEngine.Random.Range(minPosition.z, maxPosition.z));
                Vector3 vector2 = vector - (maxPosition + minPosition) / 2f;
                Vector3 direction = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-angleVariance, angleVariance)) * (vector2.normalized * UnityEngine.Random.Range(baseMagnitude - magnitudeVariance, vector2.magnitude + magnitudeVariance));
                DoSingleParticle(vector, direction, startSize, startLifetime, startColor, systemType);
            }
        }

        public static void EmitFromRegion(EmitRegionStyle emitStyle, float numPerSecond, float duration, Vector3 minPosition, Vector3 maxPosition, Vector3 direction, float angleVariance, float magnitudeVariance, float? startSize = null, float? startLifetime = null, Color? startColor = null, SparksType systemType = SparksType.SPARKS_ADDITIVE_DEFAULT)
        {
            GameUIRoot.Instance.StartCoroutine(HandleEmitFromRegion(emitStyle, numPerSecond, duration, minPosition, maxPosition, direction, angleVariance, magnitudeVariance, startSize, startLifetime, startColor, systemType));
        }

        private static IEnumerator HandleEmitFromRegion(EmitRegionStyle emitStyle, float numPerSecond, float duration, Vector3 minPosition, Vector3 maxPosition, Vector3 direction, float angleVariance, float magnitudeVariance, float? startSize = null, float? startLifetime = null, Color? startColor = null, SparksType systemType = SparksType.SPARKS_ADDITIVE_DEFAULT)
        {
            float elapsed = 0f;
            float numReqToSpawn = 0f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                numReqToSpawn += numPerSecond * BraveTime.DeltaTime;
                if (numReqToSpawn > 1f)
                {
                    int num = Mathf.FloorToInt(numReqToSpawn);
                    if (emitStyle != EmitRegionStyle.RANDOM)
                    {
                        if (emitStyle == EmitRegionStyle.RADIAL)
                        {
                            DoRadialParticleBurst(num, minPosition, maxPosition, angleVariance, direction.magnitude, magnitudeVariance, startSize, startLifetime, startColor, systemType);
                        }
                    }
                    else
                    {
                        DoRandomParticleBurst(num, minPosition, maxPosition, direction, angleVariance, magnitudeVariance, startSize, startLifetime, startColor, systemType);
                    }
                }
                numReqToSpawn %= 1f;
                yield return null;
            }
            yield break;
        }

        private static ParticleSystem InitializeParticles(SparksType targetType)
        {
            switch (targetType)
            {
                default:
                case SparksType.SPARKS_ADDITIVE_DEFAULT:
                    m_particles = ((GameObject)Object.Instantiate(ResourceCache.Acquire("Global VFX/SparkSystem"), Vector3.zero, Quaternion.identity)).GetComponent<ParticleSystem>();
                    return m_particles;
                case SparksType.VEGETABLE_BLOOD:
                    m_greenBloodParticles = (Object.Instantiate(greenBlood, Vector3.zero, Quaternion.identity)).GetComponent<ParticleSystem>();
                    return m_greenBloodParticles;
            }
        }

        public static void CleanupOnSceneTransition()
        {
            m_particles = null;
            m_greenBloodParticles = null;
        }

        private static ParticleSystem m_particles;

        private static ParticleSystem m_greenBloodParticles;
        public enum EmitRegionStyle
        {
            RANDOM,
            RADIAL
        }

        public enum SparksType
        {
            SPARKS_ADDITIVE_DEFAULT,
            VEGETABLE_BLOOD,
            
        }

        public static void InitPrefabs()
        {
            greenBlood = FakePrefab.Clone((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/BloodSystem"), Vector3.zero, Quaternion.identity));
            greenBlood.GetComponent<ParticleSystem>().startColor = new Color(0, 0.149f, 0);
            greenBlood.GetComponentsInChildren<ParticleSystem>()[1].startColor = new Color(0, 0.12f, 0);
        }

        public static GameObject greenBlood;
    }
}
