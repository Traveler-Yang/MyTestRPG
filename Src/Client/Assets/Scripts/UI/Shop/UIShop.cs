using Common.Data;
using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : UIWindow
{
    public Text title;//标题
    public Text money;//金钱

    public GameObject shopItem;//商定物品预制体
    ShopDefine shop;
    public Transform[] itemRoot;//商店页面的根节点

    void Start()
    {
        StartCoroutine(InitItems());
    }

    /// <summary>
    /// 初始化商店的道具表
    /// </summary>
    /// <returns></returns>
    IEnumerator InitItems()
    {
        foreach (var kv in DataManager.Instance.ShopItems[shop.ID])
        {
            if (kv.Value.Status > 0)
            {
                GameObject go = Instantiate(shopItem, itemRoot[0]);
                UIShopItem ui = go.GetComponent<UIShopItem>();
                ui.SetShopItem(kv.Key, kv.Value, this);

            }
        }
        yield return null;
    }

    /// <summary>
    /// 设置商店的基本属性
    /// </summary>
    /// <param name="shop"></param>
    public void SetShop(ShopDefine shop)
    {
        this.shop = shop;
        this.title.text = shop.Name;
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    #region 选择哪个商店物品

    private UIShopItem selsecShopItem;
    /// <summary>
    /// 选择的哪个商品，并得到这个商品
    /// </summary>
    /// <param name="item">当前选择的商品</param>
    public void SelectShopItem(UIShopItem item)
    {
        if (selsecShopItem != null)
            selsecShopItem.Selected = false;
        selsecShopItem = item;
    }

    #endregion

    /// <summary>
    /// 点击购买
    /// </summary>
    public void OnClickBuy()
    {
        if (this.selsecShopItem == null)
        {
            MessageBox.Show("请选择要购买的道具", "购买物品");
            return;
        }
        if (!ShopManager.Instance.BuyItem(this.shop.ID, this.selsecShopItem.ShopItemID))
        {

        }
    }
}
