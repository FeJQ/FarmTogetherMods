using Harmony12;
using Logic.Farm;
using Logic.Farm.Buildings;
using Milkstone.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UnityModManagerNet;

namespace CustomFarmhandWorkRadius
{
    //public class Config
    //{
    //    [JsonProperty("Radius")]
    //    public int radius;
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
        public static Assembly asm = Assembly.Load("Assembly-CSharp");
        public static Type farmhandScriptType = asm.GetType("LocalFarmhandScript");


        // public static Config config;
        public static bool isEnable;
        public static Settings settings;
        public static UnityModManager.ModEntry.ModLogger logger;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            isEnable = false;
            //读取用户配置文件
            settings = Settings.Load<Settings>(modEntry);
            //储存日志输出对象
            logger = modEntry.Logger;
            //logger.Log("Load");
            //modEntry.OnGUI = OnGUI;
            //modEntry.OnSaveGUI = OnSaveGUI;
            //modEntry.OnToggle = OnToggle;           
            //将更改应用到游戏上
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            //logger.Log(trainScriptType.Name.ToString());

            MethodInfo original = farmhandScriptType.GetMethod("GetNextWorkTile", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo prefix = typeof(MainMod).GetMethod("LocalFarmhandScript_GetNextWorkTile_Transpiler");
            HarmonyMethod transpiler = new HarmonyMethod(prefix);
            harmony.Patch(original, null, null, transpiler);

            return isEnable;
        }

        
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> LocalFarmhandScript_GetNextWorkTile_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            logger.Log("codes");

            int i = 0;

            int radius = 10;
            int _radius = -10;
            
        

            

            
           //foreach (var code in codes)
           //{
           //    logger.Log(i.ToString() + " " + code.opcode + " " + code.operand);
           //    i++;
           //}
           // codes[16] = new CodeInstruction(OpCodes.Ldc_I4_S, -10);
           // codes[19] = new CodeInstruction(OpCodes.Ldc_I4_S, 10);
           // codes[52] = new CodeInstruction(OpCodes.Ldc_I4_S, -10);
           // codes[59] = new CodeInstruction(OpCodes.Ldc_I4_S, 10);  
           // logger.Log("-------");
           // i = 0;
            //foreach (var code in codes)
            //{
            //    logger.Log(i.ToString() + " " + code.opcode + " " + code.operand);
            //    i++;
            //}
            return codes.AsEnumerable();

        }

        //[HarmonyPatch]
        //public class SelectedTiles_Awake_Patch
        //{
        //    private static MethodBase TargetMethod(HarmonyInstance instance)
        //    {
        //        return AccessTools.Method("LocalFarmhandScript:GetNextWorkTile");
        //    }
        //
        //    public static void Prefix(out FarmTileId selectedTile,object __instance,bool __result)
        //    {
        //        selectedTile = FarmTileId.Zero;
        //        int maxValue = int.MaxValue;
        //        int num = maxValue;
        //        Vector3 position= Traverse.Create(__instance).Field("transform").GetValue<Transform>().position;
        //        FarmTileId posToTile = FarmData.GetPosToTile(position);
        //        FarmData farm= Traverse.Create(__instance).Field("building").GetValue<Building>().FarmData;
        //        FarmData farmData = farm;
        //        for (int i = -15; i <= 15; i++)
        //        {
        //            for (int j = 15; j >= -15; j--)
        //            {
        //                FarmTileId TopLeftTile = Traverse.Create(__instance).Field("building").GetValue<Building>().TopLeftTile;
        //                FarmTileId farmTileId = TopLeftTile + new Int2(i, j);
        //                int num2 = FarmTileId.ManhattanDistance(posToTile, farmTileId);
        //                WorkType workType;
        //                if (num2 < num && FarmhandScript.CanWorkTile(farmTileId, farmData, out workType))
        //                {
        //                    selectedTile = farmTileId;
        //                    num = num2;
        //                }
        //            }
        //        }
        //        __result= num < maxValue;
        //    }
        //}
    }
}
