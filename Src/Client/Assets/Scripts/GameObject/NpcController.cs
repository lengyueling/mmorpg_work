using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Data;
using Managers;

public class NpcController : MonoBehaviour {

    public int NpcID;
    Animation anim;
    NpcDefine npc;
	void Start ()
    {
        anim = this.gameObject.GetComponent<Animation>();
        npc = NpcManager.Instance.GetNpcDefine(NpcID);
	}
	
	void Update ()
    {
        
	}

    private void OnMouseDown()
    {
        Debug.LogWarning(gameObject.name);
    }
}
