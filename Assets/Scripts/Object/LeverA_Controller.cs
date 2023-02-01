using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using DG.Tweening;


public class LeverA_Controller : _Base
{

	[SerializeField]
    public bool sw = false;                                     // true でスイッチオン
	[SerializeField]
    float OperationPower = 15;                                  // 動作する力
	[SerializeField]
    bool oneTime;                                               // 一度だけ

    readonly float coolTime = 1f;                               // クールタイム

    GameObject Lever;
    float      waitTime;
    int        count;                                           // 開閉数(※ -1 : 無限)


    void Start()
    {
        Lever = transform.GetChild(0).gameObject;
        Initialize();
    }

    // 初期化
    void Initialize()
    {
        count = oneTime ? 1 : -1;                               // 開閉数

        // 最初のレバーの状態
        if(sw)
        {
            Lever.transform.rotation = Quaternion.Euler(0f, 0f, -45.0f);
		}
        else
        {
            Lever.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		}
        waitTime = 0;
	}


	void OnTriggerEnter2D(Collider2D col)
	{
        if(waitTime > 0){ return;}

		switch (col.gameObject.tag)
		{
			case Define.tagCatchObj:
                if(col.GetComponent<_CharaBase>().isAttack){ break;}  // 攻撃中か?
                changeSwitch(col.gameObject);
                break;

			case Define.tagConnectObj:
	    		if (!Global.plc[0].connectPlugObj.isAttack) { break;}       // 攻撃中か?
                changeSwitch(col.gameObject);
				break;
		}
	}

    // スイッチを変える
	void changeSwitch(GameObject go)
	{

        Rigidbody2D r = go.GetComponent<Rigidbody2D>();
        if(r.velocity.magnitude < OperationPower){ return; }

        if(count == 0){ return;}                                // 開閉数が 0 ならスイッチは変わらない

		PlaySound(SE.ID.Object, SE.OBJ_HIT_LEVER);
        count--;
        waitTime = coolTime;                                    // ウエイト
        if(sw)
        {
            sw = false;
            Lever.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		}
        else
        {
            sw = true;
            float z = 45f;
            if(r.velocity.x > 0){ z *= -1;}                     // コネクトオブジェの方向でレバーの倒れる方向を変える
            Lever.transform.rotation = Quaternion.Euler(0f, 0f, z);
		}
        //oneTime = true;
	}


	void Update()
    {
        if(waitTime > 0)
        {
            waitTime -= Time.deltaTime;
		}
    }
}
