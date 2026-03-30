
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameControlScript gameControlScript;
    public GameDataBaseScript gameDataBaseScript;
    public CameraScript cameraScript;
    public GameObject childGameObject;
    public GameObject bulletPrefab;
    public SpriteRenderer spriteRenderer, childSpriteRenderer;
    public Rigidbody2D rb, childRb;
    public float[] weights = {0,0};
    public float health = 100, speed = 1.2f;
    public float hitCooldown = 0, shotCooldown;
    public GameObject targetMain, wayPoint = null;
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
        if(name.Contains("CyberTruck"))
        {
            wayPoint = Instantiate(Resources.Load<GameObject>("WayPoint"), transform.position + new Vector3(50,0,0), Quaternion.identity);
            speed = 3.2f;
            health = 250;
            GetComponent<CircleCollider2D>().radius = 1.25f;
            Destroy(transform.GetChild(0).gameObject);
        }
        
        transform.Rotate(0,0,Random.Range(0,360));
    }
    void Update()
    {
        hitCooldown -= Time.deltaTime;
        shotCooldown -= Time.deltaTime;
        GameObject target = null;
        //if(name.Contains("CyberTruck")) target = wayPoint;
        closestTroop = gameControlScript.FindNearestObject(gameControlScript.troops, 50, gameObject);
        closestTurret = gameControlScript.FindNearestObject(gameControlScript.turrets, 50, gameObject);
        GameObject ram = gameControlScript.FindNearestObjectDictionary(gameControlScript.ramTracking, Mathf.Infinity, gameObject);

        if (name.Contains("Enemy"))
        {
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
        if(name.Contains("CyberTruck"))
        {
            target = closestTurret;
        }
        
        if(target == null)
            target = ram;
        //else if(name.Contains("CyberTruck"))
        if (name.Contains("Troop"))
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
        else if(target == null)
        {
            
            rb.linearVelocity = Vector2.zero;
        }
        /*
        float zRotation = transform.eulerAngles.z;

        if (zRotation > 180) zRotation -= 360;
        spriteRenderer.flipX = zRotation > 90 || zRotation < -90;
        if(name.Contains("Troop"))        
            spriteRenderer.flipX = !(zRotation > 90 || zRotation < -90);

        if(childGameObject != null)
        {
            childSpriteRenderer.flipX = spriteRenderer.flipX;
        }
        */
        targetMain = target;
        UpdateSpriteFacing();    
    }
    void UpdateSpriteFacing()
    {
        float zRotation = transform.eulerAngles.z;
        if (zRotation > 180) zRotation -= 360; // normalize -180 to 180

        bool facingLeft = zRotation > 90 || zRotation < -90;
        if(name.Contains("Troop"))    
        spriteRenderer.flipX = !facingLeft;
        else 
        spriteRenderer.flipX = facingLeft;
    

        if (childGameObject != null)
            childSpriteRenderer.flipX = spriteRenderer.flipX;
    }

    public void Shoot()
    {
        rb.linearVelocity = Vector2.zero;
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
        //rb.constraints = RigidbodyConstraints2D.FreezeRotation;
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
    void OnDestroy()
    {
        if (name.Contains("CyberTruck"))
        {
            for(int i = 0; i < 5; i++)
            {
                gameControlScript.SpawnEnemy(transform.position + new Vector3(Random.Range(-0.5f, 0.6f), Random.Range(-0.5f, 0.6f), 0));
                Destroy(wayPoint);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name.Contains("Bullet") && !collision.name.Contains("Enemy") && (name.Contains("Enemy") || name.Contains("CyberTruck")))
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
                Destroy(gameObject);
        }
        if(collision.name.Contains("Bullet") && collision.name.Contains("Troop") && (name.Contains("Enemy") || name.Contains("CyberTruck")))
        {
            Destroy(collision.gameObject);
            
            health -= 5;
            if(health <= 0)
                Destroy(gameObject);
        }

        if(collision.CompareTag("Buildings") && collision.gameObject == targetMain)
        {
            collision.GetComponent<BuildingScript>().health -= 25;
            Destroy(gameObject);
        }
        
        if(collision.name.Contains("Ram") && name.Contains("CyberTruck"))
        {
            Destroy(gameObject);
        }
        if((collision.name.Contains("Troop") || collision.name.Contains("Enemy")) && name.Contains("CyberTruck"))
        {
            Destroy(collision.gameObject);
        }
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if(collision.name.Contains("Enemy") && name.Contains("Troop") && hitCooldown <= 0)
        {
            health -= 10;
            hitCooldown = 1;
            if (health <= 0)
                Destroy(gameObject);
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


        if(collision.CompareTag("Buildings") && name.Contains("Enemy")&& hitCooldown <= 0)
        {
            collision.GetComponent<BuildingScript>().health -= 10;
            hitCooldown = 1;
        }
        if(collision.name.Contains("Ram"))
            gameControlScript.ramTracking[collision.gameObject] -= 10;
    }
}
