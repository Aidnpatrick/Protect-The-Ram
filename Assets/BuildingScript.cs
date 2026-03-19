using System;
using UnityEngine;

public class BuildingScript : MonoBehaviour
{
    public GameControlScript gameControlScript;
    public Building building;
    public GameObject bulletPrefab;
    public float shotCooldown = 0;
    public int typeOfBuilding = 0;

    void Start()
    {
        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();
        shotCooldown = building.fireRate;
        if(building.name.Contains("Turret"))
            typeOfBuilding = 0;
    }

    void Update()
    {
        //turrets
        if(typeOfBuilding == 0)
        {
            shotCooldown -= Time.deltaTime;
            Shoot();
        }
    }
    void Shoot()
    {
        if(shotCooldown <= 0)
        {
            GameObject target = gameControlScript.FindNearestObject(gameControlScript.enemies, building.range);
            if(target == null) return;
            GameObject bulletClone = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Vector3 direction;
            direction = target.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bulletClone.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            bulletClone.name = "Bullet" + building.name.Replace("Turret" , "");
            Rigidbody2D bulletbp = bulletClone.GetComponent<Rigidbody2D>();
            bulletbp.linearVelocity = bulletClone.transform.right * 20;
            //bulletClone.transform.position += new Vector3(0.65f,0f,0);
            //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //bulletClone.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            shotCooldown = building.fireRate;
        }
    }
    
}
