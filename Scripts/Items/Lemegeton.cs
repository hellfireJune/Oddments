using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Alexandria.ItemAPI;
using JuneLib.Items;

namespace Oddments
{
    public class Lemegeton : PlayerItem
    {
        public static readonly string lemegetonNonSummonableTag = "lemegeton_non_summonable";
        public static readonly List<int> nonSummonableItems = new List<int>()
        {
            131, //Utility Belt
            473, //Hidden Compartment
            133, //Backpack
            134, //Ammo belt
            219, //Old Knight's Shield
            222, //Old Knight's Helm
            493, //Briefcase of Cash
            255, //Bandana
        };

        public static PlayerOrbital orbitalPrefab;

        public static ItemTemplate template = new ItemTemplate(typeof(Lemegeton))
        {
            Name = "Lemegeton",
            Quality = ItemQuality.EXCLUDED,
            Cooldown = 325,
            PostInitAction = _ =>
            {
                foreach (var item in nonSummonableItems)
                {
                    PickupObjectDatabase.GetById(item).SetTag(lemegetonNonSummonableTag);
                }

                BuildPrefab(); //OrbitalUtility.MakeOrbital("Lemegeton Orbital", 30, 10, 3, PlayerOrbital.OrbitalMotionStyle.ORBIT_PLAYER_ALWAYS, 1, $"{Module.ASSEMBLY_NAME}/Resources/example_item_sprite", new Vector2(14, 14), Vector2.zero);
            }
        };
        public static void BuildPrefab()
        {
            if (orbitalPrefab != null) return;
            GameObject prefab = SpriteBuilder.SpriteFromResource($"{Module.ASSEMBLY_NAME}/Resources/example_item_sprite");
            prefab.name = "Brown Guon Orbital";
            var body = prefab.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(7, 13));
            body.CollideWithTileMap = false;
            body.CollideWithOthers = true;
            body.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;

            orbitalPrefab = prefab.AddComponent<PlayerOrbital>();
            orbitalPrefab.motionStyle = PlayerOrbital.OrbitalMotionStyle.ORBIT_PLAYER_ALWAYS;
            orbitalPrefab.shouldRotate = false;
            orbitalPrefab.orbitRadius = 2f;
            orbitalPrefab.orbitDegreesPerSecond = 20f;
            orbitalPrefab.SetOrbitalTier(0);

            GameObject.DontDestroyOnLoad(prefab);
            FakePrefab.MarkAsFakePrefab(prefab);
            prefab.SetActive(false);
        }

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);

            PassiveItem item;
            do
            {
                item = LootEngine.GetItemOfTypeAndQuality<PassiveItem>(quality, GameManager.Instance.RewardManager.ItemsLootTable, true);
            } while (item.HasTag(lemegetonNonSummonableTag));
            PlayerOrbital orbital = Instantiate(orbitalPrefab.gameObject).GetComponent<PlayerOrbital>();
            orbital.Initialize(user);
            PassiveItem pItem = RealFakeItemHelper.CreateFakeItem(item, user, orbital.gameObject.transform);
            //ETGModConsole.Log(pItem.PickupObjectId);

            pItem.renderer.enabled = true;
        }

        /*public static float timerToPreventLag = 1f;
        public override void Update()
        {
            base.Update();
            timerToPreventLag -= Time.deltaTime;
            if (timerToPreventLag < 0)
            {
                ETGModConsole.Log("- -- -"); timerToPreventLag = 10f;
                foreach (KeyValuePair<string, EncounteredObjectData> kv in GameStatsManager.Instance.m_encounteredTrackables.ToList())
                {
                    EncounteredObjectData encounteredObjectData = kv.Value;
                    if (encounteredObjectData.differentiator > 0)
                    {

                        ETGModConsole.Log(encounteredObjectData.differentiator);
                        PickupObject pickupObject = PickupObjectDatabase.Instance.Objects.Find(pickup => pickup && pickup.encounterTrackable != null && pickup.encounterTrackable.EncounterGuid == kv.Key);
                        if (pickupObject != null)
                        {
                            ETGModConsole.Log($"Id:  {pickupObject.PickupObjectId}");
                            if (!string.IsNullOrEmpty(pickupObject.name))
                            {
                                ETGModConsole.Log(pickupObject.name);
                            }
                        }
                    }
                }
            }
        }*/

    }
}
