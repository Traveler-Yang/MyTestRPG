using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using UnityEngine;

namespace Services
{
    internal class ItemService : Singleton<ItemService>, IDisposable
    {

        public ItemService()
        {
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(this.OnBuyItem);
            MessageDistributer.Instance.Subscribe<ItemEquipResponse>(this.OnItemEquip);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(this.OnBuyItem);
            MessageDistributer.Instance.Unsubscribe<ItemEquipResponse>(this.OnItemEquip);
        }

        public void SendBuyItem(int shopId, int ShopItemId)
        {
            Debug.LogFormat("SendBuyItem : Shop:{0} Item{1}", shopId, ShopItemId);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemBuy = new ItemBuyRequest();
            message.Request.itemBuy.shopId = shopId;
            message.Request.itemBuy.shopItemId = ShopItemId;
            NetClient.Instance.SendMessage(message);
        }

        public void OnBuyItem(object sender, ItemBuyResponse response)
        {
            Debug.LogFormat("OnBuyItem:{0} [{1}]", response.Result, response.Errormsg);

            if (response.Result == Result.Success)
            {
                MessageBox.Show("购买结果" + response.Result + "\n" + response.Errormsg, "购买完成");
                SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Shop_Buy);
            }
        }

        Item pendingEquip = null;//当前角色穿的装备信息
        bool isEquip;//当前是穿还是脱的
        public bool SendItemEquip(Item equip, bool isEquip)
        {
            if (pendingEquip != null)
                return false;
            Debug.Log("SendItemEquip");

            pendingEquip = equip;
            this.isEquip = isEquip;

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemEquip = new ItemEquipRequset();
            message.Request.itemEquip.Slot = (int)equip.EquipInfo.Slot;
            message.Request.itemEquip.itemId = equip.Id;
            message.Request.itemEquip.isEquip = isEquip;
            NetClient.Instance.SendMessage(message);
            return true;
        }

        public void OnItemEquip(object sender, ItemEquipResponse response)
        {
            Debug.LogFormat("");

            if (response.Result == Result.Success)
            {
                if(pendingEquip != null)
                {
                    if (this.isEquip)
                        EquipManager.Instance.OnEquipItem(pendingEquip);
                    else
                        EquipManager.Instance.OnUnEquipItem(pendingEquip.EquipInfo.Slot);
                    pendingEquip = null;
                }
                SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Equip);
            }
        }

    }
}
