﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;
using ProtoBuf;
using System.IO;
using Common;
using System.Threading;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //日志的初始化，引入日志组件
            FileInfo fi = new System.IO.FileInfo("log4net.xml");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(fi);
            Log.Init("GameServer");
            Log.Info("Game Server Init");

            //服务器初始化
            GameServer server = new GameServer();
            server.Init();
            server.Start();
            Console.WriteLine("Game Server Running......");
            CommandHelper.Run();
            Log.Info("Game Server Exiting...");
            server.Stop();
            Log.Info("Game Server Exited");
        }
    }
}
