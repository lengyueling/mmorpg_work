using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharInfo : MonoBehaviour {


    public SkillBridge.Message.NCharacterInfo info;

    public Text charClass;
    public Text charName;
    public Image hightLight;

    /// <summary>
    /// 该角色是否被选中
    /// </summary>
    public bool Selected
    {
        get
        {
            return hightLight.IsActive();
        }
        set
        {
            hightLight.gameObject.SetActive(value);
        }
    }

    void Start () {
		if(info!=null)
        {
            this.charClass.text = this.info.Class.ToString();
            this.charName.text = this.info.Name;
        }
	}
	
	void Update () {
		
	}
}
