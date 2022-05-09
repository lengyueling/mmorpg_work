using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Models;

public class UISetting : UIWindow
{

	public void ExitToCharSelect()
    {
        
        UserService.Instance.SendGameLeave();
        SceneManager.Instance.LoadScene("CharSelect");
    }

    public void ExitGame()
    {
        UserService.Instance.SendGameLeave(true);
    }
}
