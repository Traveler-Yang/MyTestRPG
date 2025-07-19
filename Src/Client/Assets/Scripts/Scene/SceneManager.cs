using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneManager : MonoSingleton<SceneManager>
{
    public GameObject loadingUIPrefab;

    private LoadingUIController loadingUI;

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    /// <summary>
    /// 协程加载场景
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    IEnumerator LoadSceneAsync(string sceneName)
    {
        GameObject ui = Instantiate(loadingUIPrefab);
        loadingUI = ui.GetComponent<LoadingUIController>();

        DontDestroyOnLoad(ui); // 切场景时不销毁

        //异步加载场景
        AsyncOperation op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        //设置加载场景时不自动切换
        op.allowSceneActivation = false;

        float fakeProgress = 0f;
        //当前加载未完成
        while (!op.isDone)
        {
            //获取操作进度
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            if (progress > fakeProgress)
            {
                fakeProgress += Time.deltaTime; // 每帧平滑增加
            }
            loadingUI.SetProgress(fakeProgress);
            if (op.progress >= 0.9f)
            {
                //加载完成后
                //激活场景切换
                op.allowSceneActivation = true;
                //按任意键继续
                //loadingUI.ShowComplete();
                //if (Input.anyKeyDown)
                //{
                //    op.allowSceneActivation = true;
                //}
            }

            yield return null;
        }

        Destroy(ui); // 场景加载完成后清除 LoadingUI
    }
}
