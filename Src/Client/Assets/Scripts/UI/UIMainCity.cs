using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainCity : MonoBehaviour {

    public Text avatarName;
    public Text avatarLevel;

    void Start()
    {
        this.UpdateAvatar();
    }

    void UpdateAvatar()
    {
        this.avatarName.text = string.Format("{0}[{1}]", User.Instance.CurrentCharacter.Name, User.Instance.CurrentCharacter.Id);
        this.avatarLevel.text = User.Instance.CurrentCharacter.Level.ToString();
    }
	
}
