using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    private float x; //x방향 키 입력
    public int moveSpeed; //플레이어 걷기 속도
    public int jumpForce; //점프 크기

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            Debug.Log("sf");
        }
    }

    void FixedUpdate()
    {
        x = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(x*moveSpeed , rb.velocity.y);

    }
}
