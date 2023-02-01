using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Common;


//
// �J�����R���g���[���[
//	��ʐ؂�ւ��̈���o�Ȃ��悤�Ɋg��E�k������
//
public class CameraController : MonoBehaviour
{

	readonly float zoomSpeed  = 1;								// �g��E�k�����x
	readonly float moveTime   = 0.5f;							// �g��E�k������
	readonly float cameraSize = 5.343f;
	readonly float xRoomSize = Define.xRoomSize / 2;			// �����̉��T�C�Y
	readonly float yRoomSize = (Define.yRoomSize - 0.3f) / 2f;	// �����̏c�T�C�Y(�c���������������߂ɂ���)

	bool	whileZoomInOut = false;								// �Y�[�����t���O


	void Update()
	{
		checkScaleKey();
		scaleControll();
	}

	// �X�P�[���R���g���[��
	void scaleControll()
	{
		if (Global.isMapScroll){ return; }

		// �v���C���[���W���J�������W��ݒ�(����ʒ[���v�Z����)
		Global.camera.transform.position = GetCameraPos(new Vector3(Global.plc[0].transform.position.x, Global.plc[0].transform.position.y, Global.camera.transform.position.z));
	}

	// �L�[�{�[�h�ɂ��g��E�k��
	void checkScaleKey()
	{
		float isZoomInOut = 0;
		if(!whileZoomInOut)
		{
			if (	 Global.scale > 1 && Input.GetKeyDown(KeyCode.A)){ isZoomInOut = -zoomSpeed;}
			else if (Global.scale < 4 && Input.GetKeyDown(KeyCode.S)){ isZoomInOut =  zoomSpeed;}
			if (isZoomInOut != 0)
			{
				Global.isFocus = true;
				whileZoomInOut = true;
				DOTween.To(
					() =>  Global.scale,
					(x) => Global.scale = x,					// �����_���� Global.scale ��ς���
					Global.scale + isZoomInOut,
					moveTime)
				.OnUpdate(() =>
				{
					Global.camera.orthographicSize   = cameraSize / Global.scale;
					Global.cameraUI.orthographicSize = Global.camera.orthographicSize;
					Global.plc[0].message.SetScale(Global.scale);
				})
				.OnComplete( () =>
				{
					whileZoomInOut = false;
				});
			}
		}
	}

	// pos(�J�������W) �����ƂɃJ�������W�̎l�����v�Z���ׂ̉�ʂ������Ȃ��悤�ɒl�����H�����J�������W��Ԃ��Bscale �� 1�`4(�g��)
	Vector3 GetCameraPos(Vector3 pos)
	{
		// Step1 : �J�������W�o�b�N�A�b�v
		Vector3 bak = Global.camera.transform.position;

		// Step2 : �w����W���J�������W�Ƃ���
		Global.camera.transform.position = pos;

		// Step3 : �J�����̍���ƉE�������߂�
		Vector2 cam_rt = Global.camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
		Vector2 cam_lb = Global.camera.ScreenToWorldPoint(Vector3.zero);

		// Step4 : �`��̈���v�Z
		Vector3 retValue = pos;
		float mapx = Global.mapArea.x * Define.xRoomSize;
		float mapy = Global.mapArea.y * Define.yRoomSize;
		float remainderX0 = cam_lb.x - mapx;
		float remainderX1 = cam_rt.x - mapx;
		float remainderY0 = cam_rt.y + mapy;
		float remainderY1 = cam_lb.y + mapy;

		// �w����W�͍��[��?
		if (remainderX0 <= -Define.xRoomSize / 2)
		{
			retValue.x -= cam_lb.x + (-mapx + xRoomSize);
		}
		// �w����W�͉E�[��?
		else if (remainderX1 >= Define.xRoomSize / 2)
		{
			retValue.x -= cam_rt.x - (mapx + xRoomSize);
		}
		// �w����W�͏�[��?
		if (remainderY0 >= yRoomSize)
		{
			retValue.y -= cam_rt.y + (mapy - yRoomSize);
		}
		// �w����W�͉��[��?
		else if (remainderY1 <= -yRoomSize)
		{
			retValue.y -= cam_lb.y + (mapy + yRoomSize);
		}

		// Step5 : �J���������Ƃɖ߂�
		Global.camera.transform.position = bak;

		// �߂�l�͐V���ȃJ�����̒��S���W
		return retValue;
	}

	// �J�����ʒu�ړ�
	public void SetupCamera(Vector2 afterPos)
	{
		Vector3 retValue = GetCameraPos(new Vector3(afterPos.x, afterPos.y, Global.camera.transform.position.z));
		transform.position = retValue;
	}

	// �}�b�v�X�N���[���J�n
	public void StartCameraMove(Vector2 afterPos)
	{
		Global.isMapScroll = true;
		Global.isPause = true;								// �S�̓I�Ɉꎞ��~

		Vector3 retValue = GetCameraPos(new Vector3(afterPos.x, afterPos.y, Global.camera.transform.position.z));
		Global.camera.transform.DOLocalMove(retValue, Define.scrollTime)
			.OnComplete(() =>
			{
				Global.isMapScroll = false;
				Global.isPause = false;						// �S�̈ꎞ��~�I�t
			});
	}

	// �U�����J�n
	public void	Shake( float duration, float magnitude )
	{
		StartCoroutine(	DoShake( duration, magnitude ) );
	}

	// �U���͔񓯊��ōs��
	IEnumerator	DoShake( float duration, float magnitude )
	{
		var	pos	= transform.localPosition;

		var	elapsed	= 0f;

		while (	elapsed	< duration )
		{
			var	x =	pos.x +	Random.Range( -1f, 1f )	* magnitude;
			var	y =	pos.y +	Random.Range( -1f, 1f )	* magnitude;

			transform.localPosition	= new Vector3( x, y, pos.z );

			elapsed	+= Time.deltaTime;

			yield return null;
		}

		transform.localPosition	= pos;
	}
}



