using UnityEngine;
using System.Collections;

public class CameraCtrl2D : MonoBehaviour
{
    [Header("跟拍目標")]
    public Transform target;
    [Header("跟拍速度"), Range(0, 100)]
    public float speed = 3.5f;
    [Header("鏡頭距離")]
    public Vector3 offset;
    [Header("鏡頭晃動間隔"), Range(0, 1)]
    public float shakeInterval = 0.05f;
    [Header("鏡頭晃動值"), Range(0, 5)]
    public float shakeValue = 0.5f;
    [Header("鏡頭晃動次數"), Range(0, 10)]
    public int shakeCount = 3;

    private Vector3 nextPos;
    private bool faceToRight = true; // 鏡頭右邊保留多一點

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// LateUpdate將會在每次Update()後執行
    /// </summary>
    void LateUpdate()
    {
        Track();
    }

    /// <summary>
    /// 跟拍
    /// </summary>
    void Track()
    {
        // 角色面向哪 鏡頭也要往哪
        if (!faceToRight && target.localEulerAngles.y == 0)
        {
            faceToRight = true;
            offset.x += 10.0f;
        }
        else if (faceToRight && target.localEulerAngles.y == 180)
        {
            faceToRight = false;
            offset.x -= 10.0f;
        }

        // 最後有乘上Time.deltaTime(每禎幾秒)是為適應不同裝置會有不同的禎數表現
        nextPos = Vector3.Lerp(transform.position, target.position + offset, 0.5f * speed * Time.deltaTime);
        // 新座標的Z軸依然不能變 因為若攝影機與跟拍目標同Z軸 就會啥也看不到
        nextPos.z = transform.position.z;

        transform.position = nextPos;
    }

    /// <summary>
    /// 鏡頭晃動效果
    /// </summary>
    /// <returns></returns>
    public IEnumerator CamShake()
    {
        for (int i = 0; i < shakeCount; i++)
        {
            transform.position += Vector3.up * shakeValue;
            yield return new WaitForSeconds(shakeInterval);
            transform.position -= Vector3.up * shakeValue;
            yield return new WaitForSeconds(shakeInterval);
        }
    }
}
