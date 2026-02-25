using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState           // ゲームの状態
{
    InGame,                     // ゲーム中
    GameClear,                  // ゲームクリア
    GameOver,                   // ゲームオーバー
    GameEnd,                    // ゲーム終了
}



public class GameManager : MonoBehaviour
{


    //スコア追加
    public static int totalScore; //合計スコア

    // ゲームの状態
    public static GameState gameState;
    public string nextSceneName;            // 次のシーン名

    //サウンド関連
    public AudioClip meGameClear;//ゲームクリアの音源
    public AudioClip meGameOver;//ゲームオーバーの音源
    AudioSource soundPlayer; //AudioSouece型の変数

    public bool isGameClear = false;//ゲームクリア判定
    public bool isGameOver = false;//ゲームオーバー判定

    //ワールドマップで最後に入ったエントランスのドア番号
    public static int currentDoorNumber = 0;

    //所持アイテム　鍵の管理
    public static int keys = 1;
    //どのステージの鍵が入手済みかを管理
    public static Dictionary<string, bool> keyGot; //シーン名,true/false

    //所持アイテム　矢の管理
    public static int arrows = 10;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        gameState = GameState.InGame;//ステータスをゲーム中にする
        soundPlayer = GetComponent<AudioSource>(); //AudioSourceを参照する

        //keyGotが何もない状態だった時のみ初期化
        if (keyGot == null)
        {
            keyGot = new Dictionary<string, bool>();
        }

        //もしも現シーン名がDictionary(keyGot)に登録されていなければ
        if (!(keyGot.ContainsKey(SceneManager.GetActiveScene().name)))
        {
            //Dictionary(keyGot)に登録しておく(現シーン名、鍵の取得情報false)
            keyGot.Add(SceneManager.GetActiveScene().name, false);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if
            (gameState == GameState.GameClear)
        {
            //Debug.Log("LateClear");
            soundPlayer.Stop();//ステージ曲を止める
            soundPlayer.PlayOneShot(meGameClear);//ゲームクリアの音を一回だけ鳴らす
            isGameClear = true; //クリアフラグ
            Invoke("GameStatusChange", 0.02f);
            //gameState = GameState.GameEnd;//ゲームの状態を更新
        }
        else if (gameState == GameState.GameOver)
        {
            soundPlayer.Stop();//ステージ曲を止める
            soundPlayer.PlayOneShot(meGameOver);//ゲームクリアの音を一回だけ鳴らす
            isGameOver = true; //ゲームオーバーフラグ
            Invoke("GameStatusChange", 0.02f);
            //gameState = GameState.GameEnd;//ゲームの状態を更新
        }
    }

    void GameStatusChange()
    {
        gameState = GameState.GameEnd; //ゲームの状態を更新
    }

    //リスタート
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //次へ
    public void Next()
    {
        SaveDataManager.SaveGamedata(); //ワールドマップに戻るタイミングでオートセーブ
        SceneManager.LoadScene(nextSceneName);
    }

    //PlayerController経由で「UIマップのSubmit」が押されたとき呼び出される
    public void GameEnd()
    {
        //UI表示が終わって最後の状態であれば
        if (gameState == GameState.GameEnd)
        {
            //ゲームクリアの状態なら
            if (isGameClear)
            {
                {
                    Next();
                }
            }
            else if (isGameOver)//ゲームオーバーの状態なら
            {
                Restart();
            }
        }
    }

}
