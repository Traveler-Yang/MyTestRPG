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

    /// <summary>
    /// 打开UI界面
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Show<T>()
    {
        //检查UI是否已经打开
        Type type = typeof(T);
        if (UIResources.ContainsKey(type))
        {

        }
        return default(T);
    }

    /// <summary>
    /// 关闭UI界面
    /// </summary>
    public void Colse()
    {

    }
}
