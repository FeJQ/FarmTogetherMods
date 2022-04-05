using Harmony12;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityModManagerNet;

namespace TrainSpeedUp
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
        public static Assembly asm = Assembly.Load("Assembly-CSharp");
        //public static Assembly asm = Assembly.LoadFile(@"E:\Program Files (x86)\steamapps\common\Farm Together\FarmTogether_Data\Managed\Assembly-CSharp.dll");
        public static Type trainScriptType = asm.GetType("TrainScript");

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
            modEntry.OnGUI = OnGUI;
            //modEntry.OnSaveGUI = OnSaveGUI;
            //modEntry.OnToggle = OnToggle;           
            //将更改应用到游戏上
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            if(!isEnable)
            {
                return false;
            }
            ConstructorInfo t1Constructor = trainScriptType.GetConstructor(Type.EmptyTypes);
            System.Object oPubClass = t1Constructor.Invoke(new System.Object[] { });
            MethodInfo method = trainScriptType.GetMethod("TrainCoroutine", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo prefix = typeof(MainMod).GetMethod("TrainScript_TrainCoroutine_Transpiler");
            HarmonyMethod transpiler = new HarmonyMethod(prefix);
            harmony.Patch(method, null, null, transpiler);


            //string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //StreamReader sr = new StreamReader(path + "\\config.json");
            //
            //string json = sr.ReadToEnd();
            //speed = JsonConvert.DeserializeObject<Speed>(json);


            //logger.Log(trainScriptType.GetMethods(BindingFlags.NonPublic).Length.ToString());
            //MethodInfo methodInfo= trainScriptType.GetMethod("TrainCoroutine");
            //logger.Log(methodInfo.Name);
            //harmony.Patch()

            return true;
        }
        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
        private static void OnGUI(UnityModManager.ModEntry obj)
        {
            GUILayout.Label("启动成功!");
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> TrainScript_TrainCoroutine_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            logger.Log("Transpiler");
            float ACCTimes = 10;
            float maxSpeedTimes = 3;
            var codes = new List<CodeInstruction>(instructions);

            foreach (var code in codes)
            {
                logger.Log(code.opcode.ToString());
            }
            return codes.AsEnumerable();

            //var injectedCodes = new List<CodeInstruction>()
            //    {
            //        new CodeInstruction(OpCodes.Ldc_I4,0.6*10),
            //    };
            //codes.InsertRange(startIndex, injectedCodes);
            codes[247] = new CodeInstruction(OpCodes.Ldc_I4, 0.6 * ACCTimes);

            codes.InsertRange(254, new List<CodeInstruction>() {
                new CodeInstruction(OpCodes.Ldc_I4,maxSpeedTimes),
                new CodeInstruction(OpCodes.Div)
            });
            codes.InsertRange(263, new List<CodeInstruction>() {
                new CodeInstruction(OpCodes.Ldc_I4,maxSpeedTimes),
                new CodeInstruction(OpCodes.Mul)
            });


            return codes.AsEnumerable();


        }
    }
}
