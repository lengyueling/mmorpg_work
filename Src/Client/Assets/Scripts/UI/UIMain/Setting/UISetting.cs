using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Models;
using Managers;

public class UISetting : UIWindow
{

	public void ExitToCharSelect()
    {
        UserService.Instance.SendGameLeave();
        SoundManager.Instance.PlayMusic(SoundDefine.Music_Select);
        SceneManager.Instance.LoadScene("CharSelect");
    }

    public void SystemConfig()
    {
        UIManager.Instance.Show<UISystemConfig>();
        this.Close();
    }

    public void ExitGame()
    {
        UserService.Instance.SendGameLeave(true);
    }
}
