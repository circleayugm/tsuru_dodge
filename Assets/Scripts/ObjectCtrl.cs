//#define MUTEKI
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectCtrl : MonoBehaviour
{

	[Space]
	[SerializeField]
	SpriteRenderer MainPic; // メイン画像.
	[SerializeField]
	Transform MainPos;      // 座標・回転関連.
	[SerializeField]
	CircleCollider2D MainHit;  // 当たり判定.

	[Space]

	public int LIFE = 0;        // 耐久力.
	public bool NOHIT = false;  // 当たり判定の有無.
	[Space]
	public float speed = 0.0f;  // 移動速度.
	public int angle = 0;       // 移動角度(360度を256段階で指定).
	public int type = 0;		// キャラクタタイプ(同じキャラクタだけど動きが違うなどの振り分け).
	public int mode = 0;        // 動作モード(キャラクタによって意味が違う).
	public int power = 0;       // 相手に与えるダメージ量.
	public int count = 0;       // 動作カウンタ.
	public Vector3 vect = Vector3.zero;


	public int ship_energy_charge = 0;



	[Space]
	public Sprite[] SPR_SHIP; // 暫定；キャラクタ画像置き場・将来的にはキャラクタ設定時にリストに登録するようにする.
	public Sprite[] SPR_MYSHOT;
//	public Sprite[] SPR_CHARGE;
//	public Sprite[] SPR_MYPOWERSHOT;
	public Sprite[] SPR_ENEMY;
	public Sprite[] SPR_BULLET;
	public Sprite[] SPR_CRUSH;
	public Sprite[] SPR_GENERATE;

	readonly Color COLOR_NORMAL = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	readonly Color COLOR_DAMAGE = new Color(1.0f, 0.0f, 0.0f, 1.0f);

	readonly int[] SCORE_TABLE = new int[8] { 470, 260, 280, 580, 930, 670, 490, 530 };

	const float SHIP_MOVE_MIN_X = -2.4f;
	const float SHIP_MOVE_MAX_X = 2.4f;
	const float SHIP_MOVE_MIN_Y = -4.7f;
	const float SHIP_MOVE_MAX_Y = 4.5f;

	const float MYSHOT_DISP_MAX_Y = 5.5f;

	const float ENEMY_OFFSCREEN_MIN_X = -3.0f;
	const float ENEMY_OFFSCREEN_MAX_X = 3.0f;
	const float ENEMY_OFFSCREEN_MIN_Y = -5.5f;
	const float ENEMY_OFFSCREEN_MAX_Y = 5.5f;

	const float BULLET_OFFSCREEN_MIN_X = -3.0f;
	const float BULLET_OFFSCREEN_MAX_X = 3.0f;
	const float BULLET_OFFSCREEN_MIN_Y = -5.5f;
	const float BULLET_OFFSCREEN_MAX_Y = 5.5f;

	readonly Vector2 HITSIZE_MYSHIP = new Vector2(0.08f, 0.08f);
	readonly Vector2 HITSIZE_MYSHOT = new Vector2(0.48f, 0.84f);
	readonly Vector2 HITSIZE_ENEMY = new Vector2(0.24f, 0.24f);
	readonly Vector2 HITSIZE_ENEMYBULLET = new Vector2(0.08f, 0.08f);
	readonly Vector2 HITSIZE_ENEMYBIGBULLET = new Vector2(0.16f, 0.16f);

	public Color color = new Color();                       // キャラクタ表示色(ホワイトアウトシェーダ採用のため0.5fが基準).
	public List<Sprite> PictureList = new List<Sprite>();   // キャラクタのスプライトリスト.

	public ObjectManager.MODE obj_mode = ObjectManager.MODE.NOUSE;  // キャラクタの管理状態.
	public ObjectManager.TYPE obj_type = ObjectManager.TYPE.NOUSE;  // キャラクタの分類(当たり判定時に必要).

	public bool destroy = false;	// 全滅時true(タイムアップで敵全滅)
	public bool stop = false;       // 停止時true(自機やられで敵移動停止)

	float[] param = new float[4];

	MainSceneCtrl MAIN;
	TouchPanelCtrl TOUCH;
	ObjectManager MANAGE;
//	PadControlManager pad;


	void Awake()
	{
		MAIN = GameObject.Find("root_main").GetComponent<MainSceneCtrl>();
		TOUCH = GameObject.Find("root_main").GetComponent<TouchPanelCtrl>();
		MANAGE = GameObject.Find("root_main").GetComponent<ObjectManager>();
//		pad = GameObject.Find("gameroot").GetComponent<PadControlManager>();
		MainHit.enabled = false;
	}


	// Use this for initialization
	void Start()
	{

	}
	// Update is called once per frame
	void Update()
	{
		Vector3 pos = Vector3.zero;
/*
		if (game.mode==MainGameLoop.MODE.WAIT) {
			return;
		}
*/
		if (obj_mode == ObjectManager.MODE.NOUSE)
		{
			return;
		}
		switch (obj_mode)
		{
			case ObjectManager.MODE.NOUSE:
				break;
			case ObjectManager.MODE.INIT:
				MainPic.enabled = true;
				if (obj_type == ObjectManager.TYPE.NOHIT_EFFECT)
				{
					obj_mode = ObjectManager.MODE.NOHIT;
					MainHit.enabled = false;
				}
				else {
					obj_mode = ObjectManager.MODE.HIT;
					MainHit.enabled = true;
				}
				count = 0;
				break;
			case ObjectManager.MODE.HIT:
				MainHit.enabled = true;
				break;
			case ObjectManager.MODE.NOHIT:
				MainHit.enabled = false;
				break;
			case ObjectManager.MODE.FINISH:
//				game.ReturnObject(this);
				break;
		}
		switch (obj_type)
		{
			case ObjectManager.TYPE.MYSHIP:
				if (count == 0)
				{
					MainHit.enabled = true;
					NOHIT = false;
					MainHit.radius = 0.1f;
					MainPic.color = COLOR_NORMAL;
					MainPic.sortingOrder = 5;
					MainPic.sprite = MANAGE.SPR_SHIP[0];
				}
				if (MAIN.mode != MainSceneCtrl.MODE.OVER)
				{
					MainPic.sprite = MANAGE.SPR_SHIP[0];
					obj_mode = ObjectManager.MODE.HIT;

					if (Input.GetKey(KeyCode.W) == true)
					{
						TOUCH.pos.y += 0.2f;
						if (TOUCH.pos.y>4.2f)
						{
							TOUCH.pos.y = 4.2f;
						}
					}
					if (Input.GetKey(KeyCode.S) == true)
					{
						TOUCH.pos.y -= 0.2f;
						if (TOUCH.pos.y < -4.2f)
						{
							TOUCH.pos.y = -4.2f;
						}
					}
					if (Input.GetKey(KeyCode.D) == true)
					{
						TOUCH.pos.x += 0.2f;
						if (TOUCH.pos.x > 5.2f)
						{
							TOUCH.pos.x = 5.2f;
						}
					}
					if (Input.GetKey(KeyCode.A) == true)
					{
						TOUCH.pos.x -= 0.2f;
						if (TOUCH.pos.x < -5.2f)
						{
							TOUCH.pos.x = -5.2f;
						}
					}
				}
				else if (MAIN.mode== MainSceneCtrl.MODE.OVER)
				{
					MainPic.sprite = MANAGE.SPR_SHIP[(count >> 2) % 2];
				}
				this.transform.localPosition = TOUCH.pos;
				MANAGE.SHIP_POS = this.transform.localPosition;
				break;




			case ObjectManager.TYPE.ENEMY1:
				{

					if (count == 0)
					{
						MainHit.enabled = true;
						NOHIT = false;
						LIFE = 2;
						power = 1;
						MainHit.radius = 2f;
						MainPic.sortingOrder = 2;
						MainPic.color = COLOR_NORMAL;
						MainPic.sprite = MANAGE.TSURU[0];
						this.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
						param[0] = Random.Range(-5f, 5f);

					}

					this.transform.localPosition += AngleToVector3(angle, speed)/40f;
					this.transform.localEulerAngles += new Vector3(0, 0, param[0]);
					this.transform.localScale += new Vector3(0.001f, 0.001f, 0);
				}
				if (Mathf.Abs(this.transform.localPosition.x)>9f)
				{
					MANAGE.Return(this);
				}
				if (Mathf.Abs(this.transform.localPosition.y)>6f)
				{
					MANAGE.Return(this);
				}
				break;








			case ObjectManager.TYPE.NOHIT_EFFECT:
				MainPic.color = COLOR_NORMAL;
				MainHit.enabled = false;
				if (MANAGE.stop == false)
				{
					switch (mode)
					{
						case 0:
							MainPic.sprite = MANAGE.TSURU[2];
							if (count == 16)
							{
								MANAGE.Return(this);
							}
							break;
						case 1:
							MainPic.sprite = MANAGE.TSURU[2];
							vect = SetVector(new Vector3(0, 0, 0), speed, angle, false);
							this.transform.localPosition += vect;
							if (count == 16)
							{
								MANAGE.Return(this);
							}
							break;
						case 2:
							MainPic.sprite = MANAGE.TSURU[2];
							vect.x = (float)Random.Range(-(count), (count)) / 10.0f;
							vect.y = (float)Random.Range(-(count), (count)) / 10.0f;
							this.transform.localPosition += vect;
							MANAGE.Set(ObjectManager.TYPE.NOHIT_EFFECT, 1, this.transform.localPosition, Random.Range(0, 360), Random.Range(1, 5));
							if (count == 16)
							{
								MANAGE.Return(this);
							}
							break;
					}
				}
				break;
		}



		if (LIFE <= 0)
		{
			Dead();
		}



		count++;
	}
	//-------------------------------------------------------------------
	//	public void SetVector(Vector3 pos, float spd, float rad_offset)
	//		敵弾に位置を基準とした移動量を設定
	//	Vector3 pos=基準座標
	//	float spd=移動速度
	//	float rad_offset=追加角度(扇状弾などに使う)
	//-------------------------------------------------------------------
	Vector3 SetVector(Vector3 pos, float spd, float rad_offset,bool rotation)
	{
		Vector3 vec = Vector3.zero;
		if (spd == 0.0f)    //必ず少しは動くようにする.
		{
			spd = 1f;
		}
		spd = spd / 100.0f;
		float rad = Mathf.Atan2(
								pos.y - this.transform.position.y,
								pos.x - this.transform.position.x
								);              //座標から角度を求める.
		rad += rad_offset;                      //角度にオフセット値追加.
		vec.x = spd * Mathf.Cos(rad);           //角度からベクトル求める.
		vec.y = spd * Mathf.Sin(rad);
		if (rotation == true)
		{
			this.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90.0f + RadToRotation(rad)));
//			Debug.Log("rad=" + rad + " / angle=" + this.transform.localRotation.z);
		}
		return vec;
	}
	Vector3 AngleToVector3(int ang,float spd) {
		Vector3 vec = Vector3.zero;
		float rot = AngleToRotation(ang);
		float rad = ((rot - 90.0f) * Mathf.PI) / 180.0f;
		vec.x = spd * Mathf.Cos(rad);           //角度からベクトル求める.
		vec.y = spd * Mathf.Sin(rad);
		//Debug.Log("ang=" + ang + " / rot=" + rot + " / rad=" + rad);
		return vec;
	}



	float GetRad(Vector3 pos) {
		return Mathf.Atan2(	pos.y - this.transform.localPosition.y,
							pos.x - this.transform.localPosition.x
						);
	}
	float RadToRotation(float rad) {
		return (float)(((rad * 180.0f) / Mathf.PI) % 360.0f);
	}

	float AngleToRotation(int ang) {
		return (float)(360.0f - (360.0f * ((float)(ang % 256) / 256.0f)));
	}
	int RotationToAngle(float rot)
	{
		return ((int)(256.0f * (360.0f - ((rot % 360.0f) / 360.0f)))) % 256;
	}


	public void Generate(ObjectManager.TYPE type, Vector3 pos, int ang, int spd)
	{
		obj_mode = ObjectManager.MODE.INIT;
		obj_type = type;
		this.transform.localPosition = pos;
		this.transform.localRotation = Quaternion.identity;
		angle = ang;
		speed = spd;
		LIFE = 1;
		vect = Vector3.zero;
	}


	void OnTriggerStay2D(Collider2D collider)
	{
		if (obj_mode == ObjectManager.MODE.NOHIT)
		{
			return;
		}
		ObjectCtrl other = collider.gameObject.GetComponent<ObjectCtrl>();
		if (other.obj_mode == ObjectManager.MODE.NOHIT)
		{
			return;
		}
		if (NOHIT == true)
		{
			return;
		}
		if (other.NOHIT == true)
		{
			return;
		}
		switch (other.obj_type)
		{
			case ObjectManager.TYPE.ENEMY1:          // 敵・自分がMYSHIPかMYSHOTの場合に処理.
				if (this.obj_type == ObjectManager.TYPE.MYSHIP)
				{
					other.MainPic.sprite = MANAGE.TSURU[1];
					MANAGE.Set(ObjectManager.TYPE.NOHIT_EFFECT, 2, other.transform.localPosition, 0, 0);
					MAIN.GAME_END = true;
				}
				break;
		}
	}



	public void Damage(int damage)
	{
		LIFE -= damage;
		if (LIFE <= 0)
		{
			Dead();
		}
		MainPic.color = COLOR_DAMAGE;
	}


	public void Dead()
	{
		obj_mode = ObjectManager.MODE.NOHIT;
		switch (obj_type)
		{
			case ObjectManager.TYPE.ENEMYBULLET:
			case ObjectManager.TYPE.ENEMYBIGBULLET:
				if (MANAGE.destroy == true)
				{
					obj_type = ObjectManager.TYPE.NOHIT_EFFECT;
					LIFE = 1;
					mode = 0;
					count = 0;
					MainHit.enabled = false;
					MainPic.sprite = SPR_CRUSH[0];
					MainPic.color = COLOR_NORMAL;
				}
				else
				{
					MANAGE.Return(this);
				}
				//game.ReturnObject(this);
				break;
			case ObjectManager.TYPE.MYSHOT:
				MANAGE.Return(this);
				break;
			case ObjectManager.TYPE.ENEMY1:
			case ObjectManager.TYPE.ENEMY2:
			case ObjectManager.TYPE.ENEMY3:
				//game.score += SCORE_TABLE[mode];
				if (MANAGE.destroy == false)
				{
					switch (obj_type)
					{
						case ObjectManager.TYPE.ENEMY1:
							//manage.score += MainGameLoop.SCORETABLE[0];
							break;
					}
				}
				//FileOutput.Log("dead:pos=" + this.transform.localPosition);
				obj_type = ObjectManager.TYPE.NOHIT_EFFECT;
				LIFE = 1;
				mode = 0;
				count = 0;
				MainHit.enabled = false;
				MainPic.sprite = SPR_CRUSH[0];
				MainPic.color = COLOR_NORMAL;
				//SoundManager.Instance.PlaySE((int)SoundHeader.SE.ENEMY_DEAD);
				if (Random.Range(0, 32) == 0)
				{
					mode = 2;
				}
				if (MANAGE.expert == true)
				{
					int spd = Random.Range(2, 7);
					MANAGE.Set(ObjectManager.TYPE.ENEMYBIGBULLET, 0, this.transform.localPosition, 0, spd);
					MANAGE.Set(ObjectManager.TYPE.ENEMYBULLET, 0, this.transform.localPosition, 0, spd + Random.Range(1, 3));
				}
				//manage.Return(this);
				break;
			case ObjectManager.TYPE.MYSHIP:
				//obj_type = ObjectManager.TYPE.NOHIT_EFFECT;
				LIFE = 1;
				//mode = 2;
				//count = 0;
				//MainHit.enabled = false;
				//game.mode = MainGameLoop.MODE.GAMEOVER;
				//game.count = -1;
#if MUTEKI
				// 死亡処理スキップ
#else
				MANAGE.stop = true;
				mode = 100;
#endif
				break;
		}
		MainPic.color = COLOR_NORMAL;
		count = 0;
	}


	public void DisplayOff() {
		MainPic.enabled = false;
		MainHit.enabled = false;
	}



	public void Erase()
	{
		MainPic.enabled = false;
		MainHit.enabled = false;
		MainPic.color = COLOR_NORMAL;
		MainPic.sprite = SPR_CRUSH[7];
	}
}
