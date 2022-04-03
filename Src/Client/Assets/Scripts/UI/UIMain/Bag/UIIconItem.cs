using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIconItem : MonoBehaviour
{
    public Image mainImage;
    public Image secondImage;
    public Text mainText;

    /// <summary>
    /// 显示道具icon和数量
    /// </summary>
    /// <param name="iconName"></param>
    /// <param name="text"></param>
    public void SetMainIcon(string iconName, string text)
    {
        this.mainImage.overrideSprite = Resloader.Load<Sprite>(iconName);
        this.mainText.text = text;
    }

}
