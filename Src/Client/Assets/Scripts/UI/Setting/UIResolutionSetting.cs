using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResolutionSetting : MonoBehaviour
{
    public Dropdown resolutionDropdown;

    private List<Vector2Int> windowSizes = new List<Vector2Int>
    {
        new Vector2Int(960, 540),
        new Vector2Int(1280, 720),
        new Vector2Int(1600, 900),
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440)
    };

    void Start()
    {
        List<string> options = new List<string>();
        int currentIndex = 0;

        for (int i = 0; i < windowSizes.Count; i++)
        {
            var size = windowSizes[i];
            string option = $"{size.x} x {size.y}";
            options.Add(option);

            if (Screen.width == size.x && Screen.height == size.y && !Screen.fullScreen)
                currentIndex = i;
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(OnResolutionSelected);
    }

    void OnResolutionSelected(int index)
    {
        Vector2Int selectedSize = windowSizes[index];
        Screen.SetResolution(selectedSize.x, selectedSize.y, false); // 窗口模式
        Debug.Log($"设置窗口分辨率为：{selectedSize.x} x {selectedSize.y}");
    }
}
