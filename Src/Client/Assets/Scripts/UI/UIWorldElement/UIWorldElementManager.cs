using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager> {

    public GameObject nameBarPrefab;

    private Dictionary<Transform, GameObject> elements = new Dictionary<Transform, GameObject>();

    /// <summary>
    /// 加入跟随角色的世界空间UI
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="character"></param>
    public void AddCharacterNameBar(Transform owner, Character character)
    {
        GameObject goNameBar = Instantiate(nameBarPrefab, this.transform);
        goNameBar.name = "NameBar" + character.entityId;
        goNameBar.GetComponent<UIWorldElement>().owner = owner;
        goNameBar.GetComponent<UINameBar>().character = character;
        goNameBar.SetActive(true);
        this.elements[owner] = goNameBar;
    }
    /// <summary>
    /// 移除跟随角色的世界空间UI
    /// </summary>
    /// <param name="owner"></param>
    public void RemoveCharacterNameBar(Transform owner)
    {
        if (this.elements.ContainsKey(owner))
        {
            Destroy(this.elements[owner]);
            this.elements.Remove(owner);
        }
    }
}
