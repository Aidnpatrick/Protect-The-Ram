using UnityEditor.Rendering;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameControlScript gameControlScript;
    public GameDataBaseScript gameDataBaseScript;
    public float[] weights = {0,0};
    public float health = 100, speed = 1.2f;
    public float hitCooldown = 0;
    void Start()
    {
        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();
        gameDataBaseScript = GameObject.Find("GameControl").GetComponent<GameDataBaseScript>();
        health = 100;
    }
    void Update()
    {
        hitCooldown -= Time.deltaTime;

        GameObject target = null;

        if (name.Contains("Enemy"))
        {
            GameObject closestTroop = gameControlScript.FindNearestObject(gameControlScript.troops, Mathf.Infinity, gameObject);
            GameObject closestTurret = gameControlScript.FindNearestObject(gameControlScript.turrets, Mathf.Infinity, gameObject);

            if (closestTroop == null && closestTurret == null)
                target = null;
            else if (closestTroop == null)
                target = closestTurret;
            else if (closestTurret == null)
                target = closestTroop;
            else
            {
                float troopDist = Vector3.Distance(closestTroop.transform.position, transform.position);
                float turretDist = Vector3.Distance(closestTurret.transform.position, transform.position);
                target = troopDist < turretDist ? closestTroop : closestTurret;
            }
        }
        else if (name.Contains("Troop"))
            target = gameControlScript.FindNearestObject(gameControlScript.enemies, 20, gameObject);

        if (target != null)
            MoveTowards(target.transform.position);

        if (health <= 0)
            Destroy(gameObject);
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
        if(collision.name.Contains("Bullet") && name.Contains("Enemy"))
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
        if(collision.name.Contains("Enemy") && name.Contains("Troop"))
        {
            health -= 10;
            hitCooldown = 1;
        }
        if(collision.name.Contains("Troop") && name.Contains("Enemy")) {
            health -= 10;
            hitCooldown = 1;
        }
    }
}
