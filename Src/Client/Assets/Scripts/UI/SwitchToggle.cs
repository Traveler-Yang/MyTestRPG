using DG.Tweening;
using Suntail;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchToggle : MonoBehaviour
{
    //把手RectTransform
    [SerializeField] private RectTransform handleRectTransform;

    //把手关闭颜色
    [SerializeField] private Color _handleOffColor;

    //开关背景关闭颜色
    [SerializeField] private Color _backgroundOffColor;

    //动画时间
    [SerializeField] private float _duration = 0.5f;

    //开关背景与把手的图片
    private Image _handleImage, _backgroundImage;

    //开关背景与把手的默认颜色
    private Color _handleColor, _backgroundColor;

    //把手移动的位置
    private Vector2 _handlePosition;

    private Toggle _toggle;


    private void Awake()
    {

        _toggle = GetComponent<Toggle>();
        _handleImage = handleRectTransform.GetComponent<Image>();
        _backgroundImage = handleRectTransform.parent.GetComponent<Image>();

        _handleColor = _handleImage.color;
        _backgroundColor = _backgroundImage.color;

        _handlePosition = handleRectTransform.anchoredPosition;

        _toggle.onValueChanged.AddListener(OnSwitch);

        //在游戏对象启动时检查 _toggle 组件的初始选中状态，依据选中状态来执行OnSwitch方法函数
        //if (_toggle.isOn)
        //{
        //    OnSwitch(true);
        //}
    }

    //控制开关动画方法函数
    public void OnSwitch(bool on)
    {
        handleRectTransform.DOAnchorPos(on ? _handlePosition : -_handlePosition, _duration).SetEase(Ease.InOutBack);

        _backgroundImage.DOColor(on ? _backgroundColor : _backgroundOffColor, _duration).SetEase(Ease.InOutBack);

        _handleImage.DOColor(on ? _handleColor : _handleOffColor, _duration).SetEase(Ease.InOutBack);
    }

    //避免持续监听 _toggle 的 onValueChanged 事件，防止在对象已销毁的情况下继续调用 OnSwitch 方法，避免潜在的错误或资源浪费
    private void OnDestroy()
    {
        _toggle.onValueChanged.RemoveListener(OnSwitch);
    }
}