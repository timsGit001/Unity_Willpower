using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    [Header("音效播放器")]
    public AudioSource aud;
    [Header("按鈕音效")]
    public AudioClip btnClipSound;


    private IEnumerator cor;


    private void delayStartGame()
    {
        SceneManager.LoadScene("MainGame01");
    }
    public void StartGame()
    {
        aud.PlayOneShot(btnClipSound);
        Invoke("delayStartGame", 0.5f);

    }
    private void delayBackMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void BackMenu()
    {
        aud.PlayOneShot(btnClipSound);
        Invoke("delayBackMenu", 0.5f);
    }
    private void delayLeaveGame()
    {
        Application.Quit();
    }
    public void LeaveGame()
    {
        aud.PlayOneShot(btnClipSound);
        //Invoke("delayLeaveGame", 0.5f);

        // 練習使用Coroutine
        //cor = delayChangeScene(0.5f);
        //StartCoroutine(cor);

        new WaitForSeconds(0.5f);
        Application.Quit();
    }
    /*
    private IEnumerator delayChangeScene(float delayTime) {
        yield return new WaitForSeconds(0.5f);
        Application.Quit();
    }
*/
    
}
