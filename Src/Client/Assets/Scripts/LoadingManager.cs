using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityScript.Scripting.Pipeline;

public class LoadingManager : MonoBehaviour
{

	public GameObject UITips;//提示图片
	public GameObject UILoadingPanel;//加载场景
	public GameObject UILoaginPanel;//登录场景
	public GameObject UIRegisterPanel;//注册场景

	public Slider loadingSlider;//加载进度条
	public Text loadingNumberText;//加载显示数字
	public Text loagingText;//加载显示文本

	private IEnumerator WaitLoading()
	{
        UITips.SetActive(true);
        yield return new WaitForSeconds(2f);
		//UILoadingPanel.SetActive(true);
		UITips.SetActive(false);

		yield return DataManager.Instance.LoadData();

		MapService.Instance.Init();
		UserService.Instance.Init();

        for (float i = 30; i <= 100;)
        {
			i += Random.Range(1f, 1.5f);
            loadingSlider.value = i;
            loadingNumberText.text = loadingSlider.value.ToString();
			yield return new WaitForEndOfFrame();
            if (loadingSlider.value >= 100)
            {
                UILoadingPanel.SetActive(false);
                UILoaginPanel.SetActive(true);
			}
        }

    }
	
	void Start ()
	{
		StartCoroutine(WaitLoading());
		loadingSlider.value = 0;
	}
	
	
	void Update ()
	{
		
	}
}
