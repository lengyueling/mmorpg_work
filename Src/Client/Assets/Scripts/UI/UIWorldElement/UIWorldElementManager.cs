using Entities;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager> {

    public GameObject nameBarPrefab;
    public GameObject npcStatusPrefab;

    private Dictionary<Transform, GameObject> elementNames = new Dictionary<Transform, GameObject>();
    private Dictionary<Transform, GameObject> elementStatus = new Dictionary<Transform, GameObject>();


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
        this.elementNames[owner] = goNameBar;
    }
    /// <summary>
    /// 移除跟随角色的世界空间UI
    /// </summary>
    /// <param name="owner"></param>
    public void RemoveCharacterNameBar(Transform owner)
    {
        if (this.elementNames.ContainsKey(owner))
        {
            Destroy(this.elementNames[owner]);
            this.elementNames.Remove(owner);
        }
    }

    /// <summary>
    /// 加入Npc状态UI
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="status"></param>
    public void AddNpcQuestStatus(Transform owner, NpcQuestStatus status)
    {
        if (this.elementStatus.ContainsKey(owner))
        {
            elementStatus[owner].GetComponent<UIQuestStatus>().SetQuestStatus(status);
        }
        else
        {
            GameObject go = Instantiate(npcStatusPrefab, this.transform);
            go.name = "NpcQuestStatus" + owner.name;
            go.GetComponent<UIWorldElement>().owner = owner;
            go.GetComponent<UIQuestStatus>().SetQuestStatus(status);
            go.SetActive(true);
            this.elementStatus[owner] = go;
        }
    }

    /// <summary>
    /// 移除Npc状态UI
    /// </summary>
    /// <param name="owner"></param>
    public void RemoveNpcQuestStatus(Transform owner)
    {
        if (this.elementStatus.ContainsKey(owner))
        {
            Destroy(this.elementStatus[owner]);
            this.elementStatus.Remove(owner);
        }
    }
}
