using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public int startingPoint; // index (position of Platform)
    public float speed;
    public Transform[] points; // An array of platform's destinations

    private int i; // index for array
    private bool canMove = true;

    [SerializeField] private Collider2D triggerArea;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = points[startingPoint].position; // platform starting position set
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            if (i == startingPoint)
            {
                canMove = false;
            }

            i++;
            if (i == points.Length) // check if current indexx is last one
            {
                i = 0;
            }
        }

        // moving platform to next index i
        if (canMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            canMove = true;
        }
    }
}
