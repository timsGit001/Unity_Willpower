using UnityEngine;

public class Gate : MonoBehaviour
{

    [Header("鑰匙")]
    public GameObject key;
    [Header("開門聲")]
    public AudioClip openAudio;

    private Animator amt;
    private AudioSource m_audioSource;       // 音效來源
    // Start is called before the first frame update
    void Start()
    {
        amt = GetComponent<Animator>();
        m_audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 事件之觸發
    /// </summary>
    /// <param name="collision"></param> 啟動觸發之Collider2D
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "warrior" && key == null)
        {
            // 開門
            amt.SetTrigger("open");
            m_audioSource.PlayOneShot(openAudio);
        }
    }
}
