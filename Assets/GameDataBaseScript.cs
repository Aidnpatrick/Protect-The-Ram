using UnityEngine;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;

public class Turret
{
    public string name = "";
    public float fireRate = 0;
    public float damage = 0;
    public string information = "";
    public int id = 0;
    public string image = "";
    public int cost = 0;
    public static List<Turret> turretsCurrent = new List<Turret>();
    public Turret(string n, float f, float d, string i, int id, string image, int c)
    {
        turretsCurrent.Add(this);
        name = n;
        fireRate = f;
        damage = d;
        information = i;
        this.id = id;
        this.image = image;
        cost = c;
    }
};

public class Building
{
    
}

public class GameDataBaseScript : MonoBehaviour
{
    public GameControlScript gameControlScript;
    public GameObject selectionContainer;
    public Turret[] turrets;
    void Start()
    {
        Turret.turretsCurrent.Clear(); // prevent duplicates

        new Turret("BasicTurret",1,25,"Shoots medium ammo. Versatile. (SINGLE TARGET)", 0, "BasicTurret", 50);
        new Turret("HeavyTurret",4f,150,"Shoots big ammo. Slow but does big damage. (SINGLE TARGET)", 1, "HeavyTurret", 70);

        turrets = Turret.turretsCurrent.ToArray();

        foreach (var t in turrets)
            Debug.Log($"{t.name} - Damage: {t.damage} - FireRate: {t.fireRate}");
        
        gameControlScript.UpdateContainer();
    }
}
