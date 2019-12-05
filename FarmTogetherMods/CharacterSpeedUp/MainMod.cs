using Harmony12;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityModManagerNet;

namespace CharacterSpeedUp
{
    public class Speed
    {
        [JsonProperty("Speed")]
        public List<struct_speed> speeds;
    }
    public struct struct_speed
    {
        [JsonProperty("Type")]
        public string SpeedType;
        [JsonProperty("Times")]
        public float Times;
    }
    public class Settings : UnityModManager.ModSettings
    {
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
    public class MainMod
    {
        //public static Assembly asm = Assembly.Load("Assembly-CSharp");
        //public static Type trainScriptType = asm.GetType("TrainScript");

        
        public static float baseSpeed = 0;
        public static float sprintSpeed = 0;
        public static float animationSpeed = 0;

        public static Speed speed;
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
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            //modEntry.OnToggle = OnToggle;           
            //将更改应用到游戏上
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            //logger.Log(trainScriptType.Name.ToString());


            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            StreamReader sr = new StreamReader(path + "\\config.json");

            string json = sr.ReadToEnd();
            speed = JsonConvert.DeserializeObject<Speed>(json);


            return true;
        }

        /// <summary>
        /// //当用户打开mod配置的时候调用
        /// </summary>
        /// <param name="modEntry"></param>
        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.Label("启动成功!");
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
        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            isEnable = value;
            //modEntry.Logger.Log(Application.loadedLevelName);
            return true;
        }

        [HarmonyPatch(typeof(PlayerScript), "updateCharacterControllerParameters")]
        public class PlayerScript_updateCharacterControllerParameters_Patch
        {
            [HarmonyPostfix]
            public static void updateCharacterControllerParameters(PlayerScript __instance)
            {
                if (!isEnable)
                {
                    return;
                }
                //var controller=Traverse.Create(__instance).Field("controller").GetValue<MilkThirdPersonController>();
                //controller.BaseSpeed = 20;

                foreach (struct_speed s in speed.speeds)
                {
                    switch (s.SpeedType)
                    {
                        case "BaseSpeed": baseSpeed = s.Times; break;
                        case "SprintSpeed": sprintSpeed = s.Times; break;
                        default: break;
                    }
                }
                __instance.Controller.BaseSpeed *= baseSpeed;
                __instance.Controller.SprintSpeed *= sprintSpeed;

                //__instance.Controller.Responsiveness *= 10;
                //__instance.Controller.LookAtSpeed *= 10;
                //__instance.Controller.CharacterController.radius *= 10;
               
            }
        }



        [HarmonyPatch(typeof(CharacterAnimator), "GetInteractionTime")]
        public class CharacterAnimator_GetInteractionTime_Patch
        {
            [HarmonyPrefix]
            public static bool GetInteractionTime(CharacterAnimator.InteractionAnimation anim, out float animationDuration, PlayerScript __instance, float __result)
            {
                if (!isEnable)
                {
                    animationDuration = 0;
                    return true;
                }

                float result;
                switch (anim)
                {
                    case CharacterAnimator.InteractionAnimation.Plow:
                        animationDuration = 1.1666666f;
                        result = 0.73333335f;
                        break;
                    case CharacterAnimator.InteractionAnimation.Plant:
                        animationDuration = 1.1666666f;
                        result = 0.6333333f;
                        break;
                    case CharacterAnimator.InteractionAnimation.Build:
                        animationDuration = 1.1666666f;
                        result = 0.6333333f;
                        break;
                    case CharacterAnimator.InteractionAnimation.Water:
                        animationDuration = 1.1666666f;
                        result = 1f;
                        break;
                    case CharacterAnimator.InteractionAnimation.FeedAnimal:
                        animationDuration = 1.6666666f;
                        result = 0.73333335f;
                        break;
                    case CharacterAnimator.InteractionAnimation.RefillGas:
                    case CharacterAnimator.InteractionAnimation.BuySellResources:
                        animationDuration = 1.8333334f;
                        result = 1.5f;
                        break;
                    case CharacterAnimator.InteractionAnimation.HarvestCrop:
                        animationDuration = 1.6666666f;
                        result = 1f;
                        break;
                    case CharacterAnimator.InteractionAnimation.HarvestTree:
                        animationDuration = 1.6666666f;
                        result = 0.5f;
                        break;
                    case CharacterAnimator.InteractionAnimation.HarvestFish:
                        animationDuration = 2.3333333f;
                        result = 1.6666666f;
                        break;
                    default:
                        switch (anim)
                        {
                            case CharacterAnimator.InteractionAnimation.Recipe_Cook:
                                animationDuration = 1.8333334f;
                                result = 1.3333334f;
                                break;
                            case CharacterAnimator.InteractionAnimation.Recipe_Song:
                                animationDuration = 6f;
                                result = 1.6f;
                                break;
                            case CharacterAnimator.InteractionAnimation.Recipe_Paint:
                                animationDuration = 2.5333333f;
                                result = 2.2f;
                                break;
                            default:
                                if (anim != CharacterAnimator.InteractionAnimation.VehicleInteract && anim != CharacterAnimator.InteractionAnimation.VehicleWater)
                                {
                                    animationDuration = 1f;
                                    result = 1f;
                                }
                                else
                                {
                                    animationDuration = 1.3333334f;
                                    result = 0.8666667f;
                                }
                                break;
                        }
                        break;
                }

                foreach (struct_speed s in speed.speeds)
                {
                    switch (s.SpeedType)
                    {
                        case "AnimationSpeed": animationSpeed = s.Times; break;
                        default: break;
                    }
                }
                animationDuration = animationDuration / animationSpeed;
                __result = result/10;
                return false;
            }
        }


        
    }
}
