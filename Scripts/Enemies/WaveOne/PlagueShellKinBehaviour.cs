using Alexandria.EnemyAPI;
using System.Collections.Generic;
using UnityEngine;

namespace Oddments
{
    public class PlagueShellKinBehaviour : BraveBehaviour
    {
        private const string guid = "odmnts_plague_shotgun";
        public static void Init()
        {
            GameObject prefab = EnemyBuilder.BuildPrefab("Plague Doctor", guid, $"{Module.ASSEMBLY_NAME}/Resources/Sprites/Enemies/Placeholder/iloveweezer.png", new IntVector2(0, 0), new IntVector2(8, 9), true);
            var enemy = prefab.AddComponent<PlagueShellKinBehaviour>();

            AIActor actor = enemy.aiActor;
            actor.knockbackDoer.weight = 200;
            actor.MovementSpeed = 1f;
            actor.healthHaver.PreventAllDamage = false;
            actor.CollisionDamage = 1f;
            actor.HasShadow = false;
            actor.IgnoreForRoomClear = false;
            actor.aiAnimator.HitReactChance = 0f;
            actor.specRigidbody.CollideWithOthers = true;
            actor.specRigidbody.CollideWithTileMap = true;
            actor.PreventFallingInPitsEver = false;
            actor.healthHaver.ForceSetCurrentHealth(55f);
            actor.CollisionKnockbackStrength = 0f;
            actor.procedurallyOutlined = true;
            actor.CanTargetPlayers = true;
            actor.healthHaver.SetHealthMaximum(55f, null, false);

            actor.CorpseObject = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5").CorpseObject;
            GameObject shootpoint = EnemyBuildingTools.GenerateShootPoint(prefab, new Vector2(0.5f, 0.5f), "Gootpoint");

            BehaviorSpeculator spec = prefab.GetComponent<BehaviorSpeculator>();
            spec.TargetBehaviors = new List<TargetBehaviorBase>()
            {
                new TargetPlayerBehavior()
                {
					/*
					Radius = 35f,
					LineOfSight = true,
					ObjectPermanence = true,
					SearchInterval = 0.25f,
					PauseOnTargetSwitch = false,
					PauseTime = 0.25f,*/
                }
            };
            spec.MovementBehaviors = new List<MovementBehaviorBase>()
            {

                new SeekTargetBehavior
                {
					/*ExternalCooldownSource = true,
					StopWhenInRange = true,
					CustomRange = 5f,
					LineOfSight = false,
					ReturnToSpawn = false,
					SpawnTetherDistance = 0f,
					PathInterval = 0.125f,
					SpecifyRange = false,
					MinActiveRange = 0f,
					MaxActiveRange = 0f*/
				}
            };
            spec.AttackBehaviorGroup.AttackBehaviors = new List<AttackBehaviorGroup.AttackGroupItem>()
            {
                new AttackBehaviorGroup.AttackGroupItem()
                {
                    NickName = "Shoot",
                    Probability = 1f,
                    Behavior = new ShootGunBehavior() {/*
                        GroupCooldownVariance = 0.2f,
                        LineOfSight = true,
                        WeaponType = WeaponType.BulletScript,
                        BulletScript = new CustomBulletScriptSelector(typeof(RevolverOneScript)),
                        FixTargetDuringAttack = true,
                        StopDuringAttack = false,
                        LeadAmount = 0.6f,
                        LeadChance = 1,
                        RespectReload = true,
                        MagazineCapacity = 3,
                        ReloadSpeed = 3f,
                        EmptiesClip = true,
                        SuppressReloadAnim = false,
                        TimeBetweenShots = 0.3f,
                        PreventTargetSwitching = true,
                        OverrideBulletName = StaticUndodgeableBulletEntries.undodgeableSniper.Name,
                        OverrideAnimation = null,
                        OverrideDirectionalAnimation = null,
                        HideGun = false,
                        UseLaserSight = false,
                        UseGreenLaser = false,
                        PreFireLaserTime = -1,
                        AimAtFacingDirectionWhenSafe = false,
                        Cooldown = 2f,
                        CooldownVariance = 0,
                        AttackCooldown = 0,
                        GlobalCooldown = 0,
                        InitialCooldown = 0,
                        InitialCooldownVariance = 0,
                        GroupName = null,
                        GroupCooldown = 0,
                        MinRange = 0,
                        Range = 16,
                        MinWallDistance = 0,
                        MaxEnemiesInRoom = 0,
                        MinHealthThreshold = 0,
                        MaxHealthThreshold = 1,
                        HealthThresholds = new float[0],
                        AccumulateHealthThresholds = true,
                        targetAreaStyle = null,
                        resetCooldownOnDamage = null,
                        RequiresLineOfSight = true,
                        MaxUsages = 0,*/
                    },
                },
            };

            AIShooter shooter = prefab.GetComponent<AIShooter>();
        }
    }
}
