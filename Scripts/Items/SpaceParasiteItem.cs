using Dungeonator;
using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oddments
{
    public class SpaceParasiteItem : PassiveItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(SpaceParasiteItem))
        {
            Name = "Space Slug",
            Quality = ItemQuality.C,
            Description = "Symbiotic Friend",
            SpriteResource = $"{Module.SPRITE_PATH}/spaceslug.png",
            LongDescription = "Increases the power of any gun you kill a boss with\n\nA parasite whose diet consists solely of boss monsters and whose waste consists of damage ups"
        };

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnRoomClearEvent += OnClearBossRoom;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);
            player.OnRoomClearEvent -= OnClearBossRoom;
        }

        private void OnClearBossRoom(PlayerController player)
        {
            RoomHandler room = player.CurrentRoom;
            if (room == null || room.area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.BOSS) { return; }
            player.CurrentGun.gameObject.GetOrAddComponent<SpaceParasiteGunBehaviour>().DMGMod += 0.1f;
        }
        public class SpaceParasiteGunBehaviour : GunBehaviour
        {
            public SpaceParasiteGunBehaviour()
            {
                DMGMod = 1f;
            }

            public float DMGMod;
            public override void PostProcessProjectile(Projectile projectile)
            {
                projectile.baseData.damage *= DMGMod;
                projectile.RuntimeUpdateScale(1 + (DMGMod / 1.5f));
                base.PostProcessProjectile(projectile);
            }

            public override void PostProcessBeam(BeamController beam)
            {
                beam.DamageModifier *= DMGMod;
                base.PostProcessBeam(beam);
            }
        }
    }
}
