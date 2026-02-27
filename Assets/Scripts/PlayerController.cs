using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rbody;　// Rigidbody2D型の変数
    float axisH = 0.0f;　 // 入力
    public float speed = 3.0f;　// 移動速度
    public float jump = 9.0f;　// ジャンプ力
    public LayerMask groundLayer;// 着地できるレイヤー
    bool goJump = false;// ジャンプ開始フラグ
    bool onGround = false;//// 地面フラグ
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

    public static int playerLife = 10; //Playerの体力

    bool inDamage; //ダメージ管理フラグ

    public float shootSpeed = 12.0f; //矢の速度
    public float shootDelay = 0.25f; //発射間隔
    public GameObject arrowPrefab; //矢のプレハブ
    public GameObject gate; //矢の発射位置を担当するオブジェクト

    

    //PlayerLifeの回復メソッド
    public static void PlayerRecovery(int life)
    {
        playerLife += life; //引数lifeだけ回復
        if (playerLife > 10) playerLife = 10;

    }

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

    void OnAttack(InputValue yalue)
    {
        if(GameManager.arrows > 0)//矢の残数があれば
        {
            ShootArrow(); //矢を発射
        }
    }



    void ShootArrow()
    {
        //矢の放つ音を音を鳴らす
        SoundManager.currentSoundManager.PlaySE(SEType.Shoot);
        GameManager.arrows--; //矢を減らす
        Quaternion r; //回転の3軸の値
        //Playerの絵の向きが右向き
        if (transform.localScale.x > 0)
        {
            r = Quaternion.Euler(0, 0, 0);
        }
        else //Playerの絵の向きが左向き
        {
            r = Quaternion.Euler(0, 0, 180);
        }
        //Gateオブジェクトの位置にｒの回転で矢を生成
        GameObject arrowObj = Instantiate(arrowPrefab,gate.transform.position, r);
        //生成した矢自身のRigidbody2Dを取得
        Rigidbody2D arrowRbody = arrowObj.GetComponent<Rigidbody2D>();
        //Playerの絵の向きに合わせた方向に矢を飛ばす
        arrowRbody.AddForce(new Vector2(transform.localScale.x, 0)* shootSpeed, ForceMode2D.Impulse);
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

        playerLife = 10; //体力を全快にする
    }

    // Update is called once per frame
    void Update()

    {
        if (GameManager.gameState != GameState.InGame || inDamage)
        {
            //もしダメージ管理フラグが立っていたら点滅処理
            if (inDamage)
            {
                //Sin関数の角度に経過時間（一定リズムの値）を与えると、等間隔でプラスとマイナスの結果が得られる
                float val = Mathf.Sin(Time.time * 50);

                //等間隔で変わっているであろうvalの値をチェックして、プラスの時間帯は表示、マイナスの時間帯は非表示
                if (val > 0)
                {
                    GetComponent<SpriteRenderer>().enabled = true;
                }
                else
                {
                    GetComponent<SpriteRenderer>().enabled = false;
                }
            }
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
        //ゲームのステータスがInGameじゃない時、またはダメージ管理フラグがtrueの時
        if (GameManager.gameState != GameState.InGame || inDamage == true)
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
            //アイテムゲットの音を鳴らす
            SoundManager.currentSoundManager.PlaySE(SEType.ItemGet);
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
        else if (collision.gameObject.tag == "Enemy")
        {
            if (!inDamage)//ダメージ中でなければ
            {
                //ぶつかった相手のオブジェクト情報を引数
                GetDamage(collision.gameObject);
            }
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
    //ダメージメソッド
    void GetDamage(GameObject target)
    {
        //プレイ中のみ発動
        if (GameManager.gameState == GameState.InGame)
        {
            //ダメージの音を鳴らす
            SoundManager.currentSoundManager.PlaySE(SEType.GetDamage);

            playerLife -= 1;//体力を減少
            if (playerLife > 0)//まだゲームオーバーじゃなければ
            {
                //ぶつかった相手と反対方向にノックバック
                rbody.linearVelocity = new Vector2(0, 0);
                Vector3 v = (transform.position - target.transform.position).normalized;
                rbody.AddForce(new Vector2(v.x * 4, v.y * 4), ForceMode2D.Impulse);
                //ダメージ管理フラグを立てる
                inDamage = true;
                //時間差でダメージ管理フラグを降ろす
                Invoke("DamageEnd", 0.25f);
            }
            else　//playerLifeが0以下になってしまったら
            {
                GameOver();
            }
        }
    }

    void DamageEnd()//ダメージ管理フラグをOFFにするメソッド
    {
        inDamage = false;
        //ダメージ終了（点滅終了）と同時に確実に姿を表示させる
        GetComponent<SpriteRenderer>().enabled = true;
    }
}