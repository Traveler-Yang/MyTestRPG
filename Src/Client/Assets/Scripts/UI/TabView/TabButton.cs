using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    public Sprite activeImage;//激活时的图片
    private Sprite normalImage;//正常时的图片

    public TabView tabView;//这个按钮的所有者

    public int tabIndex = 0;//按钮的索引
    public bool selected = false;

    private Image tabImage;//当前图片

    void Start()
    {
        //将当前按钮身上的的图片取出来，赋值给正常时的图片
        tabImage = GetComponent<Image>();
        normalImage = tabImage.sprite;

        //给当前身上的按钮添加一个点击事件
        this.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    /// <summary>
    /// 更改图片
    /// </summary>
    /// <param name="select"></param>
    public void Select(bool select)
    {
        tabImage.overrideSprite = select ? activeImage : normalImage;
    }

    void OnClick()
    {
        this.tabView.SelectTab(this.tabIndex);
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Btn_1);
    }
}
