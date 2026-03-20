using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameControlScript gameControlScript;
    public GameDataBaseScript gameDataBaseScript;
    public float[] weights = {0,0};
    public float health = 100, speed = 1.2f;
    void Start()
    {
        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();
        gameDataBaseScript = GameObject.Find("GameControl").GetComponent<GameDataBaseScript>();
        health = 100;
    }
    void Update()
    {
        GameObject target = gameControlScript.FindNearestObject(gameControlScript.turrets, Mathf.Infinity, gameObject);
        
        if(target == null) {}
        else
            MoveTowards(target.transform.position);
        
        if(health <= 0) Destroy(gameObject);
    }

    public void MoveTowards(Vector3 targetPosition)
    {
        //Vector2 direction = (player.transform.position - transform.position);
        Vector2 direction = (targetPosition - transform.position);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        
        direction.y = direction.y;
        direction.x = direction.x;

        if (direction.magnitude < 0.1f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.MoveRotation(angle);
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name.Contains("Bullet"))
        {
            Destroy(collision.gameObject);
            foreach(Building currentBuilding in gameDataBaseScript.buildings)
            {
                if(currentBuilding.name.Replace("Turret", "").Contains(collision.name.Replace("Bullet", "")))
                {
                    health -= currentBuilding.damage;
                    return;
                }
            }        
        }
    }
}
