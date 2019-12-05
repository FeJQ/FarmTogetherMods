using Harmony12;
using Logic.Events;
using Logic.Farm;
using Logic.Mode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityModManagerNet;

namespace EventItemsCanBeHarvestedByFarmhand
{
    public class Settings : UnityModManager.ModSettings
    {
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
    public class MainMod
    {
        public static bool isEnable;
        public static Settings settings;
        public static UnityModManager.ModEntry.ModLogger logger;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            isEnable = true;
            //读取用户配置文件
            settings = Settings.Load<Settings>(modEntry);
            //储存日志输出对象
            logger = modEntry.Logger;
            //logger.Log("Load");
           // modEntry.OnGUI = OnGUI;
           // modEntry.OnSaveGUI = OnSaveGUI;
            //modEntry.OnToggle = OnToggle;           
            //将更改应用到游戏上
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            //logger.Log(trainScriptType.Name.ToString());
            return true;
        }

        [HarmonyPatch(typeof(FarmData), "ItemMustBeHarvestedByPlayer")]
        public class FarmData_ItemMustBeHarvestedByPlayer_Patch
        {
            [HarmonyPrefix]
            public static bool ItemMustBeHarvestedByPlayer(HarvestItemDefinition harvestItem,bool __result)
            {
                if (!isEnable)
                {
                    return true;
                }
                __result = false;
                return false;
            }
        }
    }
}
