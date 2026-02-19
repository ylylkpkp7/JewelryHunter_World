using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class World_UIController : MonoBehaviour


{
    //各EntranceのDoorNumberごとに開錠か非開錠かをまとめておく（falseかtrue)
    public static Dictionary<int, bool> keyOpened;

    public TextMeshProUGUI keyText;
    int currentKeys;
    public TextMeshProUGUI arrowText;
    int currentArrows;

    GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //WorldMap中のEntranceオブジェクトを集めて配列objに代入
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Entrance");

        //リストがない時の情報取得とセッティング
        if (keyOpened == null)
        {
            keyOpened = new Dictionary<int, bool>(); // 最初に初期化が必要

            //集めてきたEntranceを全点検
            for (int i = 0; i < obj.Length; i++)
            {
                //Entranceオブジェクトが持っているEntranceControllerを取得
                EntranceController entranceController = obj[i].GetComponent<EntranceController>();
                if (entranceController != null)
                {
                    //帳簿(KeyOpendディクショナリー)に変数doorNumberと変数openedの状況を記録
                    keyOpened.Add(
                        entranceController.doorNumber,
                        entranceController.opened
                    );
                }
            }
        }
        //プレイヤーの位置
        player = GameObject.FindGameObjectWithTag("Player");
        //暫定のプレイヤーの位置
        //Vector2 currentPlayer =new Vector2(0.0);
        Vector2 currentPlayerPos = Vector2.zero;

        //GameManagerに記録されているcurrentDoorNumberと一致するdoorNumberをもっているEntranceを探す
        for (int i = 0; i < obj.Length; i++)
        {
            //EntranceのEntranceControllerの変数doorNumberが、GameManagerの把握しているcurrentDoorNumberと同じかどうかチェックする
            if (obj[i].GetComponent<EntranceController>().doorNumber == GameManager.currentDoorNumber)
            {
                //暫定プレイヤーの位置を一致したEntranceオブジェクトの位置に置き換え
                currentPlayerPos = obj[i].transform.position;
            }
        }
        //最終的に残ったcurrentPlayerPosの座標がPlayerの座標になる
        player.transform.position = currentPlayerPos;

    }

    // Update is called once per frame
    void Update()
    {
        //把握していた鍵の数と、GameManagerの鍵の数に違いがでたら、正しい数にUIを更新
        if (currentKeys != GameManager.keys)
        {
            currentKeys = GameManager.keys;
            keyText.text = currentKeys.ToString();
        }
        //把握していた矢の数と、GameManagerの矢の数と違いが出たら、正しい数となるようUIを更新
        if (currentArrows != GameManager.arrows)
        {
            currentArrows = GameManager.arrows;
            arrowText.text = currentArrows.ToString();
        }
    }
}
