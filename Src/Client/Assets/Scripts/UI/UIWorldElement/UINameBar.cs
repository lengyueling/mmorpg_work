using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINameBar : MonoBehaviour {

    public Text avaverName;
    public Character character;


    // Use this for initialization
    void Start () {
		if(this.character!=null)
        {
            
        }
	}
	
	// Update is called once per frame
	void Update () {
        this.UpdateInfo();
        //this.transform.LookAt(Camera.main.transform, Vector3.up);
        this.transform.forward = Camera.main.transform.forward;
	}

    void UpdateInfo()
    {
        if (this.character != null)
        {
            string name = this.character.Name + " Lv." + this.character.Info.Level;
            //通过判断提高性能
            if(name != this.avaverName.text)
            {
                this.avaverName.text = name;
            }
        }
    }
}
