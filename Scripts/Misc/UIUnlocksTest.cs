using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.PrefabAPI;
using Alexandria.ItemAPI;

namespace Oddments
{
    public class UIUnlocksTest : MonoBehaviour
    {
        public static GameObject unlockMenuPrefab;
        public static GameObject unlockMenu_Root;
        public static GameObject unlockMenu_Core;
        public static GameObject unlockMenu_Back;
        public static GameObject unlockMenu_Camera;
        public static void Init()
        {
            unlockMenuPrefab = PrefabBuilder.BuildObject("Unlock Menu");
            unlockMenuPrefab.transform.localPosition = new Vector3(-1000, -1000);

            unlockMenu_Root = PrefabBuilder.BuildObject("Unlock Menu_Root");
            unlockMenu_Root.transform.parent = unlockMenuPrefab.transform;
            dfGUIManager guiManager = unlockMenu_Root.AddComponent<dfGUIManager>();
            dfInputManager dfInput = unlockMenu_Root.AddComponent<dfInputManager>();
            dfInput.UseMouse = false; // keeping this until it bites me in the heinie.
            unlockMenu_Root.AddComponent<InControlInputAdapter>();

            unlockMenu_Core = PrefabBuilder.BuildObject("Core");
            unlockMenu_Core.transform.parent = unlockMenu_Root.transform;
            unlockMenu_Core.AddComponent<dfPanel>();

            unlockMenu_Camera = PrefabBuilder.BuildObject("Camera");
            Camera camera = unlockMenu_Camera.AddComponent<Camera>();
            dfGUICamera dfGUICamera = unlockMenu_Camera.AddComponent<dfGUICamera>();
            guiManager.RenderCamera = camera;
            unlockMenu_Camera.transform.parent = unlockMenu_Root.transform;

            unlockMenu_Back = PrefabBuilder.BuildObject("Bottom");
            unlockMenu_Back.transform.parent = unlockMenu_Core.transform;
            dfTextureSprite backSprite = unlockMenuPrefab.gameObject.AddComponent<dfTextureSprite>();
            ResourceExtractor.GetTextureFromResource($"{Module.ASSEMBLY_NAME}/Resources/you.png");

            ETGModConsole.Commands.GetGroup("oddments").AddUnit("unlockstest", action: args =>
            {
                ETGModGUI.CurrentMenu = ETGModGUI.MenuOpened.None;

                if (!unlockMenu)
                {
                    unlockMenu = Instantiate(unlockMenuPrefab);
                } else
                {
                    unlockMenu.GetComponentInChildren<dfGUIManager>().enabled ^= true;
                }
            });
        }

        public static GameObject unlockMenu;
    }
}
