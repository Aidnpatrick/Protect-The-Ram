using UnityEngine;

public class RamScript : MonoBehaviour
{
    private GameControlScript gameControlScript;
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
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("Bullet") && collision.name.Contains("Enemy"))
        {
            
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
