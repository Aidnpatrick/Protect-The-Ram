using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;



public class Building
{
    public string name = "";
    public float fireRate = 0;
    public float damage = 0;
    public string information = "";
    public int id = 0;
    public string image = "";
    public int cost = 0;
    public float health = 0;
    public float range = 0;
    public float recoil = 0;
    public int ammo = 0;
    public float reload = 0;
    public static List<Building> buildingCurrent = new List<Building>();
    public Building(
        string name,
        float fireRate,
        float damage,
        string information,
        int id,
        string image,
        int cost,
        int health,
        float range,
        float recoil,
        int ammo,
        float reload)
    {
        buildingCurrent.Add(this);

        this.name = name;
        this.fireRate = fireRate;
        this.damage = damage;
        this.information = information;
        this.id = id;
        this.image = image;
        this.cost = cost;
        this.health = health;
        this.range = range;
        this.recoil = recoil;
        this.ammo = ammo;
        this.reload = reload;
    }

    public Building(Building other)
    {
        buildingCurrent.Add(this);
        
        name = other.name;
        fireRate = other.fireRate;
        damage = other.damage;
        information = other.information;
        this.id = other.id;
        this.image = other.image;
        cost = other.cost;
        health = other.health;
        range = other.range;
        recoil = other.recoil;
        ammo = other.ammo;
        reload = other.reload;
    }
    public Building() {
        
    }
};

public class NPC
{
    public string name, imageName, information;
    public float speed, fireRate, damage, health, recoil, reload;
    public int cost, ammo;
}

public class GameDataBaseScript : MonoBehaviour
{
    public GameControlScript gameControlScript;
    public GameObject selectionContainer;
    public List<Building> buildings = new List<Building>();
    void Start()
    {
        Building.buildingCurrent.Clear(); // prevent duplicates
        
        new Building("Basic Turret",1,38,"Versatile, good for every situation. Shoots medium ammo.  (SINGLE TARGET)", 0, "BasicTurret", 100, 250, 15, 4, 10, 3f);
        new Building("Heavy Turret",4, 200,"Slow but does big damage. Shoots big ammo.  (SINGLE TARGET)", 1, "HeavyTurret", 250, 450, 20, 1, 5, 8);
        new Building("Machine Turret", 0.1f, 9f, "Low damage but has fast fire-rate. Shoots small ammo. (SINGLE TARGET)", 2, "MachineTUrret", 350, 350, 15, 10, 50, 6.5f);
        new Building("Army Camp", 4, 0, "Spawns a troop every 4 seconds.", 3, "ArmyCamp", 300, 450, 0, 0, 0,0);
        new Building("Gold Mine", 0, 0, "A building that gives passive income. Can only be placed on deposits.", 4, "GoldMine", 180, 150, 0, 0, 0, 0);
        //new Building("Wall", 0,0, "Used for keeping enemies from advancing. Has a lot of Health.",3, "Wall", 200, 600, 0,0 ,0);
        

        buildings.AddRange(Building.buildingCurrent);

//        foreach (var t in buildings)
//            Debug.Log($"{t.name} - Damage: {t.damage} - FireRate: {t.fireRate}");
        
        gameControlScript.UpdateContainer();
    }
    void Update()
    {
        if(Keyboard.current.iKey.wasPressedThisFrame)
        {
            foreach (var t in buildings)
            Debug.Log($"{t.name} - Damage: {t.damage} - FireRate: {t.fireRate}");
        }
    }
    public Building FindBuildingClassById(int targetId)
    {
        foreach(Building building in buildings)
            if(building.id == targetId) return building;
        return null;
    }
    
}
