using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBag : UIWindow
{
    public Text money;

    public Transform[] pages;

    public GameObject bagItem;

    /// <summary>
    /// 多个背包槽形成的列表
    /// </summary>
    List<Image> slots;

    void Start()
    {
        if (slots == null)
        {
            slots = new List<Image>();
            for (int page = 0; page < this.pages.Length; page++)
            {
                slots.AddRange(this.pages[page].GetComponentsInChildren<Image>(true));
            }
            StartCoroutine(InitBags());
        }
    }
    /// <summary>
    /// 初始化背包
    /// </summary>
    /// <returns></returns>
    IEnumerator InitBags()
    {
        for (int i = 0; i < BagManager.Instance.Items.Length; i++)
        {
            var item = BagManager.Instance.Items[i];
            //设置背包槽的icon
            if (item.ItemId > 0)
            {
                GameObject go = Instantiate(bagItem, slots[i].transform);
                var ui = go.GetComponent<UIIconItem>();
                var def = ItemManager.Instance.Items[item.ItemId].Define;
                ui.SetMainIcon(def.Icon, item.Count.ToString());
            }
        }
        //设置未解锁的槽为灰色
        for (int i = BagManager.Instance.Items.Length; i < slots.Count; i++)
        {
            slots[i].color = Color.gray;
        }
        SetMoney();
        yield return null;
    }

    /// <summary>
    /// 设置当前金钱
    /// </summary>
    public void SetMoney()
    {
        //this.money.text = User.Instance.CurrentCharacter.Id.ToString();
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    /// <summary>
    /// 整理背包
    /// </summary>
    public void OnReset()
    {
        BagManager.Instance.Reset();
    }
}
