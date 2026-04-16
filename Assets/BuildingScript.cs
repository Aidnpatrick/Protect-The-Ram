
using System.Collections;
using UnityEngine;

public class BuildingScript : MonoBehaviour
{
    public GameControlScript gameControlScript;
    public Building building;
    public GameObject bulletPrefab, troopPrefab, gunPrefab, hitPrefab, explosionPrefab, damageNotificationPrefab;
    public GameObject reloadSmokeParticle;
    public AudioSource audioSource;
    public AudioClip rocketShootSound, turretShootSound, explosion;

    public float shotCooldown = 0, reloadingCooldown = 0;


    public int ammo = 0, numOfTroops;
    public int typeOfBuilding = 0;

    public bool isReloading = false;
    public float health;
    public GameObject shotPrefab;
    void Start()
    {
        if (!name.Contains("Boogie"))
        {
            tag = "Buildings";
        }
        else
        {
            tag = "Untagged";
            GetComponent<CircleCollider2D>().radius = 1f;
            transform.localScale -= new Vector3(0.2f,0.2f,0f);
        }
        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();
        if(building == null)
            return;

//        Debug.Log("Images/" + name.Replace("(Clone)", ""));
        if(Resources.Load<Sprite>("Images/" + building.image.Replace("(Clone)", "")) != null)
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/" + building.image);
            
        shotCooldown = building.fireRate;
        ammo = building.ammo;
        health = building.health;

        if(building.name.Contains("Turret"))
            typeOfBuilding = 0;
        if(building.name.Contains("Camp")) 
            typeOfBuilding = 1;
        if(building.name.Contains("Digger"))
        {
            typeOfBuilding = 2;
            if(transform.parent.name.Contains("Deposit"))
                gameControlScript.amountOfMines++;  
        }
        if (building.name.Contains("Mine"))
        {
            typeOfBuilding = 3;
        }
        if(building.name.Contains("Healer"))
        {
            typeOfBuilding = 4;
        }
    }

    void Update()
    {
        numOfTroops = Mathf.Clamp(numOfTroops, 0,5);

        shotCooldown -= Time.deltaTime;
        if(typeOfBuilding == 0)
        {
            HandleShooting();
        }
        if(typeOfBuilding == 1)
        {
            if(!gameControlScript.isRoundDone && shotCooldown <= 0 && numOfTroops < 5)
            {
                SpawnTroop();
                numOfTroops++;
                shotCooldown = building.fireRate;
            }
        }
        if(typeOfBuilding == 4)
        {

        }
        if(health <= 0)
            Destroy(gameObject);
    }

    public void HealTurrets()
    {
        foreach(GameObject i in gameControlScript.turrets)
        {
            if(Vector3.Distance(i.transform.position, transform.position) < building.range && !i.name.Contains("Healer"))
            {
                BuildingScript bs = i.GetComponent<BuildingScript>();
                bs.health = Mathf.Clamp(bs.health + building.damage, 0, bs.building.health);
            }
        }
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
        StartCoroutine(reloadEffect());
    }
    IEnumerator reloadEffect()
    {
        for(int i = 0 ; i < building.reload; i++)
        {
            gameControlScript.CreateParticle(reloadSmokeParticle, transform.position - new Vector3(0.5f,0, 0), 0, "Smoke", true);
            yield return new WaitForSeconds(1f);
        }
    }

    

    void Shoot()
    {

        GameObject target = gameControlScript.FindNearestObject(
            gameControlScript.enemies,
            building.range,
            gameObject
        );


        if(target == null) return;

        if(name.Contains("Missile"))
        {
            audioSource.PlayOneShot(rocketShootSound);
        }
        else
        {
            audioSource.PlayOneShot(turretShootSound);
        }

        GameObject bulletClone = Instantiate(
            bulletPrefab,
            transform.position,
            Quaternion.identity
        );

        Vector3 direction = target.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        bulletClone.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        bulletClone.transform.Rotate(0, 0, Random.Range(-building.recoil, building.recoil+1));

        bulletClone.name = "Bullet" + building.name;

        Rigidbody2D brb = bulletClone.GetComponent<Rigidbody2D>();
        
        brb.linearVelocity = bulletClone.transform.right * 20;

        ammo--;

        if(name.Contains("Poison"))
            bulletClone.GetComponent<SpriteRenderer>().color = Color.green;

        if(name.Contains("Missile"))
        {
            bulletClone.transform.localScale += new Vector3(0.1f,0.1f,0);
            bulletClone.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/Missile");
            brb.linearVelocity = bulletClone.transform.right * 30;
        }

        
        Destroy(bulletClone.gameObject, 4);
    }
    void OnDestroy()
    {
        gameControlScript.money += building.cost / 4;

        if(typeOfBuilding == 2 && transform.parent.name.Contains("Deposit"))
            gameControlScript.amountOfMines--;
        

    }
    void SpawnTroop()
    {
        GameObject troopClone = Instantiate(troopPrefab, transform.position, Quaternion.identity);
        troopClone.transform.position += new Vector3(-Random.Range(0.5f, 1f),-Random.Range(-0.5f,0.6f),0);
        troopClone.GetComponent<EnemyScript>().originArmyCamp = gameObject;

        troopClone.GetComponent<SpriteRenderer>().color = Color.white;
        troopClone.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/Troop");
        troopClone.GetComponent<SpriteRenderer>().flipY = true;

        if(Random.Range(0,10) <= 100f)
            Instantiate(gunPrefab, troopClone.transform.position, Quaternion.identity, troopClone.transform);
        troopClone.tag = "Troop";
        troopClone.name = "Troop";
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name.Contains("Bullet") && collision.name.Contains("Enemy") && tag == "Buildings")
        {
            GameObject hitClone = Instantiate(hitPrefab, transform.position, Quaternion.identity);
            Destroy(hitClone, 0.1f);
            Destroy(collision.gameObject);
            health -= 5;
            gameControlScript.DamageNotification(5, transform.position);
        }
        if(collision.tag == "Enemy" && name.Contains("Boogie"))
        {
            GameObject explosionClone = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            audioSource.PlayOneShot(explosion);
            explosionClone.name = "BoogieExplosion";
            Destroy(explosionClone, 0.35f);
            Destroy(gameObject);
        }
    }

}