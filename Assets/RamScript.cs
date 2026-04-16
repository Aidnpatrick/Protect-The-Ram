using UnityEngine;

public class RamScript : MonoBehaviour
{
    private GameControlScript gameControlScript;
    public GameObject damageNotificationPrefab, hitPrefab;
    public AudioClip lost;
    public AudioSource audioSource;
    public int health = 0;

    void Start()
    {
        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();
        health = 300;
    }

    void Update()
    {
        if(health <= 0)
        {
            gameControlScript.GameOver();
            audioSource.PlayOneShot(lost);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("Bullet") && collision.name.Contains("Enemy"))
        {
            
            gameControlScript.DamageNotification(25, transform.position);

            GameObject hitClone = Instantiate(hitPrefab, transform.position, Quaternion.identity);
            Destroy(hitClone, 0.1f);

            Destroy(collision.gameObject);
            health -= 10;
        }

        if(collision.name.Contains("Cyber"))
        {
            Destroy(collision.gameObject);
            health -= 30;
        }
    }
}
