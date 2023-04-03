using UnityEngine;
using System.Collections.Generic;

public class ObjectManager : MonoBehaviour {

	[SerializeField]
	Transform obj_root;
	[SerializeField]
	public Sprite[] SPR_SHIP;
	[SerializeField]
	public Sprite[] TSURU;

	public ObjectCtrl PREFAB_OBJECT;    // キャラクタオブジェクトのprefab.

	public int LIMIT = 512;             // キャラクタ表示総数.

	public enum MODE    // 処理モード.
	{
		NOUSE = 0,          // 現在未使用.
		INIT,               // 初期化(出現直後).
		NOHIT,              // 当たり無視.
		HIT,                // 当たり有効・通常時はこちら.
		DEAD,				// 死亡アニメ.
		FINISH,             // 最終処理.
	}
	public enum TYPE	// 処理タイプ(当たり判定有無を大雑把に仕分け).
	{
		NOUSE = 0,			// 未使用.
		MYSHIP,				// 自機.
		MYSHOT,				// 自機ショット.
//		MYPOWERSHOT,		// 自機パワーショット(敵弾も消去).
		ENEMY1,				// 敵1(上から出てきて直進・斜めに進まない)
		ENEMY2,				// 敵2(上左右から出てきて直進・自機と座標が近くなったらX加算を反転して逃げようとする)
		ENEMY3,				// 敵3(上左右から出てきて直進・斜めにも進む)
		ENEMYBULLET,		// 敵弾.
		ENEMYBIGBULLET,		// 敵弾(大)
		NOHIT_EFFECT,		// エフェクト(当たり判定の存在しない・消えても問題のない表示群).
		NOHIT_SHIPDESTROY,	// 自機死亡エフェクト
	}

	public readonly Vector3 OBJECT_DISPLAY_LIMIT = new Vector3((272 * 3) / 2, (480 * 3) / 2, 0);
	public readonly Vector3 OBJECT_BULLET_LIMIT = new Vector3(272 / 2, 480 / 2, 0);

	public List<ObjectCtrl> objectStock = new List<ObjectCtrl>();
	public List<ObjectCtrl> objectUsed = new List<ObjectCtrl>();

	public Vector3 SHIP_POS = new Vector3(0, 0, 0);
	public bool boot = false;
	public bool destroy = false;	// 敵を全滅させたい時true
	public bool stop = false;       // 敵を止めたい時true
	public bool expert = false;     // エキスパートモード時true
	public int score = 0;			// スコア
	


	void Awake()
	{
		objectStock.Clear();
		objectUsed.Clear();
		for (int i = 0; i < LIMIT; i++)
		{
			ObjectCtrl obj;
			obj = Generate(TYPE.NOUSE, Vector3.zero, 0, 0);//, game);
			objectStock.Add(obj);
		}
		boot = true;
	}




	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	public ObjectCtrl Generate(TYPE type, Vector3 pos, int angle, int speed)//, MainGameLoop mloop)
	{
		ObjectCtrl obj = ObjectCtrl.Instantiate(PREFAB_OBJECT, pos, Quaternion.identity) as ObjectCtrl;
		obj.transform.SetParent(obj_root);
		obj.obj_type = TYPE.NOUSE;
		obj.obj_mode = MODE.NOUSE;
		obj.DisplayOff();
		return obj;
	}




	public ObjectCtrl Set(TYPE type, int mode,Vector3 pos, int angle, int speed)
	{
		if (objectStock.Count == 0)
		{
			return null;
		}
		ObjectCtrl obj;
		obj = objectStock[0];
		objectStock.RemoveAt(0);	// ここまで「ストックから取り出し」処理.

		obj.mode = mode;			// オブジェクト設定.
		obj.angle = angle;
		obj.speed = speed;
		obj.LIFE = 1;
		obj.transform.localRotation = Quaternion.identity;
		obj.transform.localScale = new Vector3(1, 1, 1);
		obj.obj_mode = MODE.INIT;
		obj.obj_type = type;
		obj.transform.localPosition = pos;
		obj.enabled = true;

		objectUsed.Add(obj);		// 使用中リストに入れる処理.

		return obj;
	}



	public int Return(ObjectCtrl obj)
	{
		obj.DisplayOff();
		objectUsed.Remove(obj);

		obj.GetComponent<SpriteRenderer>().sprite = null;
		obj.obj_mode = MODE.NOUSE;
		obj.obj_type = TYPE.NOUSE;
		obj.enabled = false;
		objectStock.Add(obj);

		return objectStock.Count;
	}



	public void ResetAll()
	{
		int objcnt = objectUsed.Count;
		while (objectUsed.Count > 0)
		{
			ObjectCtrl obj;
			obj = objectUsed[0];
			objectUsed.RemoveAt(0);
			obj.obj_mode = MODE.NOUSE;
			obj.obj_type = TYPE.NOUSE;
			obj.DisplayOff();
			obj.enabled = false;
			objectStock.Add(obj);
		}
	}
}

