using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameControlScript : MonoBehaviour
{

    public GameDataBaseScript gameDataBaseScript;
    public GameObject tilePrefab;
    public GameObject selectionContainer;
    //prefab
    public GameObject selectionContainerPrefab;
    
    public int currentSelectionId = 0;
    void Start()
    {
        
        for(int i = 0; i < 10; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                GameObject tileClone = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity);
            }
        }
        foreach(Turret turretsIndex in gameDataBaseScript.turrets)
        {
            GameObject selectionContainerClone = Instantiate(selectionContainerPrefab, selectionContainer.transform);
            selectionContainerClone.transform.GetChild(0).GetComponent<TMP_Text>().text = turretsIndex.name;
        }
    }
    public void UpdateContainer()
    {
        foreach(Turret turretsIndex in gameDataBaseScript.turrets)
        {
            GameObject selectionContainerClone = Instantiate(selectionContainerPrefab, selectionContainer.transform);
            selectionContainerClone.transform.GetChild(0).GetComponent<TMP_Text>().text = turretsIndex.name;
            int temp = turretsIndex.id;
            selectionContainerClone.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => currentSelectionId = temp);

        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
