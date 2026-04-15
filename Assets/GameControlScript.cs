using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameControlScript : MonoBehaviour
{

    public GameDataBaseScript gameDataBaseScript;
    public CameraScript cameraScript;

    public GameObject settingCanvas, gameCanvas;
    public GameObject selectionContainer, notificationContainer;
    public GameObject roundButton;
    public TMP_Text selectionText, gameStatText;
    public GameObject scrollSelectionContainer, informationText, controlTroopText, settingCanvasMainText, settingCanvasStatsText, hitUIButton, ToggleUIButton;


    //prefab
    public GameObject selectionContainerPrefab;
    public GameObject tilePrefab, enemyPrefab, ramPrefab, weedPrefab, gunPrefab, damageNotificationPrefab;
    public GameObject ReloadSmokePrefab, firingPrefab;
    public GameObject notificationPrefab;


    //libraries
    public GameObject[] enemies, turrets, troops, tiles, rams, weeds;
    public int amountOfMines = 0;
    public List<int> depositLocations = new List<int>();

    public int currentSelectionId = 0;
    public bool isRoundDone = true;
    public int money = 0, numberOfRounds = 0, amountOfLand = 30, moneyMadeInRound = 0, score = 0;
    public bool isTABactive = true, isSettingsActive = false, isGameCanvasActive = false, isDamageNotificationActive = true, isGameOver =false, isMusicActive = false;


    void Start()
    {
        score = 0;
        isSettingsActive = false;
        isTABactive = true;
        isGameCanvasActive = true;
        money = 500;
        amountOfLand = 100;
        numberOfRounds = 0;
        isRoundDone = true;
        moneyMadeInRound = 0;
        currentSelectionId = -1;
        isGameOver = false;

        isDamageNotificationActive = false;

        amountOfMines = 0;

        notificationContainer.SetActive(true);
        settingCanvasStatsText.SetActive(false);
        settingCanvasMainText.GetComponent<TMP_Text>().text = "Settings"; 

        NotificationText("Game Loaded!");

        depositLocations.Clear();

        depositLocations.Add(496);

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
                if(depositLocations.Contains(index) && tileClone.transform.childCount == 0) 
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

        for(int i = 0; i < 70; i++)
        {
            Instantiate(weedPrefab, FindTile(Random.Range(0,500)).transform.position, Quaternion.identity);
        }
        //StartCoroutine(e());
    }

    IEnumerator e()
    {
        while (true)
        {   
            SpawnEnemy(RandomPos());
            yield return new WaitForSeconds(5f);

        }
    }
    void Update()
    {
            
        gameCanvas.GetComponent<Canvas>().enabled = isGameCanvasActive;

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

        settingCanvas.SetActive(isSettingsActive);

        if(keyboard.escapeKey.wasPressedThisFrame && !isGameOver)
            isSettingsActive = !isSettingsActive;

        if(keyboard.lKey.wasPressedThisFrame)
            isGameCanvasActive = !isGameCanvasActive;
    }

    public GameObject SpawnEnemy(Vector3 location)
    {
        GameObject enemyClone = Instantiate(enemyPrefab, location, Quaternion.identity);
        if(Random.Range(0,10) <= 100f)
        {
            GameObject gunClone = Instantiate(gunPrefab, enemyClone.transform.position, Quaternion.identity, enemyClone.transform);
            gunClone.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/EnemyGun");
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
        rams = GameObject.FindGameObjectsWithTag("Ram");
        weeds = GameObject.FindGameObjectsWithTag("Weed");
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
        budget *= numberOfRounds / 17f + 1;



        while (budget > 0)
        {
            if(budget >= 7 && Random.value < 0.1f && numberOfRounds > 2)
            {
                GameObject big = SpawnEnemy(RandomPos());
                big.name = "EnemyBig";
                big.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/EnemyBig");
                budget -= 10;
            }
            else if (budget >= 9 && Random.value < 0.08f && numberOfRounds > 2)
            {
                GameObject cyberTruck = SpawnEnemy(RandomPos());
                cyberTruck.name = "CyberTruck";
                cyberTruck.GetComponent<SpriteRenderer>().sprite =
                    Resources.Load<Sprite>("Images/CyberTruck");
                budget -= 9;
            }
            else if(budget >= 9f && Random.value < 0.10f && numberOfRounds > 5)
            {
                GameObject spawner = SpawnEnemy(RandomPos());
                spawner.name = "EnemySpawner";
                spawner.GetComponent<SpriteRenderer>().sprite =
                Resources.Load<Sprite>("Images/EnemySpawner");
                budget -= 9f;
            }
            else if(budget >= 4.5f && Random.value < 0.10f && numberOfRounds > 1)
            {
                GameObject shield = SpawnEnemy(RandomPos());
                shield.name = "EnemyShield";
                shield.GetComponent<SpriteRenderer>().sprite =
                Resources.Load<Sprite>("Images/EnemyShield");
                budget -= 4.5f;
            }
            else
            {
                SpawnEnemy(RandomPos());
                budget -= 1;
            }
        }

        NotificationText("Round " + numberOfRounds + "\nEnemies coming!");
        isRoundDone = false;
    }
    

    public void EndRound(bool isPassed = false)
    {

        amountOfLand = Mathf.Clamp(amountOfLand + 10, 0, 300);
        isRoundDone = true;
        NotificationText("Round Completed!\n+ $" + moneyMadeInRound);
        money += amountOfMines * 50;
        if(amountOfMines > 0)
            NotificationText("Money produced from Gold Mines: + $" + amountOfMines * 90);
        money += 50;
        NotificationText("Bonus: + $50");
        score += moneyMadeInRound;

        roundButton.SetActive(true);
        foreach(GameObject enemy in enemies)
            Destroy(enemy);

        foreach(GameObject building in turrets)
        {
            BuildingScript bs = building.GetComponent<BuildingScript>();
            if(building.name.Contains("Healer"))
            {
                bs.HealTurrets();
            }
            bs.health = Mathf.Clamp(bs.health + 35, 0, bs.building.health);
            if(bs.typeOfBuilding == 1) bs.ammo = bs.building.ammo;
        }

        foreach(GameObject troop in troops)
            Destroy(troop);
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
    public 
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
    public void ToggleDamageNotification()
    {
        isDamageNotificationActive = !isDamageNotificationActive;
    }
    public void DamageNotification(float damage, Vector3 position)
    {
        if(!isDamageNotificationActive) return;
        GameObject damageNofiticationClone = Instantiate(damageNotificationPrefab, position + new Vector3(Random.Range(-0.5f,0.6f),Random.Range(-0.5f,0.6f),0), Quaternion.identity);
        damageNofiticationClone.GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        damageNofiticationClone.GetComponentInChildren<TMP_Text>().text = (damage).ToString();
        Destroy(damageNofiticationClone, 0.5f);
    }

    public void ToggleMusic()
    {
        isMusicActive = !isMusicActive;
    }
    public void ToggleUI()
    {
        isGameCanvasActive = !isGameCanvasActive;
    }

    public void GameOver()
    {
        isGameOver = true;
        isSettingsActive = true;
        isGameCanvasActive = false;
        settingCanvasMainText.GetComponent<TMP_Text>().text = "Game Over!";
        settingCanvasStatsText.SetActive(true);
        settingCanvasStatsText.transform.GetChild(1).GetComponent<TMP_Text>().text = "Score: " + score + "\nRounds Defended: " + numberOfRounds;
    }

}
