using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;
using Harmony12;
using Logic.Farm;
using Logic.Farm.House;

namespace InfiniteItems
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

            //string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //StreamReader sr = new StreamReader(path+"\\config.json");

            //string json = sr.ReadToEnd();
            //building = JsonConvert.DeserializeObject<Building>(json);

            

            return true;
        }

        /// <summary>
        /// //当用户打开mod配置的时候调用
        /// </summary>
        /// <param name="modEntry"></param>
        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            //GUILayout.BeginVertical("BOX");
            //GUILayout.BeginHorizontal();

            GUILayout.Label("启动成功!");
            //logger.Log("0");
            //GUILayout.Label("guiLayout.label");
            //System.Windows.Forms.MessageBox.Show("msgBox");
            //Console.WriteLine("console.writeline");
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

        [HarmonyPatch(typeof(FarmData), "CanBuy")]
        [HarmonyPatch(new Type[] {
            typeof(PlayerScript),
            typeof(FarmItemDefinition),
            typeof(FarmMoney),
            typeof(FailedActionReason)
        },
            new ArgumentType[]{
            ArgumentType.Normal,
            ArgumentType.Normal,
            ArgumentType.Out,
            ArgumentType.Out
        })]
        public class FarmData_CanBuyFarmItemDefinition_Patch
        {
            [HarmonyPrefix]
            public static bool CanBuy(
                PlayerScript player,
                FarmItemDefinition definition,
                FarmData __instance,
                bool __result
                )
            {
                if(!isEnable)
                {                                     
                    return true;
                }
                #region
                //logger.Log(definition.);
                //for (int i = 0; i < building.buildings.Count; i++)
                //{
                //    if (building.buildings[i].isEnable)
                //    {
                //        if(definition.FullId== building.buildings[i].ID)
                //        {
                //            definition.ItemCountLimit = building.buildings[i].limit;
                //        }
                //    }
                //}
                #endregion
                {
                    //完全版
                    definition.ItemCountLimit = 99999;                   
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(FarmData), "CanBuy")]
        [HarmonyPatch(new Type[] {
            typeof(PlayerScript),
            typeof(HouseItemDefinition),
            typeof(HouseData),
            typeof(FarmMoney),
            typeof(FailedActionReason)
        },
            new ArgumentType[]{
            ArgumentType.Normal,
            ArgumentType.Normal,
            ArgumentType.Normal,
            ArgumentType.Out,
            ArgumentType.Out
        })]
        public class FarmData_CanBuyHouseItemDefinition_Patch
        {
            [HarmonyPrefix]
            public static bool CanBuy(
                PlayerScript player,
                FarmItemDefinition definition,
                FarmData __instance,
                bool __result
                )
            {
                if (!isEnable)
                {
                    return true;
                }
                {
                    //完全版
                    definition.ItemCountLimit = 99999;
                }
                return true;
            }
        }



    }
}


