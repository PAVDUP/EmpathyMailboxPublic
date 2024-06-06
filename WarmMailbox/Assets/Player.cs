using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    private float x; //x���� Ű �Է�
    public int moveSpeed; //�÷��̾� �ȱ� �ӵ�
    public int jumpForce; //���� ũ��

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
