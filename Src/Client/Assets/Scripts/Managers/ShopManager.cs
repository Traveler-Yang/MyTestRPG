using Common.Data;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    internal class ShopManager : Singleton<ShopManager>
    {
        public void Init()
        {
            NPCManager.Instance.RegisterNpcEvent(NpcFunction.InvokeShop, OnOpenShop);
        }

        private bool OnOpenShop(NpcDefine npc)
        {
            this.ShowShop(npc.Param);
            return true;
        }

        /// <summary>
        /// 打开商店
        /// </summary>
        /// <param name="shopId">商店的ID</param>
        public void ShowShop(int shopId)
        {
            ShopDefine shop;
            if (DataManager.Instance.Shops.TryGetValue(shopId, out shop))
            {
                UIShop uiShop = UIManager.Instance.Show<UIShop>();
                if (uiShop != null)
                    uiShop.SetShop(shop);
            }
        }

        /// <summary>
        /// 点击购买后向ItemService Send信息
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="ShopItemId"></param>
        /// <returns></returns>
        public bool BuyItem(int shopId, int ShopItemId)
        {
            ItemService.Instance.SendBuyItem(shopId, ShopItemId);
            return true;
        }
    }
}
