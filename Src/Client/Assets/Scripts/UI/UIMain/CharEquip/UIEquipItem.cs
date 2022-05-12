using System;
using System.Collections;
using System.Collections.Generic;
using Common.Data;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Models;
using Managers;

public class UIEquipItem : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public Text title;
    public Text level;
    public Text limitClass;
    public Text limitCategory;

    public Image background;
    public Sprite normalBg;
    public Sprite selectBg;

    private bool selected;
    public bool Selected
    {
        get
        {
            return selected;
        }
        set
        {
            selected = value;
            this.background.overrideSprite = selected ? selectBg : normalBg;
        }
    }

    public int index { get; set; }
    private UICharEquip owner;

    private Item item;

    bool isEquiped = false;

    /// <summary>
    /// 设置装备系统UI
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="item"></param>
    /// <param name="owner"></param>
    /// <param name="equiped"></param>
    public void SetEquipItem(int idx,Item item,UICharEquip owner,bool equiped)
    {
        this.owner = owner;
        this.index = idx;
        this.item = item;
        this.isEquiped = equiped;

        if (this.title != null)
        {
            this.title.text = this.item.Define.Name;
        }
        if (this.level != null)
        {
            this.level.text = this.item.Define.Level.ToString();
        }
        if (this.limitClass != null)
        {
            this.limitClass.text = item.Define.LimitClass.ToString();
        }
        if (this.limitCategory != null)
        {
            this.limitCategory.text = item.Define.Category;
        }
        if (this.icon != null)
        {
            this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.Define.Icon);
        }
    }

    /// <summary>
    /// 实现接口IPointerClickHandler
    /// 点击UIEquipItem时触发
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.isEquiped)
        {
            this.UnEquip();
        }
        else
        {
            if (this.selected)
            {
                this.DoEquip();
                this.selected = false;
            }
            else
            {
                this.selected = true;
            }
        }
    }

    /// <summary>
    /// 穿/替换装备
    /// </summary>
    private void DoEquip()
    {
        var msg = MessageBox.Show(string.Format("要穿上装备[{0}]吗？", this.item.Define.Name), "确定", MessageBoxType.Confirm);
        msg.OnYes = () =>
         {
             var oldEquip = EquipManager.Instance.GetEquip(item.EquipInfo.Slot);
             if (oldEquip != null)
             {
                 var newmsg = MessageBox.Show(string.Format("要替换装备[{0}]吗？", this.item.Define.Name), "确定", MessageBoxType.Confirm);
                 newmsg.OnYes = () =>
                 {
                     this.owner.DoEquip(this.item);
                 };
             }
             else
             {
                 this.owner.DoEquip(this.item);
             }
         };
    }

    /// <summary>
    /// 脱装备
    /// </summary>
    private void UnEquip()
    {
        var msg = MessageBox.Show(string.Format("要取消装备[{0}]吗？", this.item.Define.Name), "确定", MessageBoxType.Confirm);
        msg.OnYes = () =>
        {
            this.owner.UnEquip(this.item);
        };
    }
}
