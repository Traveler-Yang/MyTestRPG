using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestStatus : MonoBehaviour
{
    public Image[] statusImages; //状态图标数组

    private NpcQuestStatus questStatus; //当前状态

    void Start()
    {
        
    }

    public void SetQuestStatus(NpcQuestStatus status)
    {
        this.questStatus = status;

        for (int i = 0; i < 4; i++)
        {
            if (this.statusImages[i] != null)
            {
                this.statusImages[i].gameObject.SetActive(i == (int)status);
            }
        }
    }
}
