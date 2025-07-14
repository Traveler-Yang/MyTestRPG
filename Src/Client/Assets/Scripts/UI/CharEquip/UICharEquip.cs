using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Managers;
using System;
using Models;
using SkillBridge.Message;

public class UICharEquip : UIWindow
{
    public Text title;
    public TextMeshProUGUI money;

    public GameObject itemPrefab;
    public GameObject itemEquipPrefab;

    public Transform itemListRoot;

    public List<Transform> slots;
    void Start()
    {
        RefreshUI();
        EquipManager.Instance.OnEquipChanged += RefreshUI;
    }

    private void OnDestroy()
    {
        EquipManager.Instance.OnEquipChanged -= RefreshUI;
    }

    private void RefreshUI()
    {
        ClearAllEquipList();
        InitAllEquipItems();
        ClearEquipedList();
        InitEquipedList();
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    private void InitAllEquipItems()
    {
        foreach (var kv in ItemManager.Instance.Items)
        {
            //如果是装备类型，并且类型是当前的职业
            if (kv.Value.Define.Type == ItemType.Equip && kv.Value.Id/1000 == (int)User.Instance.CurrentCharacter.Class)
            {
                //已经装备就不显示了
                if (EquipManager.Instance.Contains(kv.Key))
                    continue;
                GameObject go = Instantiate(itemPrefab, itemListRoot);
                UIEquipItem ui = go.GetComponent<UIEquipItem>();
                ui.SetEquipItem(kv.Key, kv.Value, this, false);
            }
        }
    }
    private void ClearAllEquipList()
    {
        foreach (var item in itemListRoot.GetComponentsInChildren<UIEquipItem>())
        {
            Destroy(item.gameObject);
        }
    }
    private void ClearEquipedList()
    {
        foreach (var item in slots)
        {
            if (item.childCount > 0 && item.GetComponentInChildren<UIEquipItem>())
                Destroy(item.GetComponentInChildren<UIEquipItem>().gameObject);
        }
    }

    private UIEquipItem selsecEquipItem;
    public void SelectEquipItem(UIEquipItem item)
    {
        if (selsecEquipItem != null)
            selsecEquipItem.Selected = false;
        selsecEquipItem = item;
    }

    /// <summary>
    /// 初始化已经装备的列表
    /// </summary>
    private void InitEquipedList()
    {
        for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
        {
            var item = EquipManager.Instance.Equips[i];
            {
                if (item != null)
                {
                    GameObject go = Instantiate(itemEquipPrefab, slots[i]);
                    UIEquipItem ui = go.GetComponent<UIEquipItem>();
                    ui.SetEquipItem(i, item, this, true);
                }
            }
        }
    }

    public void DoEquip(Item item)
    {
        EquipManager.Instance.EquipItem(item);
    }

    public void UnEquip(Item item)
    {
        EquipManager.Instance.UnEquipItem(item);
    }
}
