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


    // ���΂炭�����̂��ɓo��
    IEnumerator DelayEntry()
    {
        // �����҂�
        //yield return new WaitForSeconds(1);
        an.Play("Open");
        PlaySound(SE.ID.Object, SE.OBJ_OPEN_CAGE);

        Vector3 a = transform.position;
       //a.y += 0.5f;
    	// �N���[�����R�s�[����B�o���o���ɂȂ�I�u�W�F�N�g�͓ǂݍ��݂ł͓��삵�Ȃ�����
        //GameObject resource = (GameObject)Resources.Load(Define.folderCatchObj + "");
        GameObject resource = GameObject.Find(prefabName + "_Clone");
        // ���̉�
        GameObject gc = Instantiate(resource);
        gc.transform.position = new Vector3(1000, 1000, 0);      // ��������ז��ɂȂ�Ȃ��ꏊ�֏o��

        // �����҂�
        yield return new WaitForSeconds(1);

        // �B���炩��o�ꂳ����
        gc.transform.position = a;
        _CharaBase ch = gc.GetComponent<_CharaBase>();
        ch.StartFromCage();

        // �����҂�
        yield return new WaitForSeconds(1);
        an.Play("Close");


        yield return null;
	}
}
