
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameControlScript gameControlScript;
    public GameDataBaseScript gameDataBaseScript;
    public GameObject childGameObject;
    public GameObject bulletPrefab;
    public SpriteRenderer spriteRenderer, childSpriteRenderer;
    public Rigidbody2D rb, childRb;
    public float[] weights = {0,0};
    public float health = 100, speed = 1.2f;
    public float hitCooldown = 0, shotCooldown;
    public GameObject targetMain;
    public GameObject closestTroop, closestTurret;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        gameControlScript = GameObject.Find("GameControl").
        GetComponent<GameControlScript>();
        gameDataBaseScript = GameObject.Find("GameControl").GetComponent<GameDataBaseScript>();
        health = 75;

        if(transform.childCount > 0)
        {
            childGameObject = transform.GetChild(0).gameObject;
            childSpriteRenderer = childGameObject.GetComponent<SpriteRenderer>();
            if(name.Contains("Troop")) childSpriteRenderer.flipY = true;
            childRb = childGameObject.GetComponent<Rigidbody2D>();
        }
        else
        {
            speed = 2f;
            health = 120;
        }
        /*transform.rotation = Quaternion.Euler(0,0,-90);
        rb.linearVelocity = transform.right * 20;*/
        transform.Rotate(0,0,Random.Range(0,360));
    }
    void Update()
    {
        hitCooldown -= Time.deltaTime;
        shotCooldown -= Time.deltaTime;

        GameObject target = null;
        closestTroop = gameControlScript.FindNearestObject(gameControlScript.troops, 30, gameObject);
        closestTurret = gameControlScript.FindNearestObject(gameControlScript.turrets, 30, gameObject);

        if (name.Contains("Enemy"))
        {
            GameObject ram = gameControlScript.FindNearestObjectDictionary(gameControlScript.ramTracking, Mathf.Infinity, gameObject);


            if (closestTroop == null && closestTurret == null)
                target = ram;
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
            target = gameControlScript.FindNearestObject(gameControlScript.enemies, childGameObject != null ? 13f : 10, gameObject);

        if (target != null)
        {
            if(childGameObject != null && Vector3.Distance(target.transform.position, transform.position) < 10 && shotCooldown <= 0)
            {
                Shoot();

                //MoveTowards(transform.position);
                shotCooldown = 1;
            }
            else if(childGameObject != null && Vector3.Distance(target.transform.position, transform.position) > 10)
                MoveTowards(target.transform.position);
            else if(childGameObject == null)
                MoveTowards(target.transform.position);
        }
        float x = rb.linearVelocity.x;

        if (x < 0)
            spriteRenderer.flipX = true;
        else if (x > 0)
            spriteRenderer.flipX = false;

        if(childGameObject != null)
        {
            //childGameObject.transform.rotation = transform.rotation;
            


            if (x < 0)
                childSpriteRenderer.flipX = true;
            else if (x > 0)
                childSpriteRenderer.flipX = false;
            
        }
        targetMain = target;
    }

    public void Shoot()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;

        GameObject target;
        target = gameControlScript.FindNearestObject(gameControlScript.enemies, 10, gameObject);
        if(name.Contains("Enemy"))
        {
            if (closestTroop == null && closestTurret == null) target = null;
            else if (closestTroop == null) target = closestTurret;
            else if (closestTurret == null) target = closestTroop;
            else
            {
                float troopDist = Vector3.Distance(closestTroop.transform.position, transform.position);
                float turretDist = Vector3.Distance(closestTurret.transform.position, transform.position);
                target = troopDist < turretDist ? closestTroop : closestTurret;
                
            }
        }
        if(target == null)
        target = gameControlScript.FindNearestObjectDictionary(gameControlScript.ramTracking, Mathf.Infinity, gameObject);
        if(target == null)
        {
            Debug.Log("target error");
            return;
        }


        GameObject bulletClone = Instantiate(
            bulletPrefab,
            transform.position,
            Quaternion.identity
        );

        Vector3 direction = target.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        bulletClone.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        bulletClone.transform.Rotate(0, 0, Random.Range(-10f, 10f));

        bulletClone.name = "Bullet" + name;

        Rigidbody2D brb = bulletClone.GetComponent<Rigidbody2D>();
        brb.linearVelocity = bulletClone.transform.right * 20;
        Destroy(bulletClone.gameObject, 4);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    
    public void MoveTowards(Vector3 targetPosition)
    {
        //Vector2 direction = (player.transform.position - transform.position);
        Vector2 direction = (targetPosition - transform.position);
        
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
        if(collision.name.Contains("Bullet") && !collision.name.Contains("Enemy") && name.Contains("Enemy"))
        {
            Destroy(collision.gameObject);
            foreach(Building currentBuilding in gameDataBaseScript.buildings)
            {
                if(currentBuilding.name.Replace("Turret", "").Contains(collision.name.Replace("Bullet", "")))
                {
                    health -= currentBuilding.damage / 2;
                    if (health <= 0)
                    {
                        gameControlScript.money += 2;
                        Destroy(gameObject);
                    }
                    return;
                }
            }        
        }
        if(collision.name.Contains("Bullet") && !collision.name.Contains("Troop") && !collision.name.Contains("Turret") && name.Contains("Troop"))
        {
            Destroy(collision.gameObject);
            health -= 5;
            if(health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if(collision.name.Contains("Enemy") && name.Contains("Troop") && hitCooldown <= 0)
        {
            health -= 10;
            hitCooldown = 1;
            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
        if(collision.name.Contains("Troop") && name.Contains("Enemy")&& hitCooldown <= 0) {
            health -= 5;
            hitCooldown = 1;
            if (health <= 0)
            {
                gameControlScript.money += 2;
                Destroy(gameObject);
            }
        }

        if((collision.name.Contains("Turret") || collision.name.Contains("Camp")) && name.Contains("Enemy")&& hitCooldown <= 0)
        {
            collision.GetComponent<BuildingScript>().health -= 10;
            hitCooldown = 1;
        }
        if(collision.name.Contains("Ram"))
            gameControlScript.ramTracking[collision.gameObject] -= 10;
    }
}
