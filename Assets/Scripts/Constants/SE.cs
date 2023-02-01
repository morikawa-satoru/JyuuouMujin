/// <summary>
///	オーディオ名を定数で管理するクラス
///	
///	出所	
///	https://soundeffect-lab.info/sound/various/various3.html
///	https://on-jin.com/
///	
///	
///	</summary>
public static class	SE
{
	public enum	ID
	{
		System = 0,	Player = 0,	Enemy, Object, Object2, ConnectPlug 
	};

	public const string	PLY_JUMP		 = "ply_jump";
	public const string	PLY_LANDING		 = "ply_landing";
	public const string	PLY_WALK		 = "ply_walk";
	public const string	PLY_DAMAGE		 = "ply_damage";
	public const string	PLY_EXIT		 = "ply_exit";
	public const string	PLY_CONNECT		 = "ply_connect";
	public const string	PLY_DISCONNECT	 = "ply_disconnect";
	public const string	PLY_THROW		 = "ply_throw";
	public const string	PLY_PULL		 = "ply_pull";
	public const string	PLY_WINDOW_CLOSE = "ply_window_close";

	public const string	ENE_SHOT_KINOKO	 = "ene_shot_kinoko";

	public const string	FRI_ATTACK = "fri_attack";

	public const string	OBJ_HIT			 = "obj_hit";
	public const string	OBJ_HIT_GROUND	 = "obj_hit_ground";
	public const string	OBJ_HIT_SWITCH	 = "obj_hit_switch";
	public const string	OBJ_HIT_LEVER	 = "obj_hit_lever";
	public const string	OBJ_OPEN_CAGE	 = "obj_open_cage";

	public const string	OVER	= "_over";
	public const string	START	= "_start";
	public const string	SELECT	= "_select";
	public const string	OK		= "_ok";
	public const string	NG		= "_ng";
	public const string	EVENT	= "_event";
	public const string	HIT_KEY	= "_hit_key";

	public const string	DOOR_OPEN  = "door_open";
	public const string	DOOR_CLOSE = "door_close";
}
