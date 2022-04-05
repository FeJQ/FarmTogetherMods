using Harmony12;
using Logic.Farm;
using Logic.Farm.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityModManagerNet;

namespace PondMerged
{
    public class Settings : UnityModManager.ModSettings
    {
        /// <summary>
        /// 一块池塘最多的鱼数量(再多就要被分割开了)
        /// </summary>
        public  int maxTileCount = 10000;
        public  bool isEnable=true;
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
       
        public static Settings settings;
        public static UnityModManager.ModEntry.ModLogger logger;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
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

            MethodInfo method = typeof(FarmData).GetMethod("PlacePond");
            HarmonyMethod transpiler = new HarmonyMethod(typeof(MainMod).GetMethod("FarmData_PlacePond_Transpiler"));
            harmony.Patch(method, null, null, transpiler);

            return settings.isEnable;
        }
        [HarmonyPatch(typeof(FarmData), "CheckPond")]
        public class FarmData_CheckPond_Patch
        {
            [HarmonyPrefix]
            public static bool CheckPond(FarmTileId tile, Border border, PondDefinition definition, List<Pond> ponds, FarmData __instance)
            {
                if (__instance.GetFence(tile, border) != null)
                {
                    return false;
                }
                FarmTile tile2 = __instance.GetTile(tile + border.GetBorderOffset());
                if (tile2 == null)
                {
                    return false;
                }
                PondContents pondContents = tile2.Contents as PondContents;
                if (pondContents == null)
                {
                    return false;
                }
                if (ponds.Contains(pondContents.Pond))
                {
                    return false;
                }
                if (pondContents.Definition.Race != definition.Race && false)
                {
                    return false;
                }
                if (pondContents.Pond.Tiles.Count >= settings.maxTileCount)
                {
                    return false;
                }
                ponds.Add(pondContents.Pond);
                return false;
            }
        }
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> FarmData_PlacePond_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            codes[99] = new CodeInstruction(OpCodes.Ldc_I4, settings.maxTileCount);           
            return codes.AsEnumerable();
        }

        [HarmonyPatch(typeof(FarmData), "CheckMerge")]
        public class FarmData_CheckMerge_Patch
        {
            [HarmonyPrefix]
            private static bool CheckMerge(FarmTileId tile1, FarmTileId tile2, FarmData __instance)
            {
                if (!settings.isEnable)
                {                   
                    return true;
                }
                FarmTile tile3 = __instance.GetTile(tile1);
                if (tile3 == null || tile3.IsEmpty)
                {
                    return false;
                }
                FarmTile tile4 = __instance.GetTile(tile2);
                if (tile4 == null || tile4.IsEmpty)
                {
                    return false;
                }
                if (tile3.Contents.Category == tile4.Contents.Category)
                {
                    FarmTileContentsType category = tile3.Contents.Category;
                    if (category != FarmTileContentsType.Animal)
                    {
                        if (category == FarmTileContentsType.Pond)
                        {
                            PondContents pondContents = tile3.Contents as PondContents;
                            PondContents pondContents2 = tile4.Contents as PondContents;
                            if (pondContents.Pond == pondContents2.Pond)
                            {
                                return false;
                            }
                            if (pondContents.Pond.Tiles.Count + pondContents2.Pond.Tiles.Count > settings.maxTileCount)
                            {
                                return false;
                            }
                            pondContents2.Pond.Merge(pondContents.Pond);
                            tile3.StateChanged(true);
                            tile4.StateChanged(true);
        
                        }
                    }
                    else
                    {
                        AnimalFieldContents animalFieldContents = tile3.Contents as AnimalFieldContents;
                        AnimalFieldContents animalFieldContents2 = tile4.Contents as AnimalFieldContents;
                        if (animalFieldContents.Field == animalFieldContents2.Field)
                        {
                            return false;
                        }
                        if (animalFieldContents.Field.TileCount + animalFieldContents2.Field.TileCount > settings.maxTileCount)
                        {
                            return false;
                        }
                        animalFieldContents2.Field.Merge(animalFieldContents.Field);
                        tile3.StateChanged(true);
                        tile4.StateChanged(true);
        
                    }
                }
                return false;
            }
        }
    }
}
