using Candlelight.UI;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;

public class UIChat : MonoBehaviour
{
    public HyperText textArea;//����������ʾ��

    public TabView channelTab;//��ť��

    public InputField chatText;//���������
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
        //�ҵ�ǰ����������Ƿ��н��㣬����н��㣬�������������
        InputManager.Instance.IsInputMode = chatText.isFocused;
    }

    private void OnDisPlayChannelSelected(int idx)
    {
        //ѡ��Ƶ��ʱ����ѡ���Ƶ����������������
        ChatManager.Instance.displayChannel = (ChatManager.LocalChannel)idx;
        //ѡ���ˢ��UI
        RefreshUI();
    }

    /// <summary>
    /// ˢ��UI
    /// </summary>
    public void RefreshUI()
    {
        //����ȡ���ĵ�ǰת�����ı���������Ϣ��ֵ����ǰUI
        this.textArea.text = ChatManager.Instance.GetCurrentMassage();
        this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
        //�����ǰ���͵���˽�ģ���
        if (ChatManager.Instance.SendChannel == ChatChannel.Private)
        {
            this.target.gameObject.SetActive(true);//�������
            if (ChatManager.Instance.PrivateID != 0)//˽�Ķ���Ϊnull
            {
                this.chatTarget.text = string.Format("{0} :", ChatManager.Instance.PrivateName);//��˽�Ķ������ָ�ֵ
            }
            else
                this.chatTarget.text = "<��> :";
        }
        else
        {
            this.target.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ����������¼�
    /// </summary>
    /// <param name="text"></param>
    /// <param name="link">����</param>
    public void OnClickChatLink(HyperText text, HyperText.LinkInfo link)
    {
        //�ж��������name�Ƿ�Ϊnull����ַ�
        if (string.IsNullOrEmpty(link.Name))
            return;
        //<a name="c:1001:name" class="player">Name</a>
        //<a name="i:1001:name" class="item">Name</a>
        if (link.Name.StartsWith("c:"))//���ݿ�ͷ�ַ����ж���ʲô����
        {
            string[] strs = link.Name.Split(":".ToCharArray());
            UIPopCharMenu menu = UIManager.Instance.Show<UIPopCharMenu>();
            menu.targetId = int.Parse(strs[1]);
            menu.targetName = strs[2];
        }
    }

    /// <summary>
    /// ������Ϣ��ť
    /// </summary>
    public void OnClickSend()
    {
        OnEndInput(this.chatText.text);
    }

    /// <summary>
    /// ����������Ϣ�¼�
    /// </summary>
    /// <param name="text"></param>
    public void OnEndInput(string text)
    {
        //���������null
        if (string.IsNullOrEmpty(text.Trim()))
            this.SendChat(text);//������Ϣ
        this.chatText.text = "";//�������Ϣ��
    }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    /// <param name="content"></param>
    private void SendChat(string content)
    {
        ChatManager.Instance.SendChat(content);
    }

    public void OnSendChannelChanged(int idx)
    {
        if (ChatManager.Instance.sendChannel == (ChatManager.LocalChannel)(idx + 1))
            return;

        //�����ǰƵ����Ҫ�л���Ƶ������ͬ���Ÿ�ֵ
        if (!ChatManager.Instance.SetSendChannel((ChatManager.LocalChannel)idx + 1))
        {
            this.channelSelect.value = (int)(ChatManager.Instance.sendChannel - 1);
        }
        else
        {
            RefreshUI();
        }
    }
}
