using Dungeonator;
using System.Collections.Generic;
using System.Linq;
using JuneLib.Items;

namespace Oddments
{
    public class SiphonGem : PlayerItem
    {
        public static ItemTemplate template = new ItemTemplate(typeof(SiphonGem))
        {
            Name = "Siphon Item",
            Description = "Item Absoprtion",
            LongDescription = "Destroys any items and takes their power. Any stats within the items will be permanently applied to the player. Any active effects in the item will be stored in the book",
            SpriteResource = $"{Module.ASSEMBLY_NAME}/Resources/Sprites/siphonitem.png",
            Quality = ItemQuality.D,
            Cooldown = 325
        };

        public override bool CanBeUsed(PlayerController user)
        {
            if (user == null || user.CurrentRoom == null)
            {
                return false;
            }
            if (succedActives.Count > 0)
            {
                foreach (var succed in succedActives)
                {
                    if ((PickupObjectDatabase.GetById(succed) as PlayerItem).CanBeUsed(user))
                    {
                        return base.CanBeUsed(user);
                    }
                }
            }
            IPlayerInteractable iplayer = user.CurrentRoom?.GetNearestInteractable(user.specRigidbody.UnitCenter, 1f, user);
            if (iplayer is PlayerItem
                || iplayer is PassiveItem)
            {

                return base.CanBeUsed(user);
            }
            return false;
        }

        private List<int> succedActives = new List<int>();

        public override void MidGameSerialize(List<object> data)
        {
            base.MidGameSerialize(data);
            data.Add(succedActives);
        }

        public override void MidGameDeserialize(List<object> data)
        {
            base.MidGameDeserialize(data);
            succedActives = (List<int>)data[0];
        }

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);

            if (succedActives.Count > 0)
            {
                foreach (var succed in succedActives)
                {
                    PlayerItem active = Instantiate(PickupObjectDatabase.GetById(succed).gameObject).GetComponent<PlayerItem>();
                    if (active)
                    {
                        if (active.CanBeUsed(user))
                        {
                            active.Use(user, out _);
                            if (active.consumable)
                            {
                                succedActives.Remove(succed);
                            }
                        }
                        Destroy(active);
                    }
                }
            }
            PickupObject items = user.CurrentRoom.GetNearestInteractable(user.specRigidbody.UnitCenter, 1f, user) as PickupObject;
            if (items && (items is PlayerItem || items is PassiveItem))
            {
                List<StatModifier> stats;
                if (items is PlayerItem)
                {
                    PlayerItem playerItem = (PlayerItem)items;
                    stats = playerItem.passiveStatModifiers.ToList();
                    succedActives.Add(items.PickupObjectId);
                }
                else
                {
                    PassiveItem passiveItem = (PassiveItem)items;
                    stats = passiveItem.passiveStatModifiers.ToList();
                }
                LootEngine.DoDefaultPurplePoof(items.sprite.WorldCenter);
                Destroy(items.gameObject);
                RoomHandler.unassignedInteractableObjects.Remove((IPlayerInteractable)items);

                user.ownerlessStatModifiers.AddRange(stats);
                user.stats.RecalculateStats(user);
            }
        }
    }
}
