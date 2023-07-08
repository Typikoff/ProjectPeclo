using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpEssenceJumps : MonoBehaviour
{
    private float speed = 4f;
    private float maxLifeSpan = 0f;
    private bool toEnd = false;

    [SerializeField] private Rigidbody2D rb;
    private Animator jumpEssenceAnimator;

    // Start is called before the first frame update
    void Start()
    {
        maxLifeSpan = Time.time + 10f;
        jumpEssenceAnimator = GetComponent<Animator>();
        rb.velocity = transform.right * speed;
    }

    void Update()
    {
        jumpEssenceAnimator.SetBool("toEnd", toEnd);
    }

    void FixedUpdate()
    {
        SpeedControl();

        if (Time.time > maxLifeSpan) // disable after time runs out
        {
            toEnd = true;
            rb.velocity = new Vector2(0, 0);
            Destroy(gameObject, 1.5f);
        }
    }

    private void SpeedControl() // regulates traffic jams lol
    {
        if (rb.velocity.x > 10f)
        {
            rb.velocity = new Vector2(10f, rb.velocity.y);
        }
        else if (rb.velocity.x < -10f)
        {
            rb.velocity = new Vector2(-10f, rb.velocity.y);
        }
        else if (rb.velocity.y > 10f)
        {
            rb.velocity = new Vector2(rb.velocity.x, 10f);
        }
        else if (rb.velocity.y < -10f)
        {
            rb.velocity = new Vector2(rb.velocity.x, -10f);
        }
    }

    void OnTriggerEnter2D (Collider2D hitInfo)
    {
        Debug.Log(hitInfo.name);

        if (hitInfo.tag == "DeathZone")
        {
            maxLifeSpan = 0f;
        }
        else
        {
            if (rb.velocity.y > 0 && rb.velocity.y < 10)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + 1);
            }
            else if (rb.velocity.y > -10 && rb.velocity.y < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - 1);
            }
        }
    }
}
