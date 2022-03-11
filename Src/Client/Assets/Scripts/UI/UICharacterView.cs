using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterView : MonoBehaviour {

    public GameObject[] characters;


    private int currentCharacter = 0;

    public int CurrectCharacter
    {
        get
        {
            return currentCharacter;
        }
        set
        {
            currentCharacter = value;
            this.UpdateCharacter();
        }
    }

    /// <summary>
    /// 更新职业是否可见
    /// </summary>
    void UpdateCharacter()
    {
        for(int i=0;i<3;i++)
        {
            characters[i].SetActive(i == this.currentCharacter);
        }
    }
}
