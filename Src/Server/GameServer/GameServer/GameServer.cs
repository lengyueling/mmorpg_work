using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using System.Threading;
using Network;
using GameServer.Services;
using GameServer.Managers;

namespace GameServer
{
    class GameServer
    {
        Thread thread;
        bool running = false;
        NetService network;
        public bool Init()
        {
            int Port = Properties.Settings.Default.ServerPort;
            network = new NetService();
            network.Init(Port);
            DBService.Instance.Init();
            DataManager.Instance.Load();
            UserService.Instance.Init();
            MapService.Instance.Init();
            BagService.Instance.Init();
            ItemService.Instance.Init();
            QuestService.Instance.Init();
            FriendServiece.Instance.Init();

            thread = new Thread(new ThreadStart(this.Update));
            return true;
        }

        public void Start()
        {
            network.Start();
            running = true;
            thread.Start();
        }


        public void Stop()
        {
            network.Stop();
            running = false;
            thread.Join();
        }

        /// <summary>
        /// 通过线程模拟一秒执行十次的功能
        /// </summary>
        public void Update()
        {
            var mapManager = MapManager.Instance;
            while (running)
            {
                Time.Tick();
                Thread.Sleep(100);
                //Console.WriteLine("{0} {1} {2} {3} {4}", Time.deltaTime, Time.frameCount, Time.ticks, Time.time, Time.realtimeSinceStartup);

                //怪物生成有bug，之后改
                mapManager.Update();
            }
        }
    }
}
