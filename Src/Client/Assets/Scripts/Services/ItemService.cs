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
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(this.OnBuyItem);
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
            }
        }
    }
}
