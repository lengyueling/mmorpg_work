using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain>
{

    public Text avatarName;
    public Text avatarLevel;

    protected override void OnStart()
    {
        this.UpdateAvatar();
    }

    /// <summary>
    /// 更新Avatar
    /// </summary>
    void UpdateAvatar()
    {
        this.avatarName.text = string.Format("{0}[{1}]", User.Instance.CurrentCharacter.Name, User.Instance.CurrentCharacter.Id);
        this.avatarLevel.text = User.Instance.CurrentCharacter.Level.ToString();
    }

    /// <summary>
    /// 返回角色选择
    /// </summary>
    public void BackToCharSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");
        Services.UserService.Instance.SendGameLeave();
    }
	
}
