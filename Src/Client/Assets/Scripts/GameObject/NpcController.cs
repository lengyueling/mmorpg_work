using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Data;
using Managers;
using System;
using Models;

public class NpcController : MonoBehaviour {

    public int NpcID;

    SkinnedMeshRenderer renderer;
    Animator anim;
    Color orignColor;

    /// <summary>
    /// 正在进行交互,防止重复点击
    /// </summary>
    private bool inInteractive = false;

    NpcDefine npc;
	void Start ()
    {
        renderer = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        anim = this.gameObject.GetComponentInChildren<Animator>();
        orignColor = this.renderer.sharedMaterial.color;
        npc = NpcManager.Instance.GetNpcDefine(this.NpcID);
        this.StartCoroutine(Actions());
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
