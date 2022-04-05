using Harmony12;
using Logic.Farm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityModManagerNet;

namespace FarmHelper
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
        public static LocalPlayerScript playerScript = null;
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
            // modEntry.OnGUI = OnGUI;
            // modEntry.OnSaveGUI = OnSaveGUI;
            //modEntry.OnToggle = OnToggle;           
            //将更改应用到游戏上
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Thread timer = new Thread(new ThreadStart(PrintLocalPos));
            timer.Start();
            return true;
        }

        static void PrintLocalPos()
        {
            logger.Log("线程已启动");
            while (true)
            {
                Thread.Sleep(1000);
                if (playerScript == null)
                {
                    logger.Log("null");
                    continue;
                }
                //Chunk指地块
                //打印块号
                logger.Log(
                    playerScript.CurrentChunk.ChunkPosition.x.ToString() +
                    "," +
                    playerScript.CurrentChunk.ChunkPosition.y.ToString()
                    );
                //打印坐标
                logger.Log(
                   playerScript.CurrentTile.TileId._TileX.ToString() +
                   "," +
                   playerScript.CurrentTile.TileId._TileY.ToString()
                   );
                logger.Log(
                    "State:"+
                    playerScript.CurrentTile.Floor.ToString()
                    );
                logger.Log("--------");
            }
        }

        [HarmonyPatch(typeof(LocalPlayerScript), "Start")]
        public class LocalPlayerScript_Start_Patch
        {
            [HarmonyPostfix]
            public static void Start(LocalPlayerScript __instance)
            {
                if (!isEnable)
                {
                    return;
                }
                if (playerScript == null)
                {                    
                    playerScript = __instance;
                }
            }
        }

    }
}
