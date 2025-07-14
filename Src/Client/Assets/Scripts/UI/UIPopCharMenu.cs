using Managers;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPopCharMenu : UIWindow, IDeselectHandler
{
    //目标id
    public int targetId;
    //目标昵称
    public string targetName;

    public void OnDeselect(BaseEventData eventData)
    {
        var ed = eventData as PointerEventData;
        if (ed.hovered.Contains(this.gameObject))
            return;
        this.Close(UIWindowResult.None);
    }

    private void OnEnable()
    {
        this.GetComponent<Selectable>().Select();
        this.Root.transform.position = Input.mousePosition + new Vector3(80, 0, 0);
    }

    /// <summary>
    /// 私聊
    /// </summary>
    public void OnChat()
    {
        ChatManager.Instance.StartPrivateChat(targetId, targetName);
        this.Close(UIWindowResult.None);
    }

    /// <summary>
    /// 添加好友
    /// </summary>
    public void OnAddFriend()
    {
        FriendService.Instance.SendFriendAddRequest(targetId, targetName);
        this.Close(UIWindowResult.None);
    }

    /// <summary>
    /// 邀请组队
    /// </summary>
    public void OnInviteTemp()
    {
        TempService.Instance.SendTempInviteRequest(targetId, targetName);
        this.Close(UIWindowResult.None);
    }
}
