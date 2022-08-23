using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TabView : MonoBehaviour
{
    public UnityAction<int> OnTabSelect;
    /// <summary>
    /// 几号按钮上的脚本
    /// </summary>
    public TabButton[] tabButtons;
    /// <summary>
    /// 几号的物体
    /// ScrollView_page
    /// </summary>
    public GameObject[] tabPages;
    public int index = -1;

	IEnumerator Start ()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].tabView = this;
            tabButtons[i].tabIndex = i;
        }
        yield return new WaitForEndOfFrame();
        SelectTab(0);
	}

    /// <summary>
    /// 设置切换页码
    /// </summary>
    /// <param name="index"></param>
    public void SelectTab(int index)
    {
        if (this.index != index)
        {
            for (int i = 0; i < tabButtons.Length; i++)
            {
                tabButtons[i].Select(i == index);
                if (i < tabPages.Length)
                {
                    tabPages[i].SetActive(i == index);
                }
            }
            if (OnTabSelect != null)
            {
                OnTabSelect(index);
            }
        }
    }
	

}
