using Harmony12;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityModManagerNet;

namespace Statistics
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
            isEnable = false;
            //读取用户配置文件
            settings = Settings.Load<Settings>(modEntry);
            //储存日志输出对象
            logger = modEntry.Logger;
            //logger.Log("Load");
            // modEntry.OnGUI = OnGUI;
            // modEntry.OnSaveGUI = OnSaveGUI;
            //modEntry.OnToggle = OnToggle;           
            //将更改应用到游戏上
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            //logger.Log(trainScriptType.Name.ToString());
            return true;
        }

        //收获金钱统计
        [HarmonyPatch(typeof(CropDefinition), "GetHarvestMoney")]
        public class CropDefinition_GetHarvestMoney_Patch
        {
            [HarmonyPrefix]
            public static bool GetHarvestMoney(uint cropLevel, CropDefinition __instance, float bonusMultiplier = 1f)
            {
                try
                {
                    if (!isEnable)
                    {
                        return true;
                    }
                    List<string> cropList = new List<string>();
                    string maxCoinsID = string.Empty;
                    string maxBillsID = string.Empty;
                    string maxMedalsID = string.Empty;
                    string maxTicketsID = string.Empty;
                    int maxCoins = 0;
                    int maxBills = 0;
                    int maxMedals = 0;
                    int maxTickets = 0;

                    if (!cropList.Contains(__instance.FullId))
                    {
                        FileStream fs;
                        StreamWriter sw;
                        if (!File.Exists("D:\\" + __instance.FullId + ".txt"))
                        {
                            fs = new FileStream("D:\\" + __instance.FullId + ".txt", FileMode.Create, FileAccess.Write);
                        }
                        else
                        {
                            fs = new FileStream("D:\\" + __instance.FullId + ".txt", FileMode.Open, FileAccess.Write);

                        }
                        sw = new StreamWriter(fs);
                        for (int i = 1; i < 251; i++)
                        {
                            //非困难模式+无闪电加成
                            float fBonusMultiplier = (float)(i * 0.02 + 1);
                            FarmMoney money = __instance.GetHarvestMoney((uint)i, fBonusMultiplier);
                            sw.WriteLine("{");
                            sw.WriteLine("Level:" + i);
                            sw.WriteLine("Number:" + i.ToString() + ",");
                            sw.WriteLine("ID:" + __instance.FullId + ",");
                            sw.WriteLine("Name:" + __instance.name + ",");
                            sw.WriteLine("BuyXp:" + __instance.BuyXp.ToString() + ",");
                            sw.WriteLine("Experience:" + __instance.ExperienceFunction((uint)i).ToString() + ",");
                            sw.WriteLine("HarvestXp", __instance.HarvestXp.ToString() + ",");
                            sw.WriteLine("GrowTime:", __instance.GrowTime.ToString() + ",");
                            sw.WriteLine("Harvest coins:" + money.Coins + ",");
                            sw.WriteLine("Harvest bills:" + money.Bills + ",");
                            sw.WriteLine("Harvest medals:" + money.Medals + ",");
                            sw.WriteLine("Harvest tickets:" + money.Tickets + ",");
                            sw.WriteLine("Benifit coins:" + (money.Coins - __instance.Price.Coins) + ",");
                            sw.WriteLine("Benifit bills:" + (money.Bills - __instance.Price.Bills) + ",");
                            sw.WriteLine("Benifit medals:" + (money.Medals - __instance.Price.Medals) + ",");
                            sw.WriteLine("Benifit tickets:" + (money.Tickets - __instance.Price.Tickets) + ",");
                            sw.WriteLine("coins/GrowTime", (money.Coins - __instance.Price.Coins) / __instance.GrowTime + ",");
                            sw.WriteLine("bills/GrowTime", (money.Bills - __instance.Price.Bills) / __instance.GrowTime + ",");
                            sw.WriteLine("medals/GrowTime", (money.Medals - __instance.Price.Medals) / __instance.GrowTime + ",");
                            sw.WriteLine("tickets/GrowTime", (money.Tickets - __instance.Price.Tickets) / __instance.GrowTime + ",");
                            sw.WriteLine("},");
                            cropList.Add(__instance.FullId);

                            if (((money.Coins - __instance.Price.Coins) / __instance.GrowTime) > maxCoins)
                            {
                                maxCoins = (int)((money.Coins - __instance.Price.Coins) / __instance.GrowTime);
                                maxCoinsID = __instance.FullId;
                            }
                            if (((money.Bills - __instance.Price.Bills) / __instance.GrowTime) > maxBills)
                            {
                                maxBills = (int)((money.Bills - __instance.Price.Bills) / __instance.GrowTime);
                                maxBillsID = __instance.FullId;
                            }
                            if (((money.Medals - __instance.Price.Medals) / __instance.GrowTime) > maxMedals)
                            {
                                maxMedals = (int)((money.Medals - __instance.Price.Medals) / __instance.GrowTime);
                                maxMedalsID = __instance.FullId;
                            }
                            if (((money.Tickets - __instance.Price.Tickets) / __instance.GrowTime) > maxTickets)
                            {
                                maxTickets = (int)((money.Tickets - __instance.Price.Tickets) / __instance.GrowTime);
                                maxTicketsID = __instance.FullId;
                            }
                        }     
                        sw.Close();
                        fs.Close();
                    }
                    logger.Log("maxCoinsID:" + maxCoinsID + "," + "maxCoins" + maxCoins);
                    logger.Log("maxBillsID:" + maxBillsID + "," + "maxBills" + maxBills);
                    //logger.Log("maxMedalsID:" + maxMedalsID + "," + "maxMedals" + maxMedals);
                    //logger.Log("maxTicketsID:" + maxTicketsID + "," + "maxTickets" + maxTickets);
                }
                catch (Exception e)
                {
                    logger.Log(e.Message);
                }

                return true;
            }
        }

    }
}
