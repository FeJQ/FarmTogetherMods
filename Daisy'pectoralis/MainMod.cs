using Harmony12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityModManagerNet;

namespace Daisy_pectoralis
{
    public class MainMod
    {
        public static UnityModManager.ModEntry.ModLogger logger;
        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            //储存日志输出对象
            logger = modEntry.Logger;
            //将更改应用到游戏上
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            return true;
        }

        [HarmonyPatch(typeof(CharacterManager), "GetItemEnum")]
        public class LocalPlayerScript_UpdateFarmTiles_Patch
        {
            [HarmonyPrefix]
            public static bool GetItemEnum(AvatarCustomizationCategory type, BodyTypes currentBodyType, ref IEnumerable<CustomizationItemDefinition> __result)
            {
                List<CharacterBodyDefinition> bodiesList = Traverse.CreateWithType("CharacterManager").Field("bodiesList").GetValue<List<CharacterBodyDefinition>>();
                foreach (CharacterBodyDefinition item in bodiesList)
                {
                    item.BodyType = currentBodyType;
                }

                List<CharacterHeadDefinition> headsList = Traverse.CreateWithType("CharacterManager").Field("headsList").GetValue<List<CharacterHeadDefinition>>();
                foreach (CharacterHeadDefinition item in headsList)
                {
                    item.BodyType = currentBodyType;
                }

                List<CharacterHairDefinition> hairList = Traverse.CreateWithType("CharacterManager").Field("hairList").GetValue<List<CharacterHairDefinition>>();
                foreach (CharacterHairDefinition item in hairList)
                {
                    item.BodyType = currentBodyType;
                }

                List<CharacterHeadDecoDefinition> glassesList = Traverse.CreateWithType("CharacterManager").Field("glassesList").GetValue<List<CharacterHeadDecoDefinition>>();
                foreach (CharacterHeadDecoDefinition item in glassesList)
                {
                    item.BodyType = currentBodyType;
                }

                return true;
            }

        }
    }
}
