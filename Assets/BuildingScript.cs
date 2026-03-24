using UnityEngine;

public class BuildingScript : MonoBehaviour
{
    public GameControlScript gameControlScript;
    public Building building;
    public GameObject bulletPrefab, troopPrefab;

    public float shotCooldown = 0, reloadingCooldown = 0;


    public int ammo = 0, numOfTroops;
    public int typeOfBuilding = 0;

    public bool isReloading = false;
    public float health;
    void Start()
    {
        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();
        if(building == null)
            return;

            
        shotCooldown = building.fireRate;
        ammo = building.ammo;
        health = building.health;

        if(building.name.Contains("Turret"))
            typeOfBuilding = 0;
        if(building.name.Contains("Camp")) 
            typeOfBuilding = 1;

    }

    void Update()
    {
        shotCooldown -= Time.deltaTime;
        if(typeOfBuilding == 0)
        {
            HandleShooting();
        }
        if(typeOfBuilding == 1)
        {
            if(shotCooldown <= 0 && numOfTroops > 0)
            {
                numOfTroops -= 1;
                SpawnTroop();
                shotCooldown = building.fireRate;
            }
        }
        if(health <= 0)
            Destroy(gameObject);
    }

    void HandleShooting()
    {
        if(isReloading)
        {
            reloadingCooldown -= Time.deltaTime;

            if(reloadingCooldown <= 0)
            {
                ammo = building.ammo;
                isReloading = false;
            }
            return;
        }

        if(ammo <= 0)
        {
            StartReload();
            return;
        }

        if(shotCooldown <= 0)
        {
            Shoot();
            shotCooldown = building.fireRate;
        }
    }

    void StartReload()
    {
        isReloading = true;
        reloadingCooldown = building.reload;
    }

    void Shoot()
    {
        GameObject target = gameControlScript.FindNearestObject(
            gameControlScript.enemies,
            building.range,
            gameObject
        );

        if(target == null) return;

        GameObject bulletClone = Instantiate(
            bulletPrefab,
            transform.position,
            Quaternion.identity
        );

        Vector3 direction = target.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        bulletClone.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        bulletClone.transform.Rotate(0, 0, Random.Range(-building.recoil, building.recoil+1));

        bulletClone.name = "Bullet" + building.name.Replace("Turret", "");

        Rigidbody2D rb = bulletClone.GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            rb.linearVelocity = bulletClone.transform.right * 20;
        }
        ammo--;
        Destroy(bulletClone.gameObject, 4);
    }
    
    void SpawnTroop()
    {
        GameObject troopClone = Instantiate(troopPrefab, transform.position, Quaternion.identity);
        troopClone.transform.position += new Vector3(-1f,0,0);
        troopClone.GetComponent<SpriteRenderer>().color = Color.darkGreen;
        troopClone.tag = "Troop";
        troopClone.name = "Troop";
    }
}