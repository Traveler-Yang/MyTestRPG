using Managers;
using Services;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingManager : MonoBehaviour
{

	public GameObject UITips;//提示图片
	public GameObject UILoadingPanel;//加载场景
	public GameObject UILoaginPanel;//登录场景
	public GameObject UIRegisterPanel;//注册场景

	public Slider loadingSlider;//加载进度条
	public TextMeshProUGUI loadingNumberText;//加载显示数字
	public Text loagingText;//加载显示文本

	private IEnumerator WaitLoading()
	{
		log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
        UITips.SetActive(true);
        yield return new WaitForSeconds(2f);
		//UILoadingPanel.SetActive(true);
		UITips.SetActive(false);

		yield return DataManager.Instance.LoadData();

		MapService.Instance.Init();
		UserService.Instance.Init();
		ShopManager.Instance.Init();
		FriendService.Instance.Init();
		TempService.Instance.Init();
		GuildService.Instance.Init();
		ChatService.Instance.Init();
		SoundManager.Instance.PlayMusic(SoundDefine.Music_Login);
        for (float i = 0.01f; i <= 1;)
        {
			i += Random.Range(0.01f, 0.03f);
            loadingSlider.value = i;
			loadingNumberText.text = string.Format("{0} %", (int)(loadingSlider.value * 100));
			yield return new WaitForEndOfFrame();
            if (loadingSlider.value >= 1)
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
