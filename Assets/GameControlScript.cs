using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameControlScript : MonoBehaviour
{

    public GameDataBaseScript gameDataBaseScript;
    public CameraScript cameraScript;
    public GameObject selectionContainer;
    public GameObject roundButton;
    //prefab
    public GameObject selectionContainerPrefab;
    public GameObject tilePrefab, enemyPrefab, ramPrefab;
    //libraries
    public GameObject[] enemies, turrets, troops, tiles;
    public Dictionary<GameObject, float> ramTracking = new Dictionary<GameObject, float>();
    public int currentSelectionId = 0;
    public bool isRoundDone = true;
    public int money = 0;


    void Start()
    {
        money = 0;
        isRoundDone = true;
        currentSelectionId = -1;
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
        ramTracking.Add(cameraScript.BuildOnTileMisc(ramPrefab, 495), 100);
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
            SpawnEnemy(new Vector3(Random.Range(1,11), Random.Range(1,11), 1));
    }
    public void SpawnEnemy(Vector3 location)
    {
        GameObject enemyClone = Instantiate(enemyPrefab, location, Quaternion.identity);
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

    public void UpdateArrays()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        turrets = GameObject.FindGameObjectsWithTag("Buildings");
        troops = GameObject.FindGameObjectsWithTag("Troop");
        tiles = GameObject.FindGameObjectsWithTag("Tile");
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

    public void StartRound()
    {
        roundButton.SetActive(false);
        for(int i = 0; i < 10; i++)
            SpawnEnemy(new Vector3(Random.Range(1,11), Random.Range(1,11), 1));
        isRoundDone = false;
    }
    public void EndRound(bool isPassed = false)
    {
        isRoundDone = true;

        roundButton.SetActive(true);
        foreach(GameObject enemy in enemies)
            Destroy(enemy);
    }
}
