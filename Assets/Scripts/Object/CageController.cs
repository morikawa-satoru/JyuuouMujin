using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Common;


public class CageController : _CharaBase
{

   	[SerializeField]
    LeverA_Controller lever;                                    // レバー
	[SerializeField]
    float leverWait = 0.5f;                                     // レバーを入れて柵があくまでの時間
	[SerializeField]
    float enterWait = 1.5f;                                     // オブジェ登場までの遅延時間

    //Animator    anim;
    bool        lastSw;


    void Start()
    {
        an = GetComponent<Animator>();
        lastSw = false;
        if(lever.sw)
        {
            an.Play("OpenWait");
		}
        else
        {
            an.Play("CloseWait");
		}
    }


    void Update()
    {
        // レバーを監視
        if(lever.sw && lastSw != lever.sw)
        {
            if(prefabName != null)
            {
                StartCoroutine(DelayEntry());
            }
		}
        if(!lever.sw && lastSw != lever.sw)
        {
            an.Play("Close");
		}

        lastSw = lever.sw;
    }


    // しばらくしたのちに登場
    IEnumerator DelayEntry()
    {
        // 少し待つ
        yield return new WaitForSeconds(leverWait);
        an.Play("Open");
        PlaySound(SE.ID.Object, SE.OBJ_OPEN_CAGE);

        Vector3 a = transform.position;
       //a.y += 0.5f;
    	// クローンをコピーする。バラバラになるオブジェクトは読み込みでは動作しないため
        //GameObject resource = (GameObject)Resources.Load(Define.folderCatchObj + prefabName);
        GameObject resource = GameObject.Find(prefabName + "_Clone");
        // 実体化
        GameObject gc = Instantiate(resource);
        gc.transform.position = new Vector3(1000, 1000, 0);      // いったん邪魔にならない場所へ出す

        // 少し待つ
        yield return new WaitForSeconds(enterWait);

        // 檻からから登場させる
        gc.transform.position = a;
        _CharaBase ch = gc.GetComponent<_CharaBase>();
        ch.StartFromCage();

        yield return null;
	}
}
