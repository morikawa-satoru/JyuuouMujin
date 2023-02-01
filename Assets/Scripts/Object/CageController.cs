using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Common;


public class CageController : _CharaBase
{

   	[SerializeField]
    LeverA_Controller lever;                                    // ���o�[
	[SerializeField]
    float leverWait = 0.5f;                                     // ���o�[�����č򂪂����܂ł̎���
	[SerializeField]
    float enterWait = 1.5f;                                     // �I�u�W�F�o��܂ł̒x������

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
        // ���o�[���Ď�
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


    // ���΂炭�����̂��ɓo��
    IEnumerator DelayEntry()
    {
        // �����҂�
        yield return new WaitForSeconds(leverWait);
        an.Play("Open");
        PlaySound(SE.ID.Object, SE.OBJ_OPEN_CAGE);

        Vector3 a = transform.position;
       //a.y += 0.5f;
    	// �N���[�����R�s�[����B�o���o���ɂȂ�I�u�W�F�N�g�͓ǂݍ��݂ł͓��삵�Ȃ�����
        //GameObject resource = (GameObject)Resources.Load(Define.folderCatchObj + prefabName);
        GameObject resource = GameObject.Find(prefabName + "_Clone");
        // ���̉�
        GameObject gc = Instantiate(resource);
        gc.transform.position = new Vector3(1000, 1000, 0);      // ��������ז��ɂȂ�Ȃ��ꏊ�֏o��

        // �����҂�
        yield return new WaitForSeconds(enterWait);

        // �B���炩��o�ꂳ����
        gc.transform.position = a;
        _CharaBase ch = gc.GetComponent<_CharaBase>();
        ch.StartFromCage();

        yield return null;
	}
}
