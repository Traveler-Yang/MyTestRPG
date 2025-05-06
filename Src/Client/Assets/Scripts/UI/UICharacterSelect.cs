using Models;
using Services;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterSelect : MonoBehaviour {

    public GameObject playerFoundPanel;//选择角色界面
    public GameObject characterSelectPanel;//创建角色界面

    CharacterClass charClass;

    public Text descs;//职业描述

    public Text[] names;//职业名字

    public Text characterName;//角色昵称

    public List<GameObject> uiChars = new List<GameObject>();

    public Transform uiCharactersList;

    public GameObject uiCharInfo;

    public UICharacterView characterView;

    private int selectCharacterIdx = -1;

    void Start () {
        InitCharacterSelect(true);
        UserService.Instance.OnCharacterCreate = OnCharacterCreate;//将创建角色是否成功的消息储存起来

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitCharacterSelect(bool init)
    {
        playerFoundPanel.SetActive(true);
        characterSelectPanel.SetActive(false);
        if (init)
        {
            foreach (var old in uiChars)
            {
                Destroy(old);
            }
            uiChars.Clear();

            for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)//遍历User列表中的所有角色
            {
                GameObject go = Instantiate(uiCharInfo, uiCharactersList);//生成一个角色选择的Prefab
                UICharInfo charInfo = go.GetComponent<UICharInfo>();//获取选择角色的Prefab身上的UICharInfo脚本
                charInfo.info = User.Instance.Info.Player.Characters[i];

                Button button = go.GetComponent<Button>();//获取生成的Prefab的按钮组件
                int idx = i;
                button.onClick.AddListener(() =>//给按钮组件上的事件加上一个点击事件
                {
                    OnSelectCharacter(idx);
                    Onclass(charInfo.info.Class);
                });

                uiChars.Add(go);//给选择列表中添加一个对象
                go.SetActive(true);//将此对象设置为启用
            }
        }
    }

    public void OnSelectCharacter(int idx)
    {
        this.selectCharacterIdx = idx;
        var cha = User.Instance.Info.Player.Characters[idx];
        Debug.LogFormat("Select Char:[{0}]{1}[{2}]", cha.Id, cha.Name, cha.Class);
        User.Instance.CurrentCharacter = cha;
        characterView.CurrectCharacter = idx;
        
        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            UICharInfo ci = this.uiChars[i].GetComponent<UICharInfo>();
            ci.Selected = idx == i;
        }
    }

    public void OnCharacterCreate()
    {
        if (string.IsNullOrEmpty(characterName.text))
        {
            MessageBox.Show("请输入昵称");
            return;
        }
        UserService.Instance.SendCharacterCreate(this.characterName.text, charClass);//输入完昵称，创建角色，将信息发送给UserService
        //SceneManager.Instance.LoadScene("MainCity");
    }

    void OnCharacterCreate(Result result, string message)
    {
        if (result == Result.Success)
        {
            InitCharacterSelect(true);
        }
        else
        {
            MessageBox.Show(message, "错误 {0}", MessageBoxType.Error);
        }
    }
    /// <summary>
    /// 创建角色时的角色显隐
    /// </summary>
    /// <param name="charClass"></param>
    public void OnSelectClass(int charClass)
    {
        this.charClass = (CharacterClass)charClass;

        characterView.CurrectCharacter = charClass - 1;

        for (int i = 0; i < 3; i++)
        {
            names[i].text = DataManager.Instance.Characters[i + 1].Name;//将职业名字依次赋值给UI
        }

        descs.text = DataManager.Instance.Characters[charClass].Description;//将职业描述赋值到UI

    }
    public void Onclass(CharacterClass charClass)
    {
        this.charClass = (CharacterClass)charClass;

        characterView.CurrectCharacter = (int)charClass;
    }
    /// <summary>
    /// 进入游戏
    /// </summary>
    public void OnClickPlay()
    {
        if (selectCharacterIdx >= 0)
        {
            UserService.Instance.SendGameEnter(selectCharacterIdx);
        }
    }
}
