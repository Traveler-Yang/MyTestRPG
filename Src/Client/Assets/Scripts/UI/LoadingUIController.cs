using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingUIController : MonoBehaviour
{
    public Slider progressSlider;
    public TextMeshProUGUI progressText;

    /// <summary>
    /// 设置Loading条进度显示
    /// </summary>
    /// <param name="progress"></param>
    public void SetProgress(float progress)
    {
        progressSlider.value = progress;
        //progressText.text = Mathf.RoundToInt(progress * 100) + "%";
        progressText.text = string.Format("加载中... {0}%", Mathf.RoundToInt(progress * 100));
    }

    //public void ShowComplete()
    //{
    //    progressText.text = "加载完成，按任意键继续...";
    //}
}
