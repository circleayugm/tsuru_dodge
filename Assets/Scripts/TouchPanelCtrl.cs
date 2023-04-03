/*
 *		TouchPanelCtrl.cs 
 *			タッチパネル(転じてマウスカーソル)管理
 * 
 * 
 * 
 * 
 *		20220810	7日前くらいにRSc100用に再構成
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TouchPanelCtrl : MonoBehaviour {

	// インスペクタ指定.
	[SerializeField]
	RectTransform uGUIroot;     // ドットサイズが指定されたuGUIのパネル.
	[SerializeField]
	SpriteRenderer SPR_TARGET;	// ターゲット(照準)



	// 公開変数.
	public Vector3 pos;			// タッチ座標(中央が0,0・Zは無視).
	public bool on = false;     // タッチされている？(true=されている・false=されていない).
	public int count = 0;       // 状態が変わった瞬間を0としたカウンタ・押したor離した瞬間0.
	public Vector3 move;        // 移動量(Zは無視).


	// 内部変数.
	Vector3 pos_old;			// 1フレーム前のタッチ座標.
	bool on_old = false;		// 1フレーム前のタッチ判定.
	float multi;				// 座標取得拡大率.

	void Awake()
	{
		Vector3 reso = new Vector3(((float)uGUIroot.rect.width / (float)Screen.width), ((float)uGUIroot.rect.height / (float)Screen.height), 0);    // 解像度に依存しない倍率.
		if (reso.x < reso.y)                // 解像度比が大きな側をタッチパネルサイズ基準に指定(表示画面の外をタッチされても一応座標は返る).
		{
			multi = reso.y;
		}
		else
		{
			multi = reso.x;
		}
	}



	// Use this for initialization
	void Start()
	{
		on_old = false;
		on = false;
		count = 0;
	}

	// Update is called once per frame
	void Update()
	{
		Vector3 p = Input.mousePosition;    // タッチ位置取得.
		pos_old = pos;
		pos = new Vector3((p.x - (Screen.width / 2)) * multi, (p.y - (Screen.height / 2)) * multi, 0);  // 画面中心を(0,0,0)とした座標軸に変換.
		if (pos.x > 250)	// 照準の移動範囲を制限
		{
			pos.x = 250;
		}
		else if (pos.x < -250)
		{
			pos.x = -250;
		}
		if (pos.y > 180)
		{
			pos.y = 180;
		}
		else if (pos.y < -180)
		{
			pos.y = -180;
		}
		SPR_TARGET.transform.localPosition = pos/47;
		SPR_TARGET.transform.localScale = new Vector3(3, 3, 3);
#if false // タッチ判定・今回は不要なので除外
		on = Input.GetMouseButton(0);
		if (on==true) {
			move = pos - pos_old;
		}
		else {
			move = Vector3.zero;
		}
		if (on != on_old)
		{
			count = 0;
			on_old = on;
			move = Vector3.zero;
		}
		else {
			count++;
		}
#endif
	}
}
