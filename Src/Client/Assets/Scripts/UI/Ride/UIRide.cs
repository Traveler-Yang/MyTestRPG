using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;

public class UIRide : UIWindow
{
    public GameObject itemPrefab;
    public Text descript;//×øÆïÃèÊö
    public ListView listMain;//×øÆïÁÐ±í¿ò
    public UIRideItem selectedItem;//Ñ¡ÖÐµÄ×øÆï

    void Start()
    {
        this.listMain.onItemSelected += OnRideItemSelected;
        RefreshUI();
    }

    public void OnRideItemSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIRideItem;
        this.descript.text = this.selectedItem.item.Define.Description;
    }

    public void RefreshUI()
    {
        ClearItems();
        InitItems();
    }

    public void ClearItems()
    {
        this.listMain.RemoveAll();
    }

    public void InitItems()
    {
        foreach (var item in ItemManager.Instance.Items)
        {
            if (item.Value.Define.ID / 1000 == 8 &&
                item.Value.Define.LimitClass == CharacterClass.None)
            {
                if (EquipManager.Instance.Contains(item.Key))
                    continue;
                GameObject go = Instantiate(itemPrefab, listMain.transform);
                UIRideItem ui = go.GetComponent<UIRideItem>();
                ui.SetRideItem(item.Value, this, false);
                this.listMain.AddItem(ui);
            }
        }
    }

    public void DoRide()
    {
        if (this.selectedItem == null)
        {
            MessageBox.Show("ÇëÑ¡ÔñÒªÕÙ»½µÄ×øÆï", "ÕÙ»½×øÆï");
            return;
        }
        User.Instance.Ride(this.selectedItem.item.Id);
    }
}
