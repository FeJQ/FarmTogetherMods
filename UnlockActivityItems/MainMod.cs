using Harmony12;
using Logic.Events;
using Logic.Mode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityModManagerNet;

namespace UnlockEventItems
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
            //logger.Log("Hello");
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

            //ConstructorInfo t1Constructor = trainScriptType.GetConstructor(Type.EmptyTypes);
            //System.Object oPubClass = t1Constructor.Invoke(new System.Object[] { });

            //流程控制
            //if(Convert.ToDateTime("19/11/13").CompareTo(Convert.ToDateTime(DateTime.Now.ToString("yy/MM/dd")))<0)
            //{
            //    return false;            
            //}

            MethodInfo method = typeof(FarmManager).GetMethod("IsLockedEvent");
            HarmonyMethod transpiler = new HarmonyMethod(typeof(MainMod).GetMethod("FarmManager_IsLockedEvent_Transpiler"));
            harmony.Patch(method, null, null, transpiler);

            return true;
        }


        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> FarmManager_IsLockedEvent_Transpiler(IEnumerable<CodeInstruction> instructions)
        {           
            var codes = new List<CodeInstruction>(instructions);

            foreach (var code in codes)
            {
                //logger.Log(code.opcode+" "+code.operand);
            }
            codes[3] = new CodeInstruction(OpCodes.Ldc_I4,0);
            codes[4] = new CodeInstruction(OpCodes.Ret);

            {
                //完全版
                codes[0] = new CodeInstruction(OpCodes.Ldc_I4, 0);
                codes[1] = new CodeInstruction(OpCodes.Ret);
            }
            return codes.AsEnumerable();
        }


       
    }
}
