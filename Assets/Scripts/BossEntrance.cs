using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossEntrance : MonoBehaviour
{
    //各エントランスのクリア状況を管理
    public static Dictionary<int, bool> stagesClear;//シーン切り替え先
    public string sceneName;//開いているかの状況
    bool isOpened;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Entrance");

        //リストがない時の情報取得とセッティング
        if (stagesClear == null)
        {
            stagesClear = new Dictionary<int, bool>(); // 最初に初期化が必要

            //集めてきたEntranceを全点検
            for (int i = 0; i < obj.Length; i++)
            {
                //Entranceオブジェクトが持っているEntranceControllerを取得
                EntranceController entranceController = obj[i].GetComponent<EntranceController>();
                if (entranceController != null)
                {
                    //帳簿(KeyOpendディクショナリー)に変数doorNumberと変数openedの状況を記録
                    stagesClear.Add(entranceController.doorNumber,false);
                }
            }
        }
        else
        {
            int sum = 0;//クリアがどのくらいあるのかカウント用
            //Entranceの数分だけStagesClearの中身をチェック
            for(int i =0; i < obj.Length; i++)
            {
                if (stagesClear[i])//StagesClearディクショナリーの中身を順にチェック
                {
                    sum++; //もしtrue(クリア済み）ならカウント
                }
            }
            if(sum >=obj.Length)//もしクリアの数（trueの数）とEntranceの数が一致していたら
            {
                //全部クリアしたので扉を空ける
                GetComponent<SpriteRenderer>().enabled = false;//見た目をなくす
                isOpened = true;//扉があいたという状態にする
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //触れた相手がPlayer　かつ　扉が開いていれば
        if(collision.gameObject.tag=="Player"&& isOpened)
        {
            SceneManager.LoadScene(sceneName);//Boss部屋に行く
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
