using Harmony12;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UnityModManagerNet;

namespace UnlockDLCItems
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
       // public static Assembly asm = Assembly.Load("Assembly-CSharp");
        //public static Assembly asm = Assembly.LoadFile(@"E:\Program Files (x86)\steamapps\common\Farm Together\FarmTogether_Data\Managed\Assembly-CSharp.dll");
       // public static Type FarmManager = asm.GetType("FarmManager");

        public static bool isEnable;
        public static Settings settings;
        public static UnityModManager.ModEntry.ModLogger logger;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            isEnable = false;
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

            //ConstructorInfo t1Constructor = trainScriptType.GetConstructor(Type.EmptyTypes);
            //System.Object oPubClass = t1Constructor.Invoke(new System.Object[] { });

            //流程控制
            //if(Convert.ToDateTime("19/11/13").CompareTo(Convert.ToDateTime(DateTime.Now.ToString("yy/MM/dd")))<0)
            //{
            //    return false;            
            //}

            MethodInfo method = typeof(Logic.Mode.FarmManager).GetMethod("OwnsDLC", new Type[] { typeof(GameDLCs) }) ;
            HarmonyMethod transpiler = new HarmonyMethod(typeof(MainMod).GetMethod("FarmManager_OwnsDLC_Transpiler"));
            harmony.Patch(method, null, null, transpiler);

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
        /// //当用户保存mod配置的时候调用
        /// </summary>
        /// <param name="modEntry"></param>
        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
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

        [HarmonyPatch(typeof(Logic.Mode.FarmManager), "OwnsDLC", new Type[] { typeof(GameDLCs) })]
        public class FarmManager_OwnsDLC_Patch
        {
            [HarmonyPrefix]
            public static bool OwnsDLC(GameDLCs dlc, bool __result)
            {

                if (!isEnable)
                {
                    return true;
                }

                __result = true;
                logger.Log(__result.ToString());
                return false;
            }
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> FarmManager_OwnsDLC_Transpiler(IEnumerable<CodeInstruction> instructions)
        {

            logger.Log("Transpiler");
            var codes = new List<CodeInstruction>(instructions);
            codes.InsertRange(0, new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldc_I4,1),
                new CodeInstruction(OpCodes.Ret)
            }) ;
            return codes.AsEnumerable();
        }
    }
}
