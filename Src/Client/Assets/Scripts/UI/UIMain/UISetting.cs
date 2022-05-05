using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;

public class UISetting : UIWindow
{

	public void ExitToCharSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");
        UserService.Instance.SendGameLeave();
    }

    public void ExitGame()
    {
        UserService.Instance.SendGameLeave(true);
    }
}
