using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    /// <summary>
    /// UI组件信息
    /// </summary>
    class UIElement
    {
        public string Resource;     //资源路径
        public bool cache;          //是否缓存
        public GameObject instance; //UI实例
    }
    /// <summary>
    /// 存储UI资源信息
    /// </summary>
    private Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();

    public UIManager()
    {
        //初始化UI资源信息
        UIResources.Add(typeof(UITest), new UIElement() { Resource = "UI/UITest", cache = true });
        UIResources.Add(typeof(UIBag), new UIElement() { Resource = "UI/UIBag", cache = false });
        UIResources.Add(typeof(UIShop), new UIElement() { Resource = "UI/UIShop", cache = false });
        //可以继续添加其他UI类型
    }

    /// <summary>
    /// 打开UI界面
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Show<T>()
    {
        //获取UI类型
        Type type = typeof(T);
        //判断UI是否存在
        if (UIResources.ContainsKey(type))
        {
            //如果存在，则获取UI信息
            UIElement uiInfo = UIResources[type];
            if (uiInfo.instance != null)
            {
                //如果已经打开的，则直接启用
                uiInfo.instance.SetActive(true);
            }
            else
            {
                //如果没有打开的，则加载资源
                //从Resoueces中加载资源
                UnityEngine.Object prefab = Resources.Load(uiInfo.Resource);
                //实例出来Prefab UI
                uiInfo.instance = (GameObject)GameObject.Instantiate(prefab);
            }
            //将激活的UI返回出去
            return uiInfo.instance.GetComponent<T>();
        }
        return default(T);
    }

    /// <summary>
    /// 关闭UI界面
    /// </summary>
    /// <param name="type">UI信息字典的Key值，指的哪一个UI</param>
    public void Colse(Type type)
    {
        //检查UI是否存在
        if (UIResources.ContainsKey(type))
        {
            //获取UI信息
            UIElement uiInfo = UIResources[type];
            if (uiInfo.cache)
            {
                //如果UI实例存在，则禁用它
                uiInfo.instance.SetActive(false);
            }
            else
            {
                //如果UI实例不存在，则销毁它
                GameObject.Destroy(uiInfo.instance);
                //然后将实例置空
                uiInfo.instance = null;
            }
        }
    }
}
