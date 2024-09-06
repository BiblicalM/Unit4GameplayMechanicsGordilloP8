using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    private Rigidbody enemyRb;
    private GameObject player;

    public GameObject strongerVariant;
    public bool strong;

    // Start is called before the first frame update
    void Start()
    {
        if (!strong)
        {
            int i = Random.Range(0, 6);
            if (i == 2)
            {
                Instantiate(strongerVariant, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookDirection = (player.transform.position - transform.position).normalized;

        enemyRb.AddForce(lookDirection * speed * Time.deltaTime);

        if(transform.position.y < -10) { Destroy(gameObject); }
    }
}
