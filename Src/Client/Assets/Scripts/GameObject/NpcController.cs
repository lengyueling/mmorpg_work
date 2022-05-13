using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Data;
using Managers;
using System;
using Models;

public class NpcController : MonoBehaviour {

    public int npcID;

    new SkinnedMeshRenderer renderer;
    Animator anim;
    Color orignColor;

    /// <summary>
    /// 正在进行交互,防止重复点击
    /// </summary>
    private bool inInteractive = false;

    NpcDefine npc;

    NpcQuestStatus questStatus;
	void Start ()
    {
        renderer = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        anim = this.gameObject.GetComponentInChildren<Animator>();
        orignColor = this.renderer.sharedMaterial.color;
        npc = NpcManager.Instance.GetNpcDefine(this.npcID);
        NpcManager.Instance.UpdateNpcPosition(this.npcID, this.transform.position);
        this.StartCoroutine(Actions());
        RefreshNpcStatus();
        QuestManager.Instance.onQuestStatusChanged += OnQuestStatusChanged;
    }

    /// <summary>
    /// 当任务状态改变
    /// </summary>
    /// <param name="quest"></param>
    void OnQuestStatusChanged(Quest quest)
    {
        this.RefreshNpcStatus();
    }

    /// <summary>
    /// 刷新npc状态
    /// </summary>
    void RefreshNpcStatus()
    {
        questStatus = QuestManager.Instance.GetQuestStatusByNpc(this.npcID);
        UIWorldElementManager.Instance.AddNpcQuestStatus(this.transform, questStatus);
    }

    private void OnDestroy()
    {
        QuestManager.Instance.onQuestStatusChanged -= OnQuestStatusChanged;
        if (UIWorldElementManager.Instance != null)
        {
            UIWorldElementManager.Instance.RemoveNpcQuestStatus(this.transform);
        }
    }

    IEnumerator Actions()
    {
        while (true)
        {
            if (inInteractive)
            {
                yield return new WaitForSeconds(2f);
            }
            else
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 10f));
            }
            this.Relax();
        } 
    }

    private void Relax()
    {
        anim.SetTrigger("Relax");
    }

    void Interactive()
    {
        if (!inInteractive)
        {
            inInteractive = true;
            StartCoroutine(DoInteractive());
        }
    }

    IEnumerator DoInteractive()
    {
        yield return FaceToPlayer();
        if (NpcManager.Instance.Interactive(npc))
        {
            anim.SetTrigger("Talk");
        }
        yield return new WaitForSeconds(3f);
        inInteractive = false;
    }

    IEnumerator FaceToPlayer()
    {
        Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
        while (Mathf.Abs(Vector3.Angle(this.gameObject.transform.forward, faceTo)) > 5)
        {
            this.gameObject.transform.forward = Vector3.Lerp(this.gameObject.transform.forward, faceTo, Time.deltaTime * 5f);
            yield return null;
        }
    }
    void OnMouseDown()
    {
        if (Vector3.Distance(this.transform.position, User.Instance.CurrentCharacterObject.transform.position) > 2f)
        {
            User.Instance.CurrentCharacterObject.StartNav(this.transform.position);
        }
        this.Interactive();
    }

    void OnMouseOver()
    {
        Highlight(true);
    }

    void OnMouseEnter()
    {
        Highlight(true);
    }

    void OnMouseExit()
    {
        Highlight(false);
    }

    void Highlight(bool hightlight)
    {
        if (hightlight == true)
        {
            if (renderer.sharedMaterial.color != Color.white)
            {
                renderer.sharedMaterial.color = Color.white;
            }
        }
        else
        {
            if (renderer.sharedMaterial.color != orignColor)
            {
                renderer.sharedMaterial.color = orignColor;
            }
        }

    }
}
