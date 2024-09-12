using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private GameObject focalPoint;
    public float speed = 50.0f;
    public bool hasPowerup;
    public float powerupStrength = 15.0f;
    public GameObject powerupIndicator;
    private string powerupType = "None";

    public GameObject rocketPrefab;
    private GameObject tmpRocket;
    private Coroutine powerupCountdown;

    public float hangTime;
    public float smashSpeed;
    public float explosionForce;
    public float explosionRadius;
    bool smashing = false;
    float floorY;

    public string gameOverSceneName;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
    }

    // Update is called once per frame
    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");

        playerRb.AddForce(focalPoint.transform.forward * speed * forwardInput * Time.deltaTime);

        float horizontalInput = Input.GetAxis("HorizontalTwo");

        playerRb.AddForce(focalPoint.transform.right * speed * horizontalInput * Time.deltaTime);
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);

        if (powerupType == "Shoot" && Input.GetKeyDown(KeyCode.UpArrow))
        {
            LaunchRockets();
        }

        if (powerupType == "Smash" && Input.GetKeyDown(KeyCode.Space) && !smashing)
        {
            smashing = true;
            StartCoroutine(Smash());
        }

        if (transform.position.y < -4)
        {
            SceneManager.LoadScene(gameOverSceneName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            powerupType = other.GetComponent<Powerup>().powerupName;
            Destroy(other.gameObject);
            StartCoroutine(PowerupCountdownRoutine());
            powerupIndicator.SetActive(true);
            if (powerupCountdown != null)
            {
                StopCoroutine(powerupCountdown);
            }
            powerupCountdown = StartCoroutine(PowerupCountdownRoutine());
        }
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerup = false;
        powerupType = "None";
        powerupIndicator.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && hasPowerup && powerupType == "Bump")
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = (collision.gameObject.transform.position - transform.position);
            //Debug.Log("collided with " + collision.gameObject.name + " with powerup set to " + hasPowerup);
            enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
        }
    }

    void LaunchRockets()
    {
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity);
            tmpRocket.GetComponent<RocketBehavior>().Fire(enemy.transform);
        }
    }

    IEnumerator Smash()
    {
        var enemies = FindObjectsOfType<Enemy>();

        floorY = transform.position.y;

        float jumpTime = Time.time + hangTime;
        while(Time.time < jumpTime)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, smashSpeed);
            yield return null;
        }

        while(transform.position.y > floorY)
        {
        playerRb.velocity = new Vector2(playerRb.velocity.x, -smashSpeed * 2);
        yield return null;
        }

        for (int i = 0; i < enemies.Length; i++)
        {
            if(enemies[i] != null)
            {
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
            }
        }

            smashing = false;
    }
}
