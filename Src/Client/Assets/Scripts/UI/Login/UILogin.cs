using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;
using Services;

public class UILogin : MonoBehaviour
{

    public InputField username;
    public InputField password;
    public Button buttonEnter;

    void Start()
    {
        UserService.Instance.OnLogin += OnLogin;
    }

    void OnLogin(Result result, string msg)
    {
        MessageBox.Show(string.Format("结果：{0} msg:{1}", result, msg));
        if (result == Result.Success)
        {
            //登录成功，进入角色选择
            SceneManager.Instance.LoadScene("CharSelect");
            SoundManager.Instance.PlaySound(SoundDefine.Music_Select);
        }
    }

    public void OnClickLogin()
    {
        if (string.IsNullOrEmpty(this.username.text))
        {
            MessageBox.Show("请输入账号");
            return;
        }
        if (string.IsNullOrEmpty(this.password.text))
        {
            MessageBox.Show("请输入密码");
            return;
        }
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        UserService.Instance.SendLogin(this.username.text, this.password.text);
    }

}
