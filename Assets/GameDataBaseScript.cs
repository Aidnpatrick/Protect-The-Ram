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
    public static List<Building> buildingCurrent = new List<Building>();
    public Building(string n, float f, float d, string i, int id, string image, int c, int h, float r)
    {
        buildingCurrent.Add(this);
        name = n;
        fireRate = f;
        damage = d;
        information = i;
        this.id = id;
        this.image = image;
        cost = c;
        health = h;
        range = r;
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
    }
};

public class GameDataBaseScript : MonoBehaviour
{
    public GameControlScript gameControlScript;
    public GameObject selectionContainer;
    public List<Building> buildings = new List<Building>();
    void Start()
    {
        Building.buildingCurrent.Clear(); // prevent duplicates

        new Building("BasicTurret",1,25,"Shoots medium ammo. Versatile. (SINGLE TARGET)", 0, "BasicTurret", 50, 300, 10);
        new Building("HeavyTurret",4f,150,"Shoots big ammo. Slow but does big damage. (SINGLE TARGET)", 1, "HeavyTurret", 70, 450, 20);
        buildings.AddRange(Building.buildingCurrent);

        foreach (var t in buildings)
            Debug.Log($"{t.name} - Damage: {t.damage} - FireRate: {t.fireRate}");
        
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
