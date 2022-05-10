using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common.Data;
using Models;
using Managers;
using SkillBridge.Message;
using System;

public class UIRide : UIWindow
{
    public Text descript;
    public GameObject itemPrefab;
    public ListView listMain;
    private UIRideItem selectedItem;
	void Start ()
    {
        RefreshUI();
        this.listMain.onItemSelected += this.OnItemSelected;
	}

    private void OnDestroy()
    {
        
    }

    private void OnItemSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIRideItem;
        this.descript.text = this.selectedItem.item.Define.Description;
    }

    private void RefreshUI()
    {
        ClearItems();
        InitItems();
    }

    private void InitItems()
    {
        foreach (var kv in ItemManager.Instance.Items)
        {
            if (kv.Value.Define.Type == ItemType.Ride && (kv.Value.Define.LimitClass == User.Instance.CurrentCharacter.Class || kv.Value.Define.LimitClass == CharacterClass.None))
            {
                GameObject go = Instantiate(itemPrefab, this.listMain.transform);
                UIRideItem ui = go.GetComponent<UIRideItem>();
                ui.SetRideItem(kv.Value, this, false);
                this.listMain.AddItem(ui);
            }
        }
    }

    private void ClearItems()
    {
        this.listMain.RemoveAll();
    }

    public void DoRide()
    {
        if (this.selectedItem == null)
        {
            MessageBox.Show("请选择要召唤的坐骑", "提示");
            return;
        }
        User.Instance.Ride(this.selectedItem.item.Id);
        this.Close();
    }
}
