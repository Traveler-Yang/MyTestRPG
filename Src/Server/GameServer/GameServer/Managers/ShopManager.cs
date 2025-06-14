using Common;
using Common.Data;
using GameServer.Services;
using Network;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class ShopManager : Singleton<ShopManager>
    {
        /// <summary>
        /// 购买物品
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="shopId">商店ID</param>
        /// <param name="ShopItemId">购买的物品ID</param>
        /// <returns></returns>
        public Result BuyItem(NetConnection<NetSession> sender, int shopId, int ShopItemId)
        {
            //验证商店ID是否正确
            if (!DataManager.Instance.Shops.ContainsKey(shopId))
                return Result.Failed;

            ShopItemDefine shopItem;
            //验证物品ID是否存在
            if (DataManager.Instance.ShopItems[shopId].TryGetValue(ShopItemId, out shopItem))
            {
                Log.InfoFormat("BuyItem: :character:{0}:Item:{1} Count:{2} Price{3}", sender.Session.Character.Id, shopItem.ItemID, shopItem.Count, shopItem.Price);
                //这里验证我的钱是否是大于等于要购买的商品的价格的
                if (sender.Session.Character.Gold >= shopItem.Price)
                {
                    //购买验证通过，添加物品，扣钱，异步保存到数据库
                    sender.Session.Character.ItemManager.AddItem(shopItem.ItemID, shopItem.Count);
                    sender.Session.Character.Gold -= shopItem.Price;
                    DBService.Instance.Save();
                    return Result.Success;
                }
            }
            return Result.Failed;
        }
    }
}
