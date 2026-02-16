using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class TitleManager : MonoBehaviour

{
    public string sceneName;//スタートボタンを押して読み込むシーン名
                            // public InputAction submitAction; //決定のInputAction;

    // private void OnEnable()
    //{
    // submitAction.Enable();//InputActionを有効化
    // }
    // private void OnDisable()
    //{
    //    submitAction.Disable();//InputActionを無効化
    //}

    //InputSyastem?Actionsで決めたUIマップのSubmitアクションが押されたとき
    void OnSubmit(InputValue yaluse)
    {
        Load();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //インスペクター上で登録したキーのいずれかがおされていたら成立
        //if (submitAction.WasPressedThisFrame())
        // {
        //    Load();
        //}
        //列挙型のKeyboard型の値を変数kdに代入
        //Keyboard kd = Keyboard.current;
        //if (kd != null)//キーボードがつながっていれば
        //{
        //エンターキーがおされた状態
        // if (kd.enterKey.wasPressedThisFrame)
        //{
        //Load();
        //}

        //}
    }
    //シーンを読み込むメソッド作成
    public void Load()
    {
        GameManager.totalScore = 0;//新しくゲームを始めるにあたってスコアをリセット
        SceneManager.LoadScene(sceneName);
    }
}
