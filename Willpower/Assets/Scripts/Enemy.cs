using UnityEngine;
using UnityEngine.UI;           //引用 介面 API


//第一次套用腳本時執行
//添加元件(類型(元件)，類型(元件))
[RequireComponent(typeof(AudioSource), typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class enemy : MonoBehaviour
{
   
    [Header("攻擊距離"), Range(0, 100)]
    public float rangeATK = 10;
    [Header("移動速度"), Range(0, 1000)]
    public float speed = 10;
    [Header("攻擊力"), Range(0, 1000)]
    public float ATK = 100;
    [Header("血量"), Range(0, 5000)]
    public float hp = 2500;
    [Header("血量文字")]
    public Text textHp;
    [Header("血量圖片")]
    public Image imgHp;


    private Animator ani;
    private AudioSource aud;
    private Rigidbody2D rig;
    private float hpMax;
    private Player player;

    private void Start()
    {
        ani = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();
        rig = GetComponent<Rigidbody2D>();
        hpMax = hp;
        player = FindObjectOfType<Player>();    //透過類行尋找物件<類型>() - 不能是重複物件
    }

    private void Update()
    {
        if (ani.GetBool("死亡")) return;

        Move();
    }
    /// <summary>
    /// 受傷
    /// </summary>
    /// <param name="damage"></param>
    public void Damage(float damage)
    {
        hp -= damage;                   //遞減
        ani.SetTrigger("受傷");         //受傷動畫
        textHp.text = hp.ToString();    //血量文字.文字內容 = 血量.轉字串()
        imgHp.fillAmount = hp / hpMax;  //血量圖片.填滿長度 = 目前血量 / 最大血量

        if (hp <= 0) Dead();
    }

    /// <summary>
    /// 死亡
    /// </summary>
    private void Dead()
    {
        hp = 0;
        textHp.text = 0.ToString();
        ani.SetBool("死亡", true);
        //取得元件<膠囊碰撞>().啟動 = 關閉
        GetComponent<CapsuleCollider2D>().enabled = false;
        //剛體.睡著()
        rig.Sleep();
        //剛體.約束 - 約束.凍結全部
        rig.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    /// <summary>
    /// 追蹤玩家與面向玩家
    /// </summary>
    private void Move()
    {
        /** 判斷式寫法
        if (transform.position.x > player.transform.position.x)
        {
            transform.eulerAngles = new Vector3(0, 100, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        */

        // y = x 是否大於 玩家 x ? 是 y 為 180 : 否 y 為 0
        // 第0層 取得目前執行的動畫資訊
        AnimatorStateInfo info = ani.GetCurrentAnimatorStateInfo(0);
        // 如果正在 攻擊 或 受傷 => 不移動
        if (info.IsName("攻擊") || info.IsName("受傷")) return;


        float direction = 1.0f; // 正:往右 負:往左
        // 面向玩家處理
        // transform.eulerAngles = (transform.position.x < m_player.transform.position.x) ? Vector3.zero : new Vector3(0, 180, 0);
        if (transform.position.x < player.transform.position.x)
            transform.eulerAngles = Vector3.zero;
        else
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            direction = -1.0f;
        }

        //距離 = 二維.距離(A座標.B座標)
        float dis = Vector2.Distance(transform.position, player.transform.position);
        if (dis > 15f) ;
        else if (dis > rangeATK)
        {
            //剛體.移動座標(座標 + 前方 * 一禎 * 移動速度)
            rig.MovePosition(transform.position + transform.right * Time.deltaTime * speed);
        }
        else
        {
            Attack();
        }

        //動畫.設定布林值("跑步開關",剛體.加速度.值>0)
        ani.SetBool("跑步", rig.velocity.magnitude > 0);
    }


    /// <summary>
    /// 攻擊冷卻與攻擊行為
    /// </summary>
    private void Attack()
    {
        rig.velocity = Vector3.zero;
        ani.SetTrigger("攻擊");
    }

}
