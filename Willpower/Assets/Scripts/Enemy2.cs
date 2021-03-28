using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

// 第一次套用此腳本時，會自動添加的元件(一次RequireComponent可包含1~3個)
[RequireComponent(typeof(AudioSource), typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    [Header("移動速度"), Range(0, 500)]
    public float speed = 100f;
    [Header("攻擊力"), Range(0, 500)]
    public float atk = 20f;
    [Header("攻擊範圍"), Range(0, 50)]
    public float rangeAtk = 3f;
    [Header("延遲幾秒攻擊"), Range(0, 5)]
    public float delayTimeAtk = 0.7f;
    [Header("攻擊CD"), Range(0, 10)]
    public float cdTimeAtk = 3f;
    [Header("攻擊範圍位移")]
    public Vector3 offsetAtk;
    [Header("攻擊範圍大小")]
    public Vector3 sizeAtk;
    [Header("血量"), Range(100, 5000)]
    public float hp = 2500f;

    [Header("血量值")]
    public Text textHp;
    [Header("血量圖片")]
    public Image imgHp;

    [Header("死亡事件")]
    public UnityEvent onDead;

    private AudioSource m_audioSource;       // 音效來源
    private Rigidbody2D m_rigidbody2D;       // 2D 剛體
    private Animator m_animator;             // 動畫控制器
    private Player m_player; // Player腳本
    private CameraCtrl2D m_cam;
    private float hpMax;
    private float cdTimer;

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_player = FindObjectOfType<Player>(); // 用類型尋找物件 (小心場上同時有重複的類型，你會不知他抓到誰)
        m_cam = FindObjectOfType<CameraCtrl2D>();
        hpMax = hp;
        textHp.text = hp.ToString();
        cdTimer = 0;
    }

    private void Update()
    {
        if (m_animator.GetBool("dieSwitch")) return;

        DoMove();
    }

    /// <summary>
    /// 在unity上繪製圖形 (玩家是看不到的)
    /// </summary>
    private void OnDrawGizmos()
    {
        // 調顏色
        Gizmos.color = new Color(0, 1, 0, 0.5f);

        Gizmos.DrawCube(transform.position + transform.right * offsetAtk.x + transform.up * offsetAtk.y, sizeAtk);
    }

    /// <summary>
    /// 移動
    /// </summary>
    private void DoMove()
    {
        float direction = 1.0f; // 正:往右 負:往左
        // 面向玩家處理
        // transform.eulerAngles = (transform.position.x < m_player.transform.position.x) ? Vector3.zero : new Vector3(0, 180, 0);
        if (transform.position.x < m_player.transform.position.x)
            transform.eulerAngles = Vector3.zero;
        else
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            direction = -1.0f;
        }

        // 判斷與玩家距離
        float dis = Vector2.Distance(transform.position, m_player.transform.position);
        if (dis > 15f) ;
        else if (dis > rangeAtk)
        {
            // 物理移動 方法二:往某座標移動 (同樣也會產生出加速度)
            m_rigidbody2D.MovePosition(transform.position + transform.right * speed * Time.deltaTime);
        }
        else
        {
            DoAttack();
        }

        // 移動動畫 (判斷是否有加速度)
        m_animator.SetBool("walkSwitch", m_rigidbody2D.velocity.magnitude > 0);
    }

    /// <summary>
    /// 攻擊
    /// </summary>
    private void DoAttack()
    {
        // 停止往前
        m_rigidbody2D.velocity = Vector3.zero;
        if (cdTimer < cdTimeAtk)
        {
            cdTimer += Time.deltaTime;
        }
        else
        {
            m_animator.SetTrigger("doAttack");
            cdTimer = 0;
            StartCoroutine(DelayAttack());
        }
    }

    /// <summary>
    /// 延遲後攻擊
    /// </summary>
    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(delayTimeAtk);
        Collider2D hit = Physics2D.OverlapBox(transform.position + transform.right * offsetAtk.x + transform.up * offsetAtk.y, sizeAtk, 0, 1 << 9);
        if (hit) m_player.OnInjury(atk);
        StartCoroutine(m_cam.CamShake());
    }

    /// <summary>
    /// 受傷
    /// </summary>
    /// <param name="damage">受傷量</param>
    public void OnInjury(float damage)
    {
        hp -= damage;

        // 受傷
        m_animator.SetTrigger("onHurt");

        if (hp <= 0.0f) OnDeath(); // 死亡

        textHp.text = hp.ToString();
        imgHp.fillAmount = hp / hpMax;
    }

    /// <summary>
    /// 死亡
    /// </summary>
    private void OnDeath()
    {
        onDead.Invoke();
        hp = 0.0f;
        m_animator.SetBool("dieSwitch", true);
        m_rigidbody2D.Sleep();
        m_rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<CapsuleCollider2D>().enabled = false;
    }

}
