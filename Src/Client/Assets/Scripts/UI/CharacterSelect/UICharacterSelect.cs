using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Services;
using SkillBridge.Message;
using Common.Data;
public class UICharacterSelect : MonoBehaviour {

    public GameObject panelCreate;
    public GameObject panelSelect;

    public GameObject btnCreateCancel;

    public InputField charName;
    CharacterClass charClass;

    public Transform uiCharList;
    public GameObject uiCharInfo;

    public List<GameObject> uiChars = new List<GameObject>();

    public Image[] titles;

    public Text descs;

    public Text[] names;

    private int selectCharacterIdx = -1;

    public UICharacterView characterView;

    // Use this for initialization
    void Start()
    {
        InitCharacterSelect(true);
        UserService.Instance.OnCharacterCreate = OnCharacterCreate;
        //临时测试用，一般情况下会在登录界面加载数据
        //DataManager.Instance.Load();
    }

    /// <summary>
    /// 创建角色完成或者Start时，面板转换到选择角色界面
    /// 如果已有角色选择列表则销毁掉
    /// </summary>
    /// <param name="init">是否成功</param>
    public void InitCharacterSelect(bool init)
    {
        panelCreate.SetActive(false);
        panelSelect.SetActive(true);

        if (init)
        {
            foreach (var old in uiChars)
            {
                Destroy(old);
            }
            uiChars.Clear();
        }
        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            GameObject go = Instantiate(uiCharInfo, this.uiCharList);
            UICharInfo chrInfo = go.GetComponent<UICharInfo>();
            chrInfo.info = User.Instance.Info.Player.Characters[i];
            Button button = go.GetComponent<Button>();
            int idx = i;
            button.onClick.AddListener(() =>
            {
                OnSelectCharacter(idx);
            });
            uiChars.Add(go);
            go.SetActive(true);
        }
    }

    /// <summary>
    /// 返回选择角色界面
    /// </summary>
    public void InitCharacterCreate()
    {
        panelCreate.SetActive(true);
        panelSelect.SetActive(false);
    }

    /// <summary>
    /// 点击开始冒险（创建角色）
    /// </summary>
    public void OnClickCreate()
    {
        if (string.IsNullOrEmpty(this.charName.text))
        {
            MessageBox.Show("请输入角色名称");
            return;
        }
        if (this.charClass == CharacterClass.None)
        {
            MessageBox.Show("请选择职业");
            return;
        }
        UserService.Instance.SendCharacterCreate(this.charName.text, this.charClass);
    }

    /// <summary>
    /// 选择职业(创建职业)
    /// </summary>
    /// <param name="charClass"></param>
    public void OnSelectClass(int charClass)
    {
        this.charClass = (CharacterClass)charClass;

        characterView.CurrectCharacter = charClass - 1;

        for (int i = 0; i < 3; i++)
        {
            titles[i].gameObject.SetActive(i == charClass - 1);
            names[i].text = DataManager.Instance.Characters[i + 1].Name;
        }
        descs.text = DataManager.Instance.Characters[charClass].Description;

    }

    /// <summary>
    /// 委托函数
    /// </summary>
    /// <param name="result"></param>
    /// <param name="message"></param>
    void OnCharacterCreate(Result result, string message)
    {
        if (result == Result.Success)
        {
            InitCharacterSelect(true);

        }
        else
            MessageBox.Show(message, "错误", MessageBoxType.Error);
    }

    /// <summary>
    /// 选择职业（创建完成后）
    /// </summary>
    /// <param name="idx"></param>
    public void OnSelectCharacter(int idx)
    {
        this.selectCharacterIdx = idx;
        var cha = User.Instance.Info.Player.Characters[idx];
        Debug.LogFormat("Select Char:[{0}]{1}[{2}]", cha.Id, cha.Name, cha.Class);
        characterView.CurrectCharacter = (int)cha.Class - 1;
        
        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            UICharInfo ci = this.uiChars[i].GetComponent<UICharInfo>();
            ci.Selected = idx == i;
        }
    }

    /// <summary>
    /// 点击进入游戏（加载已有角色）
    /// </summary>
    public void OnClickPlay()
    {
        if (selectCharacterIdx >= 0)
        {
            //MessageBox.Show("进入游戏", "进入游戏", MessageBoxType.Confirm);
            UserService.Instance.SendGameEnter(selectCharacterIdx);
        }
    }
}
