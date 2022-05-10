using System;
using System.Collections;
using System.Collections.Generic;
using Common.Data;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Models;

public class UIRideItem : ListView.ListViewItem
{
    public Image icon;
    public Text title;
    public Text level;

    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;

    public Item item;

    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }

    public void SetRideItem(Item item, UIRide owner, bool equiped)
    {
        this.item = item;
        if (this.title != null)
        {
            this.title.text = this.item.Define.Name;
        }

        if (this.level != null)
        {
            this.level.text = "lv." + this.item.Define.Level.ToString();
        }

        if (this.icon != null)
        {
            this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.Define.Icon);
        }
    }
	
}
