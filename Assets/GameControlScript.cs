
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameControlScript : MonoBehaviour
{

    public GameDataBaseScript gameDataBaseScript;
    public CameraScript cameraScript;


    public GameObject selectionContainer, notificationContainer;
    public GameObject roundButton;
    public TMP_Text selectionText, gameStatText;
    public GameObject scrollSelectionContainer, informationText, controlTroopText;


    //prefab
    public GameObject selectionContainerPrefab;
    public GameObject tilePrefab, enemyPrefab, ramPrefab, weedPrefab, gunPrefab;
    public GameObject ReloadSmokePrefab, firingPrefab;
    public GameObject notificationPrefab;


    //libraries
    public GameObject[] enemies, turrets, troops, tiles;
    public int amountOfMines = 0;
    public List<int> depositLocations = new List<int>();
    public Dictionary<GameObject, float> ramTracking = new Dictionary<GameObject, float>();


    public int currentSelectionId = 0;
    public bool isRoundDone = true;
    public int money = 0, numberOfRounds = 0, amountOfLand = 30, moneyMadeInRound = 0;
    public bool isTABactive = true;


    void Start()
    {
        isTABactive = true;
        money = 500;
        amountOfLand = 100;
        numberOfRounds = 0;
        isRoundDone = true;
        moneyMadeInRound = 0;
        currentSelectionId = -1;

        amountOfMines = 0;

        notificationContainer.SetActive(true);
        for(int i = 0; i < 10; i++)
        {
            int randomLocation = Random.Range(100,500);
            if(!depositLocations.Contains(randomLocation)) depositLocations.Add(randomLocation);
        }

        int index = 0;
        for(int i = 0; i < 50; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                GameObject tileClone = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity);
                tileClone.GetComponent<TileScript>().id = index;
                if(depositLocations.Contains(index)) 
                {
                    tileClone.name += " Deposit";
                    tileClone.GetComponent<TileScript>().oreDeposit = true;
                    tileClone.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/DepositTile");
                }
                index++;
            }
        }

        foreach(Building turretsIndex in gameDataBaseScript.buildings)
        {
            GameObject selectionContainerClone = Instantiate(selectionContainerPrefab, selectionContainer.transform);
            selectionContainerClone.transform.GetChild(0).GetComponent<TMP_Text>().text = turretsIndex.name;
        }

        UpdateArrays();
        GameObject ram = BuildOnTileMisc(ramPrefab, 495);

        if (ram != null)
        {
            ramTracking.Add(ram, 100);
        }
        SpawnInGroups(weedPrefab, 70);
        //StartCoroutine(e());
    }

    IEnumerator e()
    {
        SpawnEnemy(RandomPos());
        yield return new WaitForSeconds(1f);
    }
    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        UpdateArrays();
    
        if(!isRoundDone && enemies.Length < 1)
        {
            EndRound(true);
        }

        if(keyboard.jKey.wasPressedThisFrame)
            SpawnEnemy(new Vector3(Random.Range(1f,9f), Random.Range(1f,9f), 1));

        if(keyboard.tabKey.wasReleasedThisFrame)
        {
            isTABactive = !isTABactive;
        }
        if(cameraScript.isControllingTroops)
            selectionText.text = "Currently Controlling Troops!";
        else
            selectionText.text = "Currently Selecting:\n" + (currentSelectionId >= 0 ? gameDataBaseScript.buildings[currentSelectionId].name : "None");

        gameStatText.text = $"Round {numberOfRounds}\n${money}";

        controlTroopText.SetActive(cameraScript.isControllingTroops);
        informationText.SetActive(isTABactive);
    }

    public GameObject SpawnEnemy(Vector3 location)
    {
        GameObject enemyClone = Instantiate(enemyPrefab, location, Quaternion.identity);
        if(Random.Range(0,10) <= 100f)
        {
            GameObject gunClone = Instantiate(gunPrefab, enemyClone.transform.position, Quaternion.identity, enemyClone.transform);
            gunClone.GetComponent<SpriteRenderer>().flipY = false;
        }
        return enemyClone;
    }
    public void UpdateContainer()
    {
        foreach(Building turretsIndex in gameDataBaseScript.buildings)
        {
            GameObject selectionContainerClone = Instantiate(selectionContainerPrefab, selectionContainer.transform);
            selectionContainerClone.transform.GetChild(0).GetComponent<TMP_Text>().text = turretsIndex.name;
            int temp = turretsIndex.id;
            selectionContainerClone.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => CheckSelectionAvailability(temp));
        }
    }
    
    private void CheckSelectionAvailability(int targetId)
    {
        if(currentSelectionId == targetId) currentSelectionId = -1;
        else currentSelectionId = targetId;
    }

    // Update is called once per frame


    public GameObject FindNearestObject(GameObject[] targets, float lineOfSight, GameObject origin)
    {
        float closestDistance = Mathf.Infinity;
        if (targets.Length == 0)
            return null;
        GameObject closestEnemy = null;
        foreach(GameObject enemy in targets)
        {
            float distance = Vector3.Distance(origin.transform.position, enemy.transform.position);
            if(distance < closestDistance && distance < lineOfSight)
            {
                closestEnemy = enemy;
                closestDistance = distance;
            }
        }
        return closestEnemy;
    }

    public GameObject FindTile(int tileId)
    {
        foreach(GameObject tileIndex in tiles)
            if(tileIndex.GetComponent<TileScript>().id == tileId) return tileIndex;
        return null;
    }
    
    public GameObject FindNearestObjectDictionary(
    Dictionary<GameObject, float> targets,
    float lineOfSight,
    GameObject origin)
    {
        float closestDistance = Mathf.Infinity;
        GameObject closest = null;

        foreach (var kvp in targets)
        {
            GameObject obj = kvp.Key;

            if (obj == null) continue;

            float distance = Vector3.Distance( origin.transform.position, obj.transform.position
            );

            if (distance < closestDistance && distance < lineOfSight)
            {
                closestDistance = distance;
                closest = obj;
            }
        }
        return closest;
    }

    public void UpdateArrays()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        turrets = GameObject.FindGameObjectsWithTag("Buildings");
        troops = GameObject.FindGameObjectsWithTag("Troop");
        tiles = GameObject.FindGameObjectsWithTag("Tile");
    }
    public void SpawnInGroups(GameObject child, int numberOfChilds)
    {
        for(int i = 0; i < numberOfChilds; i++) {
            GameObject clone = BuildOnTileMisc(child, Random.Range(0, tiles.Length+1));
        }
    }



    //GAMELOOP
    public void StartRound()
    {
        numberOfRounds++;
        roundButton.SetActive(false);
        moneyMadeInRound = 0;

        float budget = 10 + numberOfRounds * 5;
        if(numberOfRounds > 10) budget *= 1.5f;
        if(numberOfRounds > 20) budget *= 3f;

        while (budget > 0)
        {
            if (budget >= 5 && Random.value < 0.10f)

            {
                GameObject cyberTruck = SpawnEnemy(RandomPos());
                cyberTruck.name = "CyberTruck";
                cyberTruck.GetComponent<SpriteRenderer>().sprite =
                    Resources.Load<Sprite>("Images/CyberTruck");
                budget -= 5;
            }
            else
            {
                SpawnEnemy(RandomPos());
                budget -= 1;
            }
        }

        GameObject notificationClone = Instantiate(notificationPrefab, notificationContainer.transform);
        notificationClone.GetComponentInChildren<TMP_Text>().text =
            "Round " + numberOfRounds + "\nEnemies coming!";
        Destroy(notificationClone, 2.5f);


        isRoundDone = false;
    }
    

    public void EndRound(bool isPassed = false)
    {

        amountOfLand = Mathf.Clamp(amountOfLand + 10, 0, 300);
        isRoundDone = true;
        NotificationText("Round Completed!\n+ $" + moneyMadeInRound);
        money += amountOfMines * 50;
        NotificationText("Money produced from Gold Mines: + $" + amountOfMines * 50);
        money += 50;
        NotificationText("Bonus: + $50");

        roundButton.SetActive(true);
        foreach(GameObject enemy in enemies)
            Destroy(enemy);

        foreach(GameObject building in turrets)
        {
            BuildingScript bs = building.GetComponent<BuildingScript>();
            bs.health = Mathf.Clamp(bs.health + 35, 0, bs.building.health);
        }

        if(Random.Range(0,7) < 1f)
        {
            
        }
    }
    Vector3 RandomPos()
    {
        return new Vector3(Random.Range(1, 20), Random.Range(1, 9), 1);
    }
    public GameObject CreateParticle(GameObject particle, Vector3 location, float amountOfRotation, string imageName, bool isTrans = false)
    {
        GameObject particleClone = Instantiate(particle, location, Quaternion.identity);
        if(isTrans) particleClone.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Random.Range(0.35f,0.75f));
        particleClone.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/" + imageName);
        particleClone.transform.Rotate(0,0,Random.Range(-amountOfRotation, amountOfRotation+1));
        particleClone.GetComponent<Rigidbody2D>().linearVelocity = particleClone.transform.up * 3;
        Destroy(particleClone, 1.2f);
        return particleClone;
    }
    
    public GameObject BuildOnTileMisc(GameObject prefab, int tileId)
    {
        GameObject targetTile = FindTile(tileId);

        if (targetTile == null)
        {
            return null;
        }

        if (targetTile.transform.childCount > 0)
            return null;
        
        if(targetTile.name.Contains("Deposit"))
            return null;

        return Instantiate(prefab, targetTile.transform.position, Quaternion.identity, targetTile.transform);
    }

    public void NotificationText(string text)
    {
        GameObject notificationClone = Instantiate(notificationPrefab, notificationContainer.transform);
        notificationClone.GetComponentInChildren<TMP_Text>().text = text;
        Destroy(notificationClone, 2.5f);
    }
    
    void SpawnTroop(Vector3 location)
    {
        GameObject troopClone = Instantiate(enemyPrefab, location, Quaternion.identity);
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
}
