using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;


public class Door_EnemyComeOut : _CharaBase
{

    float Delay = 1;


    void Start()
    {
        an = GetComponent<Animator>();
        Delay = 1;
    }

    void Update()
    {
        Delay -= Time.deltaTime;
        if(Delay > 0){ return;}

        Delay = 10;

        StartCoroutine(DelayEntry());
    }


    // しばらくしたのちに登場
    IEnumerator DelayEntry()
    {
        // 少し待つ
        //yield return new WaitForSeconds(1);
        an.Play("Open");
        PlaySound(SE.ID.Object, SE.OBJ_OPEN_CAGE);

        Vector3 a = transform.position;
       //a.y += 0.5f;
    	// クローンをコピーする。バラバラになるオブジェクトは読み込みでは動作しないため
        //GameObject resource = (GameObject)Resources.Load(Define.folderCatchObj + "");
        GameObject resource = GameObject.Find(prefabName + "_Clone");
        // 実体化
        GameObject gc = Instantiate(resource);
        gc.transform.position = new Vector3(1000, 1000, 0);      // いったん邪魔にならない場所へ出す

        // 少し待つ
        yield return new WaitForSeconds(1);

        // 檻からから登場させる
        gc.transform.position = a;
        _CharaBase ch = gc.GetComponent<_CharaBase>();
        ch.StartFromCage();

        // 少し待つ
        yield return new WaitForSeconds(1);
        an.Play("Close");


        yield return null;
	}
}
