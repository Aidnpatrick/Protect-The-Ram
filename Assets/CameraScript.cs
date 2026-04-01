using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
public class CameraScript : MonoBehaviour
{
    public GameControlScript gameControlScript;
    public GameDataBaseScript gameDataBaseScript;
    public Canvas gameCanvas;
    public GameObject player;
    public GameObject informationBox;

    public GameObject notificationContainer;
    public GameObject notificationPrefab;
    public bool isInformationBoxActive = false;
    private Vector2 moveInput;
    public Collider2D hitMain = null;
    public bool canEdit = true, isControllingTroops = false;

    //prefabs
    public GameObject buildingPrefab;

    public GameObject targetMain;
    void Start()
    {
        canEdit = true;
        isControllingTroops = false;
        isInformationBoxActive = false;
    }
    void UpdateInformationBoxSelection(Building building, Vector2 screenPosition)
    {
        RectTransform canvasRect = gameCanvas.GetComponent<RectTransform>();
        RectTransform infoRect = informationBox.GetComponent<RectTransform>();

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            gameCanvas.worldCamera,
            out pos
        );

        infoRect.anchoredPosition = pos + new Vector2(100,100);

        Transform boxTransform = informationBox.transform;
        TMP_Text infoText = boxTransform.GetChild(0).GetComponent<TMP_Text>();
        GameObject destroyButton = boxTransform.GetChild(1).gameObject;

        infoText.text = $"{building.name}" + $"\nDamage: {building.damage}" + $"\nCost: {building.cost}" + $"\nInformation: {building.information}";

        destroyButton.SetActive(false);
    }
    void OpenInformationBox(GameObject targetHit)
    {
        if (targetHit.name.Contains("Weed") || targetHit.name.Contains("Ram")) return;

        RectTransform canvasRect = informationBox.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            gameCanvas.worldCamera,
            out pos
        );

        TMP_Text informationText = informationBox.transform.GetChild(0).GetComponent<TMP_Text>();
        Button informationDestroyButton = informationBox.transform.GetChild(1).GetComponent<Button>();

        GameObject actualTarget = targetHit;

        if (targetHit.transform.parent == null && targetHit.transform.childCount > 0)
        {
            actualTarget = targetHit.transform.GetChild(0).gameObject;
        }

        if (actualTarget == null || actualTarget.GetComponent<BuildingScript>() == null)
        {
            informationText.text = "Empty";
            informationDestroyButton.gameObject.SetActive(true);
        }
        else
        {
            BuildingScript building = actualTarget.GetComponent<BuildingScript>();
            hitMain = actualTarget.GetComponent<Collider2D>();

            informationDestroyButton.gameObject.SetActive(true);

            informationText.text = actualTarget.name;
            informationText.text += "\n" + building.health + " HP\n";

            if (actualTarget.name.Contains("Turret"))
                informationText.text += "\n" + building.ammo + " Ammo\n";
        }

        informationBox.GetComponent<RectTransform>().anchoredPosition = pos + new Vector2(200, 100);

        isInformationBoxActive = !isInformationBoxActive;
    }
public void MakeMoreTroops()
    {
        BuildingScript bs = hitMain.GetComponent<BuildingScript>();
        bs.numOfTroops = Mathf.Clamp(bs.numOfTroops+1,0,5);
    }
    
