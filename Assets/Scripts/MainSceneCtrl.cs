using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneCtrl : MonoBehaviour
{
    TouchPanelCtrl TOUCH;
    ObjectManager MANAGE;

    enum MODE
    {
        DEMO=0,
        PLAY,
        OVER
	}

    MODE mode = MODE.DEMO;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (count==0)
        {

		}

        MANAGE.Set(ObjectManager.TYPE.ENEMY1, 0, new Vector3(0, 0, 0), Random.Range(0,256),Random.Range(2,8));


        count++;
    }
}
