using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPointSpriteMove : MonoBehaviour
{
    public Transform Player;
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player").transform;
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.position = Player.position;
            audioManager.Play("SaveSound");
        }
    }
}
