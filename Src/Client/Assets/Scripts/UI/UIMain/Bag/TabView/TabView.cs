using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabView : MonoBehaviour
{
    /// <summary>
    /// 几号背包按钮上的脚本
    /// </summary>
    public TabButton[] tabButtons;
    /// <summary>
    /// 几号背包的物体
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
    /// 设置背包
    /// </summary>
    /// <param name="index"></param>
    public void SelectTab(int index)
    {
        if (this.index != index)
        {
            for (int i = 0; i < tabButtons.Length; i++)
            {
                tabButtons[i].Select(i == index);
                tabPages[i].SetActive(i == index);
            }
        }
    }
	

}