public void DestroyBuilding()
{
    if (hitMain != null && !hitMain.name.Contains("Ram"))
    {
        Destroy(hitMain.gameObject);
    }
}
    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        // controlling troops
        if(keyboard.rKey.wasPressedThisFrame)
        {
            isControllingTroops = !isControllingTroops;
        }

        // selection information box
        int index = 0, mouseTouched = 0;
        foreach(Transform selectionContainerIndex in gameControlScript.selectionContainer.transform)
        {
            Vector2 mousePosUI = Input.mousePosition;
            if (RectTransformUtility.RectangleContainsScreenPoint(
                selectionContainerIndex.GetComponent<Image>().rectTransform, 
                mousePosUI, 
                gameControlScript.selectionContainer.GetComponentInParent<Canvas>().worldCamera) && Input.GetMouseButtonDown(1))
            {
                UpdateInformationBoxSelection(gameDataBaseScript.buildings[index], selectionContainerIndex.GetComponent<RectTransform>().anchoredPosition);

                isInformationBoxActive = !isInformationBoxActive;
                return;
            }
            if(RectTransformUtility.RectangleContainsScreenPoint(
                selectionContainerIndex.GetComponent<Image>().rectTransform, 
                mousePosUI, 
                gameControlScript.selectionContainer.GetComponentInParent<Canvas>().worldCamera))
            {
                mouseTouched++;
            }
            index++;
        }
        if(mouseTouched > 0) canEdit = false;
        else canEdit = true;

        //camera 
        transform.position = player.transform.position + new Vector3(0,0,-5);


        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);
        informationBox.SetActive(isInformationBoxActive);

        if(!canEdit) return;
        // children on tile
        if(hit != null && hit.transform.parent != null && hit.transform.parent.name.Contains("Tile") && keyboard.xKey.wasPressedThisFrame && !hit.transform.name.Contains("Ram"))
        {
            Destroy(hit.gameObject);
            return;
        }

        if (Input.GetMouseButtonDown(1) && hit != null &&hit.transform.parent != null)
        {
            OpenInformationBox(hit.gameObject);
            return;
        }

        if(hit == null || !hit.gameObject.name.Contains("Tile")) return;

        // parent on tile

        TileScript tileScript = hit.GetComponent<TileScript>();

        if (Input.GetMouseButtonDown(1))
        {
            OpenInformationBox(hit.gameObject);
            return;
        }
        if(Input.GetMouseButtonDown(0) && hit.transform.childCount == 0 && gameControlScript.currentSelectionId != -1 && !isInformationBoxActive && !isControllingTroops)
        {
            if(500 - gameControlScript.amountOfLand < tileScript.id && gameControlScript.money >= gameDataBaseScript.FindBuildingClassById(gameControlScript.currentSelectionId).cost)
            {
                gameControlScript.money -= gameDataBaseScript.FindBuildingClassById(gameControlScript.currentSelectionId).cost;
                BuildingBuildOnTile(gameControlScript.currentSelectionId, hit.gameObject);
                gameControlScript.currentSelectionId = -1;
            }
            else if(500 - gameControlScript.amountOfLand >= tileScript.id)
            {
                gameControlScript.NotificationText("Can't place here!");
            }
            else if(gameControlScript.money < gameDataBaseScript.FindBuildingClassById(gameControlScript.currentSelectionId).cost)
            {
                gameControlScript.NotificationText("Invalid funds!");
                gameControlScript.currentSelectionId = -1;
            }
        }
        /*
        if(Input.GetMouseButtonDown(0) && hit.transform.childCount == 0 && gameControlScript.currentSelectionId != -1 && !isInformationBoxActive
        && 500 - gameControlScript.amountOfLand < tileScript.id && !isControllingTroops && gameControlScript.money >= gameDataBaseScript.FindBuildingClassById(gameControlScript.currentSelectionId).cost) 
        {
            gameControlScript.money -= gameDataBaseScript.FindBuildingClassById(gameControlScript.currentSelectionId).cost;
            BuildingBuildOnTile(gameControlScript.currentSelectionId, hit.gameObject);
            gameControlScript.currentSelectionId = -1;
        }
        else if(Input.GetMouseButtonDown(0) && 500 - gameControlScript.amountOfLand >= tileScript.id)
        {
            gameControlScript.NotificationText("Can't place here!");
        }
        else if(gameControlScript.money >= gameDataBaseScript.FindBuildingClassById(gameControlScript.currentSelectionId).cost)
        {
        }
        */

        if(hit.transform.childCount > 0 && keyboard.xKey.wasPressedThisFrame && !hit.transform.GetChild(0).name.Contains("Ram"))
            Destroy(hit.transform.GetChild(0).gameObject);
        
        if(isControllingTroops && Input.GetMouseButtonDown(0))
        {
            gameControlScript.currentSelectionId = -1;
            targetMain = hit.gameObject;
            ControlTroopTarget();
        }
    }
    
    void ControlTroopTarget()
    {
        foreach(GameObject troop in gameControlScript.troops)
            troop.GetComponent<EnemyScript>().isControlled = true;
    }

    public void BuildingBuildOnTile(int targetId, GameObject targetTile)
    {
        Building targetBuilding = gameDataBaseScript.FindBuildingClassById(targetId);
        
        GameObject buildingClone = Instantiate(buildingPrefab, targetTile.transform.position, Quaternion.identity);
        buildingClone.name = targetBuilding.name;
        buildingClone.transform.parent = targetTile.transform;
        buildingClone.GetComponent<BuildingScript>().building = new Building(targetBuilding);
    }



}
