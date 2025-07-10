using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TabView : MonoBehaviour
{
    public TabButton[] tabButtons;//按钮数
    public GameObject[] tabPages;//背包页

    public int index = -1;//当前的背包页索引


    public UnityAction<int> OnTabSelect;

    IEnumerator Start()
    {
        //开始时遍历所有的按钮
        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].tabView = this;//将每个按钮的所有者赋值给自己
            tabButtons[i].tabIndex = i;//给按钮的索引赋值
        }
        yield return new WaitForEndOfFrame();//等待1帧
        SelectTab(0);//默认选择第一页，也就是0
    }

    /// <summary>
    /// 选择哪一页背包
    /// </summary>
    /// <param name="index">背包索引</param>
    public void SelectTab(int index)
    {
        //如果当前显示的背包索引与要替换的背包不一致
        //则进行更改
        if (this.index != index)
        {
            for (int i = 0; i < tabButtons.Length; i++)
            {
                tabButtons[i].Select(i == index);
                if (this.tabPages.Length > 0)
                    tabPages[i].SetActive(i == index);
            }

            this.index = index;

            // 触发 Tab 切换事件，让 UI 刷新
            if (OnTabSelect != null)
            {
                OnTabSelect(index);
            }
        }
    }
}
