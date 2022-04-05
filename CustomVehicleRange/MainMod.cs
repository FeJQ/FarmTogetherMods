using Harmony12;
using Milkstone.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using UnityModManagerNet;

namespace CustomVehicleRange
{
    public class Settings : UnityModManager.ModSettings
    {
        public int range = 5;
        public int maxTileCount = 1000;
        public bool isEnable = true;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
    public class MainMod
    {

        public static Settings settings;
        public static UnityModManager.ModEntry.ModLogger logger;
        public static bool Load(UnityModManager.ModEntry modEntry)
        {

            //logger.Log("Hello");
            //读取用户配置文件
            settings = Settings.Load<Settings>(modEntry);
            //储存日志输出对象
            logger = modEntry.Logger;
            //logger.Log("Load");
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
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
            //GUILayout.BeginVertical("BOX");
            //GUILayout.BeginHorizontal();

            GUILayout.Label("范围:");
            if (int.TryParse(GUILayout.TextField(settings.range.ToString(), new GUILayoutOption[] { GUILayout.Width(50) }), out var num) && num > 0)
            {
                settings.range = num;
            }
            //settings.isEnable=GUILayout.Toggle(settings.isEnable,"启用");
            //logger.Log(settings.isEnable.ToString()) ;
        }

        /// <summary>
        /// //当用户保存mod配置的时候调用
        /// </summary>
        /// <param name="modEntry"></param>
        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        [HarmonyPatch(typeof(LocalPlayerScript), "UpdateFarmTiles")]
        public static class LocalPlayerScript_UpdateFarmTiles_Patch
        {
            public static Int2 VehicleToolSize = new Int2(settings.range, settings.range);

            [HarmonyTranspiler]
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                var modifierField = AccessTools.Field(typeof(LocalPlayerScript_UpdateFarmTiles_Patch),
                    nameof(VehicleToolSize));
                var newInstruction = new CodeInstruction(OpCodes.Ldsfld, modifierField);
                var indexes = new List<int>();
                for (var i = 0; i < codes.Count(); i++)
                {
                    if (codes[i].opcode != OpCodes.Ldsfld)
                        continue;

                    if (!codes[i].operand.ToString().Equals("Milkstone.Utils.Int2 VehicleToolSize"))
                        continue;
                    indexes.Add(i);
                }

                foreach (var index in indexes)
                    codes[index] = newInstruction;

                return codes;
            }
        }

        [HarmonyPatch]
        public class SelectedTiles_Awake_Patch
        {
            private static MethodBase TargetMethod(HarmonyInstance instance)
            {
                return AccessTools.Method("SelectedTiles:Awake");
            }

            public static void Postfix(object __instance)
            {
                Traverse.Create(__instance).Field("previews").SetValue(
                    new GameObject[LocalPlayerScript_UpdateFarmTiles_Patch.VehicleToolSize.x *
                                   LocalPlayerScript_UpdateFarmTiles_Patch.VehicleToolSize.y]);
            }
        }



        [HarmonyPatch]
        public class SelectedTiles_initLines_Patch
        {
            private static MethodBase TargetMethod(HarmonyInstance instance)
            {
                return AccessTools.Method("SelectedTiles:initLines");
            }

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                codes[10] = new CodeInstruction(OpCodes.Ldc_I4,settings.maxTileCount);
                codes[145] = new CodeInstruction(OpCodes.Ldc_I4, settings.maxTileCount);

                return codes.AsEnumerable();
                //Traverse obj = Traverse.Create(__instance);

                //Type t = obj.GetType();


                //obj.Field("linePoints").SetValue(new Vector3[4]);
                //MethodInfo updatePointData = t.GetMethod("updatePointData");
                //updatePointData.Invoke(__instance, new object[] { 1, 1, false });

                //Traverse traLines = obj.Field("lines");
                //traLines.SetValue(new LineRenderer[settings.maxTileCount]);
                //LineRenderer[] lines = traLines.GetValue<LineRenderer[]>();

                //for (int i = 0; i < settings.maxTileCount; i++)
                //{
                //    GameObject gameObject = new GameObject("LineBorder " + i);
                //    gameObject.transform.parent = obj.GetType().BaseType.get
                //    gameObject.transform.rotation = obj.Field("lineBaseRotation").GetValue<Quaternion>();
                //    gameObject.SetActive(false);
                //    lines[i] = gameObject.AddComponent<LineRenderer>();
                //    lines[i].shadowCastingMode = ShadowCastingMode.Off;
                //    lines[i].sharedMaterial = obj.Field("BorderMaterial").GetValue<Material>();
                //    LineRenderer lineRenderer = lines[i];
                //    float num = 1f;
                //    lines[i].endWidth = num;
                //    lineRenderer.startWidth = num;
                //    lines[i].widthMultiplier = 0.12f;
                //    lines[i].useWorldSpace = false;
                //    lines[i].alignment = LineAlignment.Local;
                //    lines[i].textureMode = LineTextureMode.Tile;
                //    lines[i].loop = true;
                //    lines[i].positionCount = 4;
                //    lines[i].numCornerVertices = 5;
                //    lines[i].SetPositions(obj.Field("linePoints").GetValue<Vector3[]>());
                //    LineRenderer lineRenderer2 = lines[i];
                //    Color color = new Color(0f, 1f, 1f);
                //    lines[i].endColor = color;
                //    lineRenderer2.startColor = color;
                //    lines[i].motionVectorGenerationMode = MotionVectorGenerationMode.Camera;
                //}
                //return false;
            }
        }
    }
}
