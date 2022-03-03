using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login : MonoBehaviour
{
	void Start ()
    {
        Network.NetClient.Instance.Init("127.0.0.1",8000);
        Network.NetClient.Instance.Connect();

        SkillBridge.Message.NetMessage msg = new SkillBridge.Message.NetMessage();
        msg.Request = new SkillBridge.Message.NetMessageRequest();
        msg.Request.firstRequest = new SkillBridge.Message.FirstTestRequest();
        msg.Request.firstRequest.Helloword = "HelloWorld";
        Network.NetClient.Instance.SendMessage(msg);
	}

	void Update ()
    {
		
	}
}
