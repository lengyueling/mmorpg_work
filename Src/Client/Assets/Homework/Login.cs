using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillBridge.Message;
using Network;

public class Login : MonoBehaviour
{
	void Start ()
    {
        NetClient.Instance.Init("127.0.0.1",8000);
        NetClient.Instance.Connect();

        NetMessage msg = new NetMessage();
        msg.Request = new NetMessageRequest();
        msg.Request.firstRequest = new FirstTestRequest();
        msg.Request.firstRequest.Helloword = "HelloWorld";
        NetClient.Instance.SendMessage(msg);
	}

	void Update ()
    {
		
	}
}
