using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 管理ListViewItem的增删操作
/// </summary>
public class ListView : MonoBehaviour
{
    /// <summary>
    /// 由UIQuestSystem管理整个任务系统
    /// 当UIQuestItem被选择时触发这个委托
    /// </summary>
    public UnityAction<ListViewItem> onItemSelected;
    
    /// <summary>
    /// ListView的内部类
    /// UIQuestItem的父类
    /// </summary>
    public class ListViewItem : MonoBehaviour, IPointerClickHandler
    {
        /// <summary>
        /// 被选中UIQusetItem时为true
        /// </summary>
        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                onSelected(selected);
            }
        }
        public virtual void onSelected(bool selected)
        {
        }

        /// <summary>
        /// 建立ListView与ListViewItem的连接
        /// </summary>
        public ListView owner;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!this.selected)
            {
                this.Selected = true;
            }
            if (owner != null && owner.SelectedItem != this)
            {
                owner.SelectedItem = this;
            }
        }
    }

    /// <summary>
    /// 管理所有UIQuestItem的列表
    /// </summary>
    List<ListViewItem> items = new List<ListViewItem>();

    /// <summary>
    /// 被选中的物品
    /// </summary>
    private ListViewItem selectedItem = null;
    public ListViewItem SelectedItem
    {
        get { return selectedItem; }
        private set
        {
            if (selectedItem!=null && selectedItem != value)
            {
                selectedItem.Selected = false;
            }
            selectedItem = value;
            if (onItemSelected != null)
            {
                onItemSelected.Invoke((ListViewItem)value);
            }
                
        }
    }

    /// <summary>
    /// 增加物品（UIQuestItem）
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(ListViewItem item)
    {
        item.owner = this;
        this.items.Add(item);
    }

    /// <summary>
    /// 移除所有物品（UIQuestItem）
    /// </summary>
    public void RemoveAll()
    {
        foreach(var it in items)
        {
            Destroy(it.gameObject);
        }
        items.Clear();
    }
}
