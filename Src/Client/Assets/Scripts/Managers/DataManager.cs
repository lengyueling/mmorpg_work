using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Text;
using System;
using System.IO;
using Common.Data;
using Newtonsoft.Json;

public class DataManager : Singleton<DataManager>
{
    public string DataPath;
    public Dictionary<int, MapDefine> Maps = null;
    public Dictionary<int, CharacterDefine> Characters = null;
    public Dictionary<int, TeleporterDefine> Teleporters = null;
    public Dictionary<int, Dictionary<int, SpawnPointDefine>> SpawnPoints = null;
    public Dictionary<int, NpcDefine> Npcs = null;
    public Dictionary<int, ItemDefine> Items = null;
    public Dictionary<int, ShopDefine> Shops = null;
    public Dictionary<int, Dictionary<int, ShopItemDefine>> ShopItems = null;
    public Dictionary<int, EquipDefine> Equips = null;
    public Dictionary<int, QuestDefine> Quests = null;


    public DataManager()
    {
        this.DataPath = "Data/";
        Debug.LogFormat("DataManager > DataManager()");
    }

    /// <summary>
    /// 服务端加载数据
    /// </summary>
    public void Load()
    {
        string json = File.ReadAllText(this.DataPath + "MapDefine.txt");
        this.Maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);

        json = File.ReadAllText(this.DataPath + "CharacterDefine.txt");
        this.Characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);

        json = File.ReadAllText(this.DataPath + "TeleporterDefine.txt");
        this.Teleporters = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);

        json = File.ReadAllText(this.DataPath + "SpawnPointDefine.txt");
        this.SpawnPoints = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnPointDefine>>> (json);

        json = File.ReadAllText(this.DataPath + "NpcDefine.txt");
        this.Npcs = JsonConvert.DeserializeObject<Dictionary<int, NpcDefine>>(json);

        json = File.ReadAllText(this.DataPath + "ItemDefine.txt");
        this.Items = JsonConvert.DeserializeObject<Dictionary<int, ItemDefine>>(json);

        json = File.ReadAllText(this.DataPath + "ShopDefine.txt");
        this.Shops = JsonConvert.DeserializeObject<Dictionary<int, ShopDefine>>(json);

        json = File.ReadAllText(this.DataPath + "ShopItemDefine.txt");
        this.ShopItems = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, ShopItemDefine>>>(json);

        json = File.ReadAllText(this.DataPath + "EquipDefine.txt");
        this.Equips = JsonConvert.DeserializeObject<Dictionary<int, EquipDefine>>(json);
    }

    /// <summary>
    /// 客户端加载数据
    /// </summary>
    /// <returns></returns>
    public IEnumerator LoadData()
    {
        string json = File.ReadAllText(this.DataPath + "MapDefine.txt");
        this.Maps = JsonConvert.DeserializeObject<Dictionary<int, MapDefine>>(json);

        yield return null;

        json = File.ReadAllText(this.DataPath + "CharacterDefine.txt");
        this.Characters = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);

        yield return null;

        json = File.ReadAllText(this.DataPath + "TeleporterDefine.txt");
        this.Teleporters = JsonConvert.DeserializeObject<Dictionary<int, TeleporterDefine>>(json);

        yield return null;

        json = File.ReadAllText(this.DataPath + "SpawnPointDefine.txt");
        this.SpawnPoints = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SpawnPointDefine>>>(json);

        json = File.ReadAllText(this.DataPath + "NpcDefine.txt");
        this.Npcs = JsonConvert.DeserializeObject<Dictionary<int, NpcDefine>>(json);

        json = File.ReadAllText(this.DataPath + "ItemDefine.txt");
        this.Items = JsonConvert.DeserializeObject<Dictionary<int, ItemDefine>>(json);

        json = File.ReadAllText(this.DataPath + "ShopDefine.txt");
        this.Shops = JsonConvert.DeserializeObject<Dictionary<int, ShopDefine>>(json);

        json = File.ReadAllText(this.DataPath + "ShopItemDefine.txt");
        this.ShopItems = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, ShopItemDefine>>>(json);

        json = File.ReadAllText(this.DataPath + "EquipDefine.txt");
        this.Equips = JsonConvert.DeserializeObject<Dictionary<int, EquipDefine>>(json);

        json = File.ReadAllText(this.DataPath + "QuestDefine.txt");
        this.Quests = JsonConvert.DeserializeObject<Dictionary<int, QuestDefine>>(json);

        yield return null;
    }

#if UNITY_EDITOR
    public void SaveTeleporters()
    {
        string json = JsonConvert.SerializeObject(this.Teleporters, Formatting.Indented);
        File.WriteAllText(this.DataPath + "TeleporterDefine.txt", json);
    }

    public void SaveSpawnPoints()
    {
        string json = JsonConvert.SerializeObject(this.SpawnPoints, Formatting.Indented);
        File.WriteAllText(this.DataPath + "SpawnPointDefine.txt", json);
    }

#endif
}
