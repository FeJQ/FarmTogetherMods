using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;
using Harmony12;
using Logic.Farm;
using Logic.Farm.House;
using Logic.Mode;

namespace UnlockDLCItems
{
    //public class Building
    //{
    //    [JsonProperty("Buildings")]
    //    public List<struct_building> buildings;
    //}
    //public struct struct_building
    //{
    //    [JsonProperty("ID")]
    //    public string ID;
    //
    //    [JsonProperty("limit")]
    //    public uint limit;
    //
    //    [JsonProperty("isEnable")]
    //    public bool isEnable;
    //}

    public class Settings : UnityModManager.ModSettings
    {

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
    public class MainMod
    {
        //public static Building  building;
        public static bool isEnable;
        public static Settings settings;
        public static UnityModManager.ModEntry.ModLogger logger;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            isEnable = true;
            //logger.Log("Hello");
            //读取用户配置文件
            settings = Settings.Load<Settings>(modEntry);
            //储存日志输出对象
            logger = modEntry.Logger;
            //logger.Log("Load");
            modEntry.OnGUI = OnGUI;
            //modEntry.OnSaveGUI = OnSaveGUI;
            //modEntry.OnToggle = OnToggle;           
            //将更改应用到游戏上
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            return true;
        }

        /// <summary>
        /// //当用户打开mod配置的时候调用
        /// </summary>
        /// <param name="modEntry"></param>
        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label("启动成功!");

        }

        /// <summary>
        /// //当用户开关mod的时候调用
        /// </summary>
        /// <param name="modEntry"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool OnToggle(UnityModManager.ModEntry arg1, bool arg2)
        {
            isEnable = arg2;
            return true;
        }

        [HarmonyPatch(typeof(FarmManager), nameof(FarmManager.OwnsDLC))]
        [HarmonyPatch(new Type[] {
            typeof(GameDLCs),     
        },
            new ArgumentType[]{
            ArgumentType.Normal,       
        })]
        public class FarmManager_OwnsDLC_Patch
        {
            static bool Postfix(bool __result,GameDLCs dlc)
            {
                if (isEnable)
                    return true;
                else
                    return __result;
            }
        }
        [HarmonyPatch(typeof(FarmData), nameof(FarmData.CheckDLC))]
        public class FarmData_CheckDLC_Patch
        {
            static bool Postfix(bool __result,GameDLCs dlc)
            {
                if (isEnable)
                    return true;
                else
                    return __result;
            }
      
        }

        [HarmonyPatch(typeof(FarmData), nameof(FarmData.CheckOwnerDLC))]
        public class FarmData_CheckOwnerDLC_Patch
        {
            static bool Postfix(bool __result ,GameDLCs dlc)
            {
                if (isEnable)
                    return true;
                else
                    return __result;
            }
        }

    }
}


