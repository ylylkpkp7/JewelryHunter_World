using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rbody;
    float axisH = 0.0f;
    public float speed = 3.0f;
    public float jump = 9.0f;
    public LayerMask groundLayer;//レイヤー指定
    bool goJump = false;//true,false
    bool onGround = false;//true,false

    // アニメーション対応
    Animator animator; // アニメーター

    //値はあくまでアニメーションクリップ名
    public string stopAnime = "Idle";
    public string moveAnime = "Run";
    public string jumpAnime = "Jump";
    public string goalAnime = "Goal";
    public string deadAnime = "Dead";
    string nowAnime = "";
    string oldAnime = "";

    public int score = 0; //スコア

    InputAction moveAction;　//Moveアクション
    InputAction jumpAction;　//Jumpアクション
    PlayerInput input;　//PlayerInputコンポーネント

    GameManager gm;　//GameManagerスクリプト

    void OnMove(InputValue value)
    {
        //取得した情報をVector2形式で抽出
        Vector2 moveInput = value.Get<Vector2>();
        axisH = moveInput.x; //そのX成分をaxisHに代入
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            goJump = true;
        }
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rbody = this.GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();        // Animator を取ってくる
        nowAnime = stopAnime;                       // 停止から開始する
        oldAnime = stopAnime;                       // 停止から開始する

        input = GetComponent<PlayerInput>();　//playerInputコンポーネントの取得
        moveAction = input.currentActionMap.FindAction("Move");　//Moveアクション取得
        jumpAction = input.currentActionMap.FindAction("Jump");　//Jumpアクション取得
        InputActionMap uiMap = input.actions.FindActionMap("UI"); //UIマップ取得
        uiMap.Disable(); //UIマップは無効化

        //GameObject型のアタッチされている特定のコンポーネントを探してくるメソッド
        gm = GameObject.FindFirstObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()

    {
        if (GameManager.gameState != GameState.InGame)
        {
            return;//Updateを中断
        }
        //地上判定
        onGround = Physics2D.CircleCast(transform.position,//発射位置
            0.2f,//円の半径
            Vector2.down,//発射方向
            0.0f,//発射距離
            groundLayer);//検出するレイヤー

        // if (Input.GetButtonDown("Jump"))//キャラクターをジャンプさせる
        //{
        //  goJump = true;//ジャンプフラグを立てる

        //  }
        //In
        //if (jumpAction.WasPressedThisFrame())
        // {
        //    goJump = true;
        // }


        //axisH = Input.GetAxisRaw("Horizontal");
        //InputActionのPlayerマップの"Move"アクションに登録されたボタンをVector2形式で読み取り、そのうちのｘ成分をaxisHに代入
        //axisH = moveAction.ReadValue<Vector2>().x;

        if (axisH > 0.0f)                           // 向きの調整
        {
            //Debug.Log("おされてる")
            transform.localScale = new Vector2(1, 1);   // 右移動
        }
        else if (axisH < 0.0f)
        {
            transform.localScale = new Vector2(-1, 1); // 左右反転させる


        }

        // アニメーション更新
        if (onGround)       // 地面の上
        {
            if (axisH == 0)
            {
                nowAnime = stopAnime; // 停止中
            }
            else
            {
                nowAnime = moveAnime; // 移動
            }
        }
        else                // 空中
        {
            nowAnime = jumpAnime;
        }
        if (nowAnime != oldAnime)
        {
            oldAnime = nowAnime;
            animator.Play(nowAnime); // アニメーション再生
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.gameState != GameState.InGame)
        {
            return;//Updateを中断
        }
        if ((onGround || axisH != 0))//地面の上　or 速度が0ではない
        {

            //速度を更新する
            rbody.linearVelocity = new Vector2(axisH * speed, rbody.linearVelocity.y);
        }

        if (onGround && goJump)//地上の上でジャンプキーが押された
        {

            //ジャンプさせる
            Vector2 jumpPw = new Vector2(0, jump);//ジャンプさせるベクトルを作る
            rbody.AddForce(jumpPw, ForceMode2D.Impulse);//瞬間的な力を加える
            goJump = false;// ジャンプフラグを下ろす
        }
    }

    //接触
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            Goal(); //ゴール!
        }
        else if (collision.gameObject.tag == "Dead")
        {
            GameOver(); //ゲームオーバー
        }
        else if (collision.gameObject.tag == "ScoreItem")
        {
            // スコアアイテム
            ScoreItem item = collision.gameObject.GetComponent<ScoreItem>();  // ScoreItemを得る			
            score = item.itemdata.value;                // スコアを得る
            UIController ui = Object.FindFirstObjectByType<UIController>();      // UIControllerを探す
            if (ui != null)
            {
                ui.UpdateScore(score);                  // スコア表示を更新する
            }
            score = 0; //次に備えてスコアをリセット
            Destroy(collision.gameObject);              // アイテム削除する
        }
    }

    //ゴール
    public void Goal()
    {
        animator.Play(goalAnime);
        GameManager.gameState = GameState.GameClear;//ステータス切り替え
        GameStop();//プレイヤーのVelcityをストップ
    }

    //ゲームオーバー
    public void GameOver()
    {
        animator.Play(deadAnime);
        GameManager.gameState = GameState.GameOver;
        GameStop();             // プレイヤーのVelcityをストップ

        // ゲームオーバー演出
        GetComponent<CapsuleCollider2D>().enabled = false;      // 当たりを消す
        rbody.AddForce(new Vector2(0, 5), ForceMode2D.Impulse); // 上に少し跳ね上げる
        Destroy(gameObject, 2.0f); // 2秒後にヒエラルキーからオブジェクトを抹消


    }
    //プレイヤー停止
    void GameStop()
    {
        rbody.linearVelocity = new Vector2(0, 0);

        input.currentActionMap.Disable(); //いったんPlayerマップを無効化
        input.SwitchCurrentActionMap("UI"); //アクションマップをUIマップに切り替え
        input.currentActionMap.Enable(); //UIマップを有効化
    }

    //UI表示にSubmitボタンが押されたら
    void OnSubmit(InputValue yalue)
    {
        //もしゲーム中でなければ
        if (GameManager.gameState != GameState.InGame)
        {
            //GameManagerスクリプトのGameEndメソッドの発動
            gm.GameEnd();
        }
    }


    //プレイヤーのaxisH()の値を取得
    public float GetAxisH()
    {
        return axisH;
    }


}