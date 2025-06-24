using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIIconItem : MonoBehaviour
{
    public Image mainImage;
    public Image secondImage;

    public TextMeshProUGUI mainText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 设置物品图标
    /// </summary>
    /// <param name="iconName">图标名称（ID）</param>
    /// <param name="text">物品数量</param>
    public void SetMainIcon(string iconName, string text)
    {
        this.mainImage.overrideSprite = Resloader.Load<Sprite>(iconName);
        if (this.mainText != null) this.mainText.text = text;
    }
}
