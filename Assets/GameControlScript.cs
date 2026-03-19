using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameControlScript : MonoBehaviour
{

    public GameDataBaseScript gameDataBaseScript;
    public GameObject tilePrefab, enemyPrefab;
    public GameObject selectionContainer;
    //prefab
    public GameObject selectionContainerPrefab;
    //libraries
    public GameObject[] enemies, turrets;
    
    public int currentSelectionId = 0;


    void Start()
    {
        for(int i = 0; i < 10; i++)
        {
            SpawnEnemy(new Vector3(Random.Range(1,11), Random.Range(1,11), 1));
        }
        currentSelectionId = -1;
        for(int i = 0; i < 10; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                GameObject tileClone = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity);
            }
        }
        foreach(Building turretsIndex in gameDataBaseScript.buildings)
        {
            GameObject selectionContainerClone = Instantiate(selectionContainerPrefab, selectionContainer.transform);
            selectionContainerClone.transform.GetChild(0).GetComponent<TMP_Text>().text = turretsIndex.name;
        }
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        turrets = GameObject.FindGameObjectsWithTag("Buildings");
        if(keyboard.jKey.wasPressedThisFrame)
        {
            SpawnEnemy(new Vector3(Random.Range(1,11), Random.Range(1,11), 1));

        }
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


    public GameObject FindNearestObject(GameObject[] targets, float lineOfSight)
    {
        float closestDistance = Mathf.Infinity;
        if (targets.Length == 0)
            return null;
        GameObject closestEnemy = targets[0];
        foreach(GameObject enemy in targets)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if(distance < closestDistance && distance < lineOfSight)
            {
                closestEnemy = enemy;
                closestDistance = distance;
            }
        }
        return closestEnemy;
    }
}
