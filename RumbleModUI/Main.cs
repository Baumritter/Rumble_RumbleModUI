using MelonLoader;
using UnityEngine;

namespace RumbleModUI
{
    public static class BuildInfo
    {
        public const string ModName = "ModUI";
        public const string ModVersion = "1.2.0";
        public const string Description = "Adds a universal UI for Mod Creators";
        public const string Author = "Baumritter";
        public const string Company = "";
    }
    public class RumbleModUIClass : MelonMod
    {
        //constants

        //variables

        //objects
        General.Delay Delay = new General.Delay { name = "Delay" };
        public static UI UI_Obj = new UI();   
        
        //Run every update
        public override void OnUpdate()
        {
            //Base Updates
            base.OnUpdate();

            if (!UI_Obj.GetInit() && Delay.Done) UI_Obj.InitUI("Mod_Setting_UI");

            if (Input.GetKeyDown(KeyCode.F10))
            {
                if (UI_Obj.IsUIVisible())
                {
                    UI_Obj.HideUI();
                }
                else
                {
                    UI_Obj.ShowUI();
                }
            }

        }

        //Overrides
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);
            Delay.Delay_Start(3);
        }
    }
}

