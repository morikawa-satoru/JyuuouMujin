using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common;


//https://kan-kikuchi.hatenablog.com/entry/AudioManager


// SingletonMonoBehaviour.cs もコピーのこと



// BGMとSEの管理をするマネージャ。シングルトン。
public class AudioManager :	SingletonMonoBehaviour<AudioManager>
{
	// ボリューム保存用のkeyとデフォルト値
	private	const string BGM_VOLUME_KEY	  =	"BGM_VOLUME_KEY";
	private	const string SE_VOLUME_KEY	  =	"SE_VOLUME_KEY";
	private	const float	BGM_VOLUME_DEFULT =	1.0f;
	private	const float	SE_VOLUME_DEFULT  =	1.0f;

	// BGMがフェードするのにかかる時間
	public const float BGM_FADE_SPEED_RATE_HIGH	= 0.9f;
	public const float BGM_FADE_SPEED_RATE_LOW	= 0.3f;
	private	float _bgmFadeSpeedRate	= BGM_FADE_SPEED_RATE_HIGH;

	// 次流すBGM名、SE名
	private	string _nextBGMName;
	private	string _nextSEName;

	// BGMをフェードアウト中か
	private	bool _isFadeOut	= false;

	// BGM用、SE用に分けてオーディオソースを持つ
	public AudioSource AttachBGMSource;
	public AudioSource[] AttachSESource;

	// 全Audioを保持
	private	Dictionary<string, AudioClip> _bgmDic;
	private	Dictionary<string, AudioClip> _seDic;

	// 初期化
	private	void Awake()
	{
		if (this !=	Instance)
		{
			Destroy(this);
			return;
		}
		DontDestroyOnLoad(this.gameObject);

		// リソースフォルダから全SE&BGMのファイルを読み込みセット
		_bgmDic	= new Dictionary<string, AudioClip>();
		_seDic = new Dictionary<string,	AudioClip>();

		object[] bgmList = Resources.LoadAll("Audio/BGM");
		object[] seList	= Resources.LoadAll("Audio/SE");

		foreach	(AudioClip bgm in bgmList)
		{
			_bgmDic[bgm.name] =	bgm;
		}
		foreach	(AudioClip se in seList)
		{
			_seDic[se.name]	= se;
		}
	}

	void Start()
	{
		AttachBGMSource.volume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, BGM_VOLUME_DEFULT);
		for	(int i = 0;	i <	AttachSESource.Length; i++)
		{
			AttachSESource[i].volume = PlayerPrefs.GetFloat(SE_VOLUME_KEY, SE_VOLUME_DEFULT);
		}
	}

	// SE

	// 指定したファイル名のSEを流す。第二引数のdelayに指定した時間だけ再生までの間隔を空ける
	public void	PlaySE(SE.ID num, string seName, float delay = 0.0f)
	{
		if (!_seDic.ContainsKey(seName))
		{
			Debug.Log(seName + "という名前のSEがありません");
			return;
		}

		_nextSEName	= seName;
		if (delay >	0)
		{
			//Invoke("DelayPlaySE",	delay);
			StartCoroutine(DelayPlaySE((int)num, delay));
		}
		else
		{
			AttachSESource[(int)num].clip =	_seDic[_nextSEName]	as AudioClip;
			AttachSESource[(int)num].Play();
		}
	}

	public bool	CheckSE(SE.ID num)
	{
		return AttachSESource[(int)num].isPlaying;
	}

	IEnumerator	DelayPlaySE(int	num, float delay)
	{
		yield return new WaitForSeconds(delay);

		// PlayOneShot は複数の音が重なるので、Playに変更
		//AttachSESource.PlayOneShot(_seDic[_nextSEName] as	AudioClip);
		AttachSESource[num].clip = _seDic[_nextSEName] as AudioClip;
		AttachSESource[num].Play();
	}

	// BGM

	// 指定したファイル名のBGMを流す。ただし既に流れている場合は前の曲をフェードアウトさせてから。
	// 第二引数のfadeSpeedRateに指定した割合でフェードアウトするスピードが変わる
	public void	PlayBGM(string bgmName,	float fadeSpeedRate	= BGM_FADE_SPEED_RATE_HIGH)
	{
		if (!_bgmDic.ContainsKey(bgmName))
		{
			Debug.Log(bgmName +	"という名前のBGMがありません");
			return;
		}

		// 現在BGMが流れていない時はそのまま流す
		if (!AttachBGMSource.isPlaying)
		{
			_nextBGMName = "";
			AttachBGMSource.clip = _bgmDic[bgmName]	as AudioClip;
			AttachBGMSource.Play();
		}
		// 違うBGMが流れている時は、流れているBGMをフェードアウトさせてから次を流す。同じBGMが流れている時はスルー
		else if	(AttachBGMSource.clip.name != bgmName)
		{
			_nextBGMName = bgmName;
			FadeOutBGM(fadeSpeedRate);
		}
	}

	// 現在流れている曲をフェードアウトさせる
	// fadeSpeedRateに指定した割合でフェードアウトするスピードが変わる
	public void	FadeOutBGM(float fadeSpeedRate = BGM_FADE_SPEED_RATE_LOW)
	{
		_bgmFadeSpeedRate =	fadeSpeedRate;
		_isFadeOut = true;
	}
	public void	StopBGM()
	{
		AttachBGMSource.Stop();
		AttachBGMSource.volume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, BGM_VOLUME_DEFULT);
		_isFadeOut = false;
		if (!string.IsNullOrEmpty(_nextBGMName))
		{
			PlayBGM(_nextBGMName);
		}
	}

	void Update()
	{
		if (!_isFadeOut){ return; }

		// 徐々にボリュームを下げていき、ボリュームが0になったらボリュームを戻し次の曲を流す
		AttachBGMSource.volume -= Time.deltaTime * _bgmFadeSpeedRate;
		if (AttachBGMSource.volume <= 0)
		{
			AttachBGMSource.Stop();
			AttachBGMSource.volume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, BGM_VOLUME_DEFULT);
			_isFadeOut = false;
			if (!string.IsNullOrEmpty(_nextBGMName))
			{
				PlayBGM(_nextBGMName);
			}
		}
	}

	// BGMとSEのボリュームを別々に変更&保存
	public void	ChangeVolume(float BGMVolume, float	SEVolume)
	{
		AttachBGMSource.volume = BGMVolume;
		for	(int i = 0;	i <	AttachSESource.Length; i++)
		{
			AttachSESource[i].volume = SEVolume;
		}
		PlayerPrefs.SetFloat(BGM_VOLUME_KEY, BGMVolume);
		PlayerPrefs.SetFloat(SE_VOLUME_KEY,	SEVolume);
	}
}


