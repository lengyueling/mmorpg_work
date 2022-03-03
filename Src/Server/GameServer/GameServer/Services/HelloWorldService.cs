﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;

namespace GameServer.Services
{
    class HelloWorldService:Singleton<HelloWorldService>
    {
        public void Init()
        {

        }
        public void Start()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FirstTestRequest>(this.onFirstTestRequest);
        }
        void onFirstTestRequest(NetConnection<NetSession> sender,FirstTestRequest request)
        {
            Log.InfoFormat("onFirstTestRequest:{0}", request.Helloword);
        }
    }
}
