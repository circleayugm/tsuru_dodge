using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneCtrl : MonoBehaviour
{
    [SerializeField]
    Text MSG_PRESSBUTTON;
    [SerializeField]
    Text MSG_SCORE;
    [SerializeField]
    Text MSG_GAMEOVER;



    TouchPanelCtrl TOUCH;
    ObjectManager MANAGE;

    public enum MODE
    {
        DEMO=0,
        PLAY,
        OVER
	}
    public bool GAME_END = false;
    public MODE mode = MODE.DEMO;

    int difficulty = 20;
    int score = 0;
    int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        TOUCH = GameObject.Find("root_main").GetComponent<TouchPanelCtrl>();
        MANAGE = GameObject.Find("root_main").GetComponent<ObjectManager>();
        Application.targetFrameRate = 60;
        while (MANAGE.boot != true)
        {
            count++;
        }
        count = 0;
        MSG_GAMEOVER.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        MSG_SCORE.text = "SCORE : " + score;

        switch(mode)
        {
            case MODE.DEMO:
                {
                    if (Input.GetKeyUp(KeyCode.Space) == true)
                    {
                        MSG_PRESSBUTTON.enabled = false;
                        difficulty = 20;
                        count = -1;
                        score = 0;
                        mode = MODE.PLAY;
                    }
                    else
                    {
                        if ((count >> 4) % 2 == 0)
                        {
                            MSG_PRESSBUTTON.enabled = true;
                        }
                        else
                        {
                            MSG_PRESSBUTTON.enabled = false;
                        }
                    }
                }
                break;
            case MODE.PLAY:
                {
                    int side = Random.Range(0, 4);

                    if (count % difficulty == 0)
                    {
                        switch (side)
                        {
                            case 0:
                                MANAGE.Set(ObjectManager.TYPE.ENEMY1, 0, new Vector3(8.5f, Random.Range(-5.0f, 5.0f), 0), Random.Range(-32, 32) + 64, Random.Range(2, 6));
                                break;
                            case 1:
                                MANAGE.Set(ObjectManager.TYPE.ENEMY1, 0, new Vector3(-8.5f, Random.Range(-5.0f, 5.0f), 0), Random.Range(-32, 32) + 192, Random.Range(2, 6));
                                break;
                            case 2:
                                MANAGE.Set(ObjectManager.TYPE.ENEMY1, 0, new Vector3(Random.Range(8.0f, -8.0f), 5.5f, 0), Random.Range(-32, 32), Random.Range(2, 6));
                                break;
                            case 3:
                            case 4:
                                MANAGE.Set(ObjectManager.TYPE.ENEMY1, 0, new Vector3(Random.Range(8.0f, -8.0f), -5.5f, 0), Random.Range(-32, 32) + 128, Random.Range(2, 6));
                                break;
                        }
                    }
                    if (count % 120 == 60)
                    {
                        difficulty--;
                        if (difficulty < 3) difficulty = 3;
					}
                    score++;
                    if (GAME_END==true)
                    {
                        count = -1;
                        mode = MODE.OVER;
					}
                }
                break;
            case MODE.OVER:
                {
                    if (Input.GetKeyUp(KeyCode.Space) == true)
                    {
                        MANAGE.ResetAll();
                        MSG_PRESSBUTTON.enabled = false;
                        MSG_GAMEOVER.enabled = false;
                        difficulty = 20;
                        count = -1;
                        score = 0;
                        GAME_END = false;
                        mode = MODE.PLAY;
                    }
                    else
                    {
                        MSG_GAMEOVER.enabled = true;
                        if ((count >> 4) % 2 == 0)
                        {
                            MSG_PRESSBUTTON.enabled = true;
                        }
                        else
                        {
                            MSG_PRESSBUTTON.enabled = false;
                        }
                    }
                }
                break;
		}

        count++;
    }
}
