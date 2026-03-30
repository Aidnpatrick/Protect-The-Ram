using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameControlScript : MonoBehaviour
{

    public GameDataBaseScript gameDataBaseScript;
    public CameraScript cameraScript;
    public GameObject selectionContainer, notificationContainer;
    public GameObject roundButton;
    public TMP_Text selectionText;
    public GameObject scrollSelectionContainer, informationText;
    //prefab
    public GameObject selectionContainerPrefab;
    public GameObject tilePrefab, enemyPrefab, ramPrefab, weedPrefab, gunPrefab;
    public GameObject ReloadSmokePrefab, firingPrefab;
    //libraries
    public GameObject[] enemies, turrets, troops, tiles;
    public Dictionary<GameObject, float> ramTracking = new Dictionary<GameObject, float>();
    public int currentSelectionId = 0;
    public bool isRoundDone = true;
    public int money = 0, numberOfRounds = 0, amountOfLand = 30;
    public bool isTABactive = true;


    void Start()
    {
        isTABactive = true;
        money = 0;
        amountOfLand = 100;
        numberOfRounds = 0;
        isRoundDone = true;
        currentSelectionId = -1;

        notificationContainer.SetActive(false);

        int index = 0;
        for(int i = 0; i < 50; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                GameObject tileClone = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity);
                tileClone.GetComponent<TileScript>().id = index;
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
        selectionText.text = "Currently Selecting:\n" + (currentSelectionId >= 0 ? gameDataBaseScript.buildings[currentSelectionId].name : "None");

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

    public void TileBlinkBoundary()
    {

        foreach(GameObject tile in tiles)
        {
            tile.GetComponent<TileScript>().BlinkBoundary();
        }
    }


    //GAMELOOP
    public void StartRound()
    {
        numberOfRounds++;
        roundButton.SetActive(false);
        for(int i = 0; i < 10; i++)
            SpawnEnemy(new Vector3(Random.Range(1,9), Random.Range(1,9), 1));

        GameObject cyberTruckClone = SpawnEnemy(new Vector3(Random.Range(1,9), Random.Range(1,9), 1));
        cyberTruckClone.name = "CyberTruck";
        cyberTruckClone.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/CyberTruck");
        
        
        notificationContainer.SetActive(true);
        notificationContainer.GetComponentInChildren<TMP_Text>().text = "Round " + numberOfRounds + "\nEnemies come from left!";
        StartCoroutine(NotificationText(notificationContainer));

        isRoundDone = false;
    }
    public void EndRound(bool isPassed = false)
    {
        amountOfLand += 10;
        isRoundDone = true;

        roundButton.SetActive(true);
        foreach(GameObject enemy in enemies)
            Destroy(enemy);
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

        return Instantiate(prefab, targetTile.transform.position, Quaternion.identity, targetTile.transform);
    }

    IEnumerator NotificationText(GameObject container)
    {
        yield return new WaitForSeconds(2.5f);
        container.SetActive(false);
    }
}
