using Common.Data;
using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    public int npcId;//NPC ID

    SkinnedMeshRenderer renderer;//NPC 的渲染器
    Animator animator;//NPC 的动画控制器
    //Color originalColor; // NPC 的原始颜色

    private bool inInteraction = false; // 是否正在交互中

    NpcDefine npc;//NPC 对象

    void Start()
    {
        renderer = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();//获取NPC的渲染器
        animator = GetComponent<Animator>();
        //originalColor = renderer.sharedMaterial.color; // 保存NPC的原始颜色
        npc = NPCManager.Instance.GetNpcDefine(npcId);
        //this.StartCoroutine(Actions());
    }

    IEnumerator Actions()
    {
        while (true)
        {
            //如果NPC正在交互中，则等待一段时间后再继续
            if (inInteraction)
                yield return new WaitForSeconds(2f);
            //如果NPC没有在交互中，则执行随机动作
            else
                //随机等待5~10秒执行一个动作
                yield return new WaitForSeconds(Random.Range(5f, 10f));

            this.Relax();
        }
    }

    void Update()
    {

    }

    void Relax()
    {
        animator.SetTrigger("Relax");
    }

    /// <summary>
    /// NPC交互方法
    /// </summary>
    void Interactive()
    {
        // 判断是否在交互中
        // 如果正在交互中，则不再执行交互逻辑
        if (!inInteraction)
        {
            inInteraction = true; // 设置为正在交互状态
            StartCoroutine(DoInteractive());
        }
    }

    /// <summary>
    /// 做交互处理
    /// </summary>
    /// <returns></returns>
    IEnumerator DoInteractive()
    {
        //开始交互时，先让NPC面向玩家
        yield return FaceToPlayer();
        //面向玩家后，开始交互
        //判断是哪种类型的NPC
        //返回结果是否成功
        //成功则执行交互动画
        if (NPCManager.Instance.Interactive(npc))
        {
            animator.SetTrigger("Talk");
        }
        //3秒内不允许再次交互
        yield return new WaitForSeconds(3f);
        inInteraction = false; // 交互结束，重置状态
    }

    /// <summary>
    /// 面向玩家
    /// </summary>
    /// <returns></returns>
    IEnumerator FaceToPlayer()
    {
        //目标的位置 减去 NPC的位置 得到一个方向向量
        Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
        while (Mathf.Abs(Vector3.Angle(this.gameObject.transform.forward, faceTo)) > 5)
        {
            this.gameObject.transform.forward = Vector3.Lerp(this.gameObject.transform.forward, faceTo, Time.deltaTime * 5f);
            yield return null;
        }
    }

    private void OnMouseDown()
    {
        //鼠标点击NPC时，开始交互
        Interactive();
    }

    private void OnMouseOver()
    {
        Highlight(true);
    }

    private void OnMouseEnter()
    {
        Highlight(true);
    }

    private void OnMouseExit()
    {
        Highlight(false);
    }

    /// <summary>
    /// 高亮NPC方法
    /// </summary>
    /// <param name="highlight"></param>
    void Highlight(bool highlight)
    {
        //if (highlight)
        //{
        //    if (renderer.sharedMaterial.color != Color.white)
        //        renderer.sharedMaterial.color = Color.white;
        //}
        //else
        //{
        //    if(renderer.sharedMaterial.color != originalColor)
        //        renderer.sharedMaterial.color = originalColor; // 恢复原始颜色
        //}
    }
}
