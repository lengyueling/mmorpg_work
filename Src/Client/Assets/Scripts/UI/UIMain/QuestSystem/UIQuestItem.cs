using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestItem : ListView.ListViewItem
{
    public Text title;
    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;

    /// <summary>
    /// 重写选中方法，如果选中则切换背景图片
    /// </summary>
    /// <param name="selected"></param>
    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }

    public Quest quest;
    bool isEquiped = false;

    /// <summary>
    /// 设置任务信息
    /// </summary>
    /// <param name="item"></param>
    public void SetQuestInfo(Quest item)
    {
        this.quest = item;
        if (this.title != null)
        {
            this.title.text = this.quest.Define.Name;
        }
    }

}
