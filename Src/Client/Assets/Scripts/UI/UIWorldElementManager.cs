using Entities;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager> {

	public GameObject nameBarPrefab;//角色头顶的信息条
	public GameObject npcStatusPrefab;//NPC状态条

    private Dictionary<Transform, GameObject> elementNames = new Dictionary<Transform, GameObject>();
	private Dictionary<Transform, GameObject> elementStatus = new Dictionary<Transform, GameObject>();

    protected override void OnStart()
    {
        nameBarPrefab.SetActive(false);
    }

	public void AddCharacterNameBar(Transform owner, Character character)
	{
		GameObject goNameBar = Instantiate(nameBarPrefab, this.transform);//默认生成在当前管理器的根节点下
		goNameBar.name = "NameBar" + character.entityId;//给每个信息条的游戏对象添加名字 后方是当前角色对象的ID
		goNameBar.GetComponent<UIWorldElement>().owner = owner;
        goNameBar.GetComponent<UINameBar>().character = character;
		goNameBar.SetActive(true);
		this.elementNames[owner] = goNameBar;
    }

    public void RemoveCharacterNameBar(Transform owner)
    {
		if(this.elementNames.ContainsKey(owner))
        {
			Destroy(this.elementNames[owner]);
            this.elementNames.Remove(owner);
        }
    }

    public void AddNpcQuestStatus(Transform owner, NpcQuestStatus status)
    {
        if (this.elementStatus.ContainsKey(owner))
        {
            elementStatus[owner].GetComponent<UIQuestStatus>().SetQuestStatus(status);
        }
        else
        {
            GameObject go = Instantiate(npcStatusPrefab, this.transform);//默认生成在当前管理器的根节点下
            go.name = "NpcQuestStatus" + owner.name;
            go.GetComponent<UIWorldElement>().owner = owner;
            go.GetComponent<UIQuestStatus>().SetQuestStatus(status);
            go.SetActive(true);
            this.elementStatus[owner] = go;
        }
    }

    public void RemoveNpcQuestStatus(Transform owner)
    {
        if (this.elementStatus.ContainsKey(owner))
        {
            Destroy(this.elementStatus[owner]);
            this.elementStatus.Remove(owner);
        }
    }
}
