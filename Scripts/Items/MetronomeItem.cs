using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using SaveAPI;

namespace Oddments
{
    internal class MetronomeItem : PlayerItem
    {
        public static OddItemTemplate template = new OddItemTemplate(typeof(MetronomeItem))
        {
            Name = "Assist Trophy",
            Description = "What's inside?",
            Quality = ItemQuality.C,
            SpriteResource = $"{Module.SPRITE_PATH}/assisttrophy.png",
            LongDescription = $"Gives the user a random gun for {m_duration} seconds",
            AltLongDesc = $"Gives the user a random gun for {m_duration} seconds\n\nThe possibilities are endles...",

            PostInitAction = item =>
            {
                item.SetupUnlockOnEncounter(PickupObjectDatabase.GetById(734)?.encounterTrackable?.EncounterGuid ?? "boogernuts", 0, DungeonPrerequisite.PrerequisiteOperation.GREATER_THAN);
                item.AddUnlockText("Find the Mimic Gun.");
            }
        };

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            this.StartCoroutine(ItemBuilder.HandleDuration(this, m_duration, user, player => { }));

            int gunId = PickupObjectDatabase.GetRandomGun().PickupObjectId;
            GunFormeSynergyProcessor.AssignTemporaryOverrideGun(user, gunId, m_duration);
        }

        public override bool CanBeUsed(PlayerController user)
        {
            if (user.inventory.GunLocked.Value)
            {
                return false;
            }
            return base.CanBeUsed(user);
        }

        static float m_duration = 12f;
    }
}
