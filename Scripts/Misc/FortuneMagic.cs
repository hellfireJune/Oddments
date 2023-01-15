using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexandria.ItemAPI;
using Alexandria.PrefabAPI;
using UnityEngine;

namespace Oddments
{
    public static class FortuneMagic
    {
        private static GameObject m_fortuneContainerPrefab;
        private static readonly string m_paperSprite = $"{Module.ASSEMBLY_NAME}/Resources/example_item_sprite";
        public static List<string> Fortunes { get; private set; }
        public static void Init()
        {
            byte[] data = ResourceExtractor.ExtractEmbeddedResource($"{Module.ASSEMBLY_NAME}/Resources/Misc/fortunes.txt");
            //https://stackoverflow.com/questions/1003275/how-to-convert-utf-8-byte-to-string
            string result = Encoding.UTF8.GetString(data);
            Fortunes = result.Split(new string[1] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            for (int i = 0; i < 3; i++)
            {
                ETGModConsole.Log(BraveUtility.RandomElement(Fortunes).ToUpper());
            }

            m_fortuneContainerPrefab = PrefabBuilder.BuildObject("Fortune Container");
            //FakePrefab.MarkAsFakePrefab(m_chargebarObject);
            int spriteID = SpriteBuilder.AddSpriteToCollection(m_paperSprite, ETGMod.Databases.Items.ProjectileCollection);
            tk2dTiledSprite tiledSprite = m_fortuneContainerPrefab.gameObject.GetOrAddComponent<tk2dTiledSprite>();

            tiledSprite.SetSprite(ETGMod.Databases.Items.ProjectileCollection, spriteID);
            tk2dSpriteDefinition def = tiledSprite.GetCurrentSpriteDef();
            def.ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter);

            ETGModConsole.Commands.GetGroup("oddments").AddUnit("showfortune", args =>
            {
                Vector3 pos = GameManager.Instance.PrimaryPlayer.specRigidbody.UnitCenter;
                MakeFortune(pos);
            });
        }

        public static void MakeFortune(Vector3 position)
        {
            string fortune = BraveUtility.RandomElement(Fortunes).ToUpper();
            if (m_inactiveFortuneContainers.Count <= 0)
            {
                GameObject containerObj = UnityEngine.Object.Instantiate(m_fortuneContainerPrefab, GameUIRoot.Instance.transform);
                UnityEngine.Object.Instantiate(BraveResources.Load("DamagePopupLabel", ".prefab"), containerObj.transform);
                tk2dTiledSprite sprite = containerObj.GetComponent<tk2dTiledSprite>();
                //sprite.AttachRenderer(GameUIRoot.Instance.spri);
                m_inactiveFortuneContainers.Add(containerObj);
            }

            GameObject container = m_inactiveFortuneContainers[0];
            container.transform.localPosition = new Vector3(0, 0.5f, 0);
            dfLabel text = container.GetComponentInChildren<dfLabel>();
            text.text = fortune;
            text.Anchor = dfAnchorStyle.CenterHorizontal;
            text.transform.localPosition = new Vector3(0, 0, 0);
            container.SetActive(true);
            
        }
        private static List<GameObject> m_inactiveFortuneContainers = new List<GameObject>();
    }
}
