using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;
using TMPro;
using Services;

public class UIChat : MonoBehaviour
{
    public TextMeshProUGUI textArea;//聊天内容显示区

    public TabView channelTab;//按钮区

    public InputField chatText;//输入聊天框
    public GameObject target;
    public Text chatTarget;

    public Dropdown channelSelect;

    void Start()
    {
        this.channelTab.OnTabSelect += OnDisPlayChannelSelected;
        ChatManager.Instance.OnChat += RefreshUI;
    }

    private void OnDestroy()
    {
        ChatManager.Instance.OnChat -= RefreshUI;
    }

    // Update is called once per frame
    void Update()
    {
        //我当前的输入框上是否有焦点，如果有焦点，则代表我在输入
        InputManager.Instance.IsInputMode = chatText.isFocused;
    }

    private void OnDisPlayChannelSelected(int idx)
    {
        //选择频道时，将选择的频道的索引给管理器
        ChatManager.Instance.displayChannel = (ChatManager.LocalChannel)idx;
        //选择后刷新UI
        RefreshUI();
    }

    /// <summary>
    /// 刷新UI
    /// </summary>
    public void RefreshUI()
    {
        //将获取到的当前转换成文本的聊天信息赋值给当前UI
        this.textArea.text = ChatManager.Instance.GetCurrentMassage();
        this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
        //如果当前发送的是私聊，则
        if (ChatManager.Instance.SendChannel == ChatChannel.Private)
        {
            this.target.gameObject.SetActive(true);//启用组件
            if (ChatManager.Instance.PrivateID != 0)//私聊对象不为null
            {
                this.chatTarget.text = string.Format("{0} :", ChatManager.Instance.PrivateName);//将私聊对象名字赋值
            }
            else
                this.chatTarget.text = "<无> :";
        }
        else
        {
            this.target.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 发送消息按钮
    /// </summary>
    public void OnClickSend()
    {
        OnEndInput(this.chatText.text);
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Btn_1);
    }

    /// <summary>
    /// 结束发送消息事件
    /// </summary>
    /// <param name="text"></param>
    public void OnEndInput(string text)
    {
        //如果不等于null，则发送消息
        if (!string.IsNullOrEmpty(text.Trim()))
            this.SendChat(text);//发送消息
        this.chatText.text = "";//并清空消息框
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="content"></param>
    private void SendChat(string content)
    {
        ChatManager.Instance.SendChat(content, ChatManager.Instance.PrivateID, ChatManager.Instance.PrivateName);
    }

    /// <summary>
    /// 切换频道
    /// </summary>
    /// <param name="idx">当前切换的频道索引</param>
    public void OnSendChannelChanged(int idx)
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Btn_1);
        //如果当前选择的频道和当前频道一致，则不切换频道，直接返回
        if (ChatManager.Instance.sendChannel == (ChatManager.LocalChannel)(idx + 1))
            return;

        //如果当前频道和要切换的频道不相同，才赋值
        //先更改Manager中的当前频道，如果更改成功，则更改UI的索引
        if (!ChatManager.Instance.SetSendChannel((ChatManager.LocalChannel)idx + 1))
        {
            this.channelSelect.value = (int)(ChatManager.Instance.sendChannel - 1);
        }
        else
        {
            //如果未更改成功，则刷新UI（无队伍，无公会才会返回失败）
            RefreshUI();
        }
    }
}
