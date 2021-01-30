﻿using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float atk;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 撞到有 Enemy腳本的 東東時
        Enemy target = collision.gameObject.GetComponent<Enemy>();
        if (target != null)
        {
            // 讓它受到 傷害
            target.OnInjury(atk);
        }

        Destroy(gameObject);
    }
}
