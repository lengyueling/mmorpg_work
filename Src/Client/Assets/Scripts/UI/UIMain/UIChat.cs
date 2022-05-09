using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using Candlelight.UI;
using System;
using SkillBridge.Message;
using Models;

public class UIChat : MonoBehaviour
{
    public HyperText textArea;
    public TabView cancelTab;

    public InputField chatText;

    public Text chatTarget;

    public Dropdown channelSelect;

    void Start ()
    {
        this.cancelTab.OnTabSelect += onDisplayChannelSelected;
        ChatManager.Instance.OnChat += RefreshUI;
	}

    private void OnDestroy()
    {
        ChatManager.Instance.OnChat -= RefreshUI;
    }

    private void Update()
    {
        InputManager.Instance.IsInputMode = chatText.isFocused;
    }

    private void onDisplayChannelSelected(int idx)
    {
        ChatManager.Instance.displayChannel = (ChatManager.LocalChannel)idx;
        RefreshUI();
    }

    public void RefreshUI()
    {
        //TODO 临时措施 离开时候CurrentCharacter = null，会导致AddMessages空引用
        if (User.Instance.CurrentCharacter == null)
        {
            return;
        }
        this.textArea.text = ChatManager.Instance.GetCurrentMessages();
        this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
        if (ChatManager.Instance.SendChannel == ChatChannel.Private)
        {
            this.chatTarget.gameObject.SetActive(true);
            if (ChatManager.Instance.PrivateID != 0)
            {
                this.chatTarget.text = ChatManager.Instance.PrivateName + ":";
            }
            else
            {
                this.chatTarget.text = "<无>:";
            }
        }
        else
        {
            this.chatTarget.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 点击发送按钮
    /// </summary>
    /// <param name="text"></param>
    /// <param name="link"></param>
    public void OnClickChatLink(HyperText text, HyperText.LinkInfo link)
    {
        if (string.IsNullOrEmpty(link.Name))
        {
            return;
        }

        //筛选点击的是什么类型的物体，是一种约定
        if (link.Name.StartsWith("c:"))
        {
            string[] strs = link.Name.Split(":".ToCharArray());
            UIPopCharMenu menu = UIManager.Instance.Show<UIPopCharMenu>();
            menu.targetId = int.Parse(strs[1]);
            menu.targetName = strs[2];
        }
    }

    public void OnClickSend()
    {
        OnEndInput(this.chatText.text);
    }

    /// <summary>
    /// 输入框失去焦点时调用
    /// </summary>
    /// <param name="text"></param>
    public void OnEndInput(string text)
    {
        if (!string.IsNullOrEmpty(text.Trim()))
        {
            this.SendChat(text);
        }
        this.chatText.text = "";
    }

    private void SendChat(string content)
    {
        ChatManager.Instance.SendChat(content, ChatManager.Instance.PrivateID, ChatManager.Instance.PrivateName);
    }

    /// <summary>
    /// 下拉菜单改变时调用
    /// </summary>
    /// <param name="idx"></param>
    public void OnSendChannelChanged(int idx)
    {
        if (ChatManager.Instance.sendChannel == (ChatManager.LocalChannel)(idx + 1))
        {
            return;
        }
        if (!ChatManager.Instance.SetSendChannel((ChatManager.LocalChannel)idx + 1))
        {
            this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
        }
        else
        {
            this.RefreshUI();
        }
    }
}
