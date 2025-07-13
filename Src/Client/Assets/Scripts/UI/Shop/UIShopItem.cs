using Common.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShopItem : MonoBehaviour, ISelectHandler
{
    public Image icon;//图标
    public Text title;//商品名字
    public TextMeshProUGUI price;//价格
    public Text limitClass;//职业类型
    public TextMeshProUGUI count;//数量


    public Image backGround;//商品的背景图
    public Sprite normalBg;//正常状态的背景图
    public Sprite selectBg;//选中状态的背景图

    private bool selected;
    /// <summary>
    /// 是否被选中
    /// </summary>
    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            //每次赋值就会改变当前背景图
            backGround.overrideSprite = selected ? selectBg : normalBg;

        }
    }

    public int ShopItemID { get; set; }
    private UIShop shop;

    private ItemDefine item;
    private ShopItemDefine shopItem {  get; set; }

    /// <summary>
    /// 设置商店物品基本信息
    /// </summary>
    /// <param name="id">物品的ID</param>
    /// <param name="shopItem">哪一个物品</param>
    /// <param name="owner">我属于哪个商店</param>
    public void SetShopItem(int id, ShopItemDefine shopItem, UIShop owner)
    {
        this.shop = owner;
        this.ShopItemID = id;
        this.shopItem = shopItem;
        this.item = DataManager.Instance.Items[this.shopItem.ItemID];

        this.title.text = this.item.Name;
        this.count.text = "X" + shopItem.Count.ToString();
        this.price.text = shopItem.Price.ToString();
        switch (item.LimitClass)
        {
            case SkillBridge.Message.CharacterClass.None:
                this.limitClass.text = "无";
                break;
            case SkillBridge.Message.CharacterClass.Warrior:
                this.limitClass.text = "战士";
                break;
            case SkillBridge.Message.CharacterClass.Wizard:
                this.limitClass.text = "法师";
                break;
            case SkillBridge.Message.CharacterClass.Archer:
                this.limitClass.text = "弓箭手";
                break;
        }
        this.icon.overrideSprite = Resloader.Load<Sprite>(item.Icon);
    }

    /// <summary>
    /// 点击商店物品事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnSelect(BaseEventData eventData)
    {
        this.Selected = true;
        this.shop.SelectShopItem(this);
    }
}
