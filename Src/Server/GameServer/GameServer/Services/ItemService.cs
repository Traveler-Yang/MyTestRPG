using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    internal class ItemService : Singleton<ItemService>
    {

        public ItemService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemBuyRequest>(this.OnBuyItem);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemEquipRequset>(this.OnItemEquip);
        }

        public void Init()
        {

        }

        private void OnBuyItem(NetConnection<NetSession> sender,ItemBuyRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnBuyItem : : Character:{0} ShopID:{1} ShopItemID{2}",character, request.shopId, request.shopItemId);

            var result = ShopManager.Instance.BuyItem(sender, request.shopId, request.shopItemId);
            sender.Session.Response.itemBuy = new ItemBuyResponse();
            sender.Session.Response.itemBuy.Result = result;
            sender.SendResPonse();
        }
        private void OnItemEquip(NetConnection<NetSession> sender, ItemEquipRequset requset)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnItemEquip : : Character:{0} Slot:{1} ItemId{2} IsEquip{3}", character, requset.Slot, requset.itemId, requset.isEquip);
            var result = EquipManager.Instance.EquipItem(sender, requset.Slot, requset.itemId, requset.isEquip);
            sender.Session.Response.itemEquip = new ItemEquipResponse();
            sender.Session.Response.itemEquip.Result = Result.Success;
            sender.Session.Response.itemEquip.Errormsg = "None";
            sender.SendResPonse();
        }
    }
}
