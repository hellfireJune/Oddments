using Alexandria.ItemAPI;
using Alexandria.Misc;
using Alexandria.PrefabAPI;
using BindingAPI;
using UnityEngine;

namespace Oddments
{
    public abstract class ShrapnelAbilityBase : PassiveItem
    {
        public enum ShrapnelChargeType
        {
            ROOMS,
            DAMAGE,
            TIMED,
        }

        public float cooldown;
        public float maxCooldown;
        public ShrapnelChargeType type;

        private static readonly string m_chargebarSprite = $"{Module.ASSEMBLY_NAME}/Resources/example_item_sprite";
        private static GameObject m_chargebarPrefab;
        private static Vector2 m_chargebarDimensions = new Vector2(20, 4);

        public static void InitSetupStuffYaddaYadda()
        {
            BindingBuilder.Init("oddmentsbindings");
            ETGModMainBehaviour.Instance.gameObject.AddComponent<BindingHandler>();
            CustomActions.OnRunStart += RunStart;

            m_chargebarPrefab = PrefabBuilder.BuildObject("Shrapnel Giant Chargebar");
            //FakePrefab.MarkAsFakePrefab(m_chargebarObject);
            int spriteID = SpriteBuilder.AddSpriteToCollection(m_chargebarSprite, ETGMod.Databases.Items.ProjectileCollection);
            tk2dTiledSprite tiledSprite = m_chargebarPrefab.gameObject.GetOrAddComponent<tk2dTiledSprite>();

            tiledSprite.SetSprite(ETGMod.Databases.Items.ProjectileCollection, spriteID);
            tk2dSpriteDefinition def = tiledSprite.GetCurrentSpriteDef();
            def.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter);/*

            tk2dSpriteAnimator animator = m_chargebarPrefab.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = m_chargebarPrefab.GetOrAddComponent<tk2dSpriteAnimation>();
            animation.clips = new tk2dSpriteAnimationClip[0];
            animator.Library = animation;
            Destroy(m_chargebarPrefab.GetComponentInChildren<tk2dSprite>());*/
        }

        public GameObject chargeBarReal;

        protected override void Update()
        {
            base.Update();
            if (Owner)
            {
                if (type == ShrapnelChargeType.TIMED)
                {
                    cooldown = Mathf.Max(0, cooldown - BraveTime.DeltaTime);
                }

                if (chargeBarReal == null)
                {
                    chargeBarReal = Instantiate(m_chargebarPrefab, Owner.specRigidbody.UnitBottomCenter, Quaternion.identity, Owner.transform);
                }
                tk2dTiledSprite tiledSprite = chargeBarReal.gameObject.GetOrAddComponent<tk2dTiledSprite>();
                tiledSprite.anchor = tk2dBaseSprite.Anchor.UpperCenter;
                tiledSprite.dimensions = new Vector2(Mathf.FloorToInt(m_chargebarDimensions.x * (cooldown / maxCooldown)), m_chargebarDimensions.y);
            }
            else if (chargeBarReal != null)
            {
                Destroy(chargeBarReal);
            }
        }

        public override void Pickup(PlayerController player)
        {
            if (m_pickedUpThisRun == false)
            {
                cooldown = 0;
            }
            base.Pickup(player);
        }

        public abstract void Effect();

        public bool SetOnCooldown()
        {
            ETGModConsole.Log(cooldown);
            if (cooldown <= 0)
            {
                cooldown = maxCooldown;
                return true;
            }
            return false;
        }


        public static void RunStart(PlayerController player)
        {
            player.OnAnyEnemyReceivedDamage += OnAnyEnemyDamaged;
        }

        public static void OnAnyEnemyDamaged(float damageDone, bool fatal, HealthHaver target)
        {
            AIActor aiactor = (!target) ? null : target.aiActor;
            if (aiactor && !aiactor.IsNormalEnemy)
            {
                return;
            }

            PlayerController player = GameManager.Instance.PrimaryPlayer; //https://discord.com/channels/998556124250898523/998556125106557020/1005333323901579265 pls an3s
            if (!player.IsGhost && !target.PreventCooldownGainFromDamage)
            {

                foreach (PassiveItem item in GameManager.Instance.PrimaryPlayer.passiveItems)
                {
                    if (item is ShrapnelAbilityBase)
                    {
                        ShrapnelAbilityBase abilityBase = (ShrapnelAbilityBase)item;
                        if (abilityBase != null && abilityBase.type == ShrapnelChargeType.DAMAGE)
                        {
                            abilityBase.cooldown = Mathf.Max(0, abilityBase.cooldown - damageDone);
                        }
                    }
                }
            }
        }

        public class BindingHandler : MonoBehaviour
        {

            public InControl.PlayerAction bindingAction;
            public void Start()
            {
                bindingAction = BindingBuilder.CreateBinding("Giant Ability", InControl.Key.F);

            }

            public void Update()
            {
                if (bindingAction.WasPressed)
                {
                    ETGModConsole.Log("pressed");
                    foreach (PassiveItem item in GameManager.Instance.PrimaryPlayer.passiveItems) //an3s ples
                    {
                        if (item is ShrapnelAbilityBase)
                        {
                            ShrapnelAbilityBase abilityBase = (ShrapnelAbilityBase)item;
                            if (abilityBase.SetOnCooldown())
                            {
                                abilityBase.Effect();
                            }
                        }
                    }
                }
            }
        }
    }
}
