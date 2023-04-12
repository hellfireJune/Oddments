using JuneLib.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oddments
{
    public class MassDevolveItem : PlayerItem
    {
        public static List<DevolverTier> devolvertier = new List<DevolverTier>();
        public static List<string> EnemyGuidsToIgnore = new List<string>();

        public static OddItemTemplate template = new OddItemTemplate(typeof(MassDevolveItem))
        {
            PostInitAction = item =>
            {
                ComplexProjectileModifier modifier = PickupObjectDatabase.GetById(638) as ComplexProjectileModifier;
                DevolverModifier mod = modifier.DevolverSourceModifier;

                devolvertier = mod.DevolverHierarchy;
                EnemyGuidsToIgnore = mod.EnemyGuidsToIgnore;
            }
        };

        public override bool CanBeUsed(PlayerController user)
		{
			if (!user || user.CurrentRoom is null) { return false; }
			List<AIActor> enemies = user.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All);
			if (enemies == null || enemies.Count == 0) { return false; }
			foreach (AIActor aiactor in enemies)
			{
				if (aiactor && aiactor.healthHaver.IsVulnerable)
				{
					return base.CanBeUsed(user);
				}
			}
			return false;
        }

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
			if (!user || user.CurrentRoom is null) { return; }
			List<AIActor> enemies = user.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All);

			foreach (AIActor aiactor in enemies) 
			{
				if (aiactor && aiactor.healthHaver.IsVulnerable)
                {
					Transmogrify(aiactor);
                }
			}
		}

        public static void Transmogrify(AIActor aiActor)
        {
			string enemyGuid = aiActor.EnemyGuid;
			for (int i = 0; i < EnemyGuidsToIgnore.Count; i++)
			{
				if (EnemyGuidsToIgnore[i] == enemyGuid)
				{
					return;
				}
			}
			int num = devolvertier.Count - 1;
			for (int j = 0; j < devolvertier.Count; j++)
			{
				List<string> tierGuids = devolvertier[j].tierGuids;
				for (int k = 0; k < tierGuids.Count; k++)
				{
					if (tierGuids[k] == enemyGuid)
					{
						num = j - 1;
						break;
					}
				}
			}
			if (num >= 0 && num < devolvertier.Count)
			{
				List<string> tierGuids2 = devolvertier[num].tierGuids;
				string guid = tierGuids2[UnityEngine.Random.Range(0, tierGuids2.Count)];
				aiActor.Transmogrify(EnemyDatabase.GetOrLoadByGuid(guid), (GameObject)ResourceCache.Acquire("Global VFX/VFX_Item_Spawn_Poof"));
				AkSoundEngine.PostEvent("Play_WPN_devolver_morph_01", aiActor.gameObject);
			}
		}
    }
}
