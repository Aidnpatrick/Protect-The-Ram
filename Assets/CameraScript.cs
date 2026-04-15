using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;

public class CameraScript : MonoBehaviour
{
    public GameControlScript gameControlScript;
    public GameDataBaseScript gameDataBaseScript;
    public Camera camera;
    public Canvas gameCanvas;
    public GameObject player, crossChair;
    public GameObject informationBox, selectionInformationBox;

    public GameObject notificationContainer;
    public GameObject notificationPrefab;
    public bool isInformationBoxActive = false, isSelectionInformationBoxActive;
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

        camera.orthographicSize = 5;
    }
    void UpdateInformationBoxSelection(Building building, Vector2 screenPosition)
    {
        RectTransform canvasRect = gameCanvas.GetComponent<RectTransform>();
        RectTransform infoRect = selectionInformationBox.GetComponent<RectTransform>();

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            gameCanvas.worldCamera,
            out pos
        );

        infoRect.anchoredPosition = pos + new Vector2(100,100);

        Transform boxTransform = selectionInformationBox.transform;

        TMP_Text infoText = boxTransform.GetChild(0).GetComponent<TMP_Text>();

        infoText.text = $"{building.name}" + $"\nDamage: {building.damage}" + $"\nCost: {building.cost}" + $"\nInformation: {building.information}";

    }
    void OpenInformationBox(GameObject targetHit)
    {

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
            actualTarget = targetHit.transform.GetChild(0).gameObject;

        if (actualTarget == null || actualTarget.GetComponent<BuildingScript>() == null)
        {
            if (actualTarget.GetComponent<TileScript>() == null)
            {
                informationText.text = actualTarget.name.Replace("(Clone)", "");
                if(actualTarget.name.Contains("Ram"))
                    informationText.text += "\n" + actualTarget.GetComponent<RamScript>().health  + "HP\n";

                hitMain = actualTarget.GetComponent<Collider2D>();
                informationDestroyButton.gameObject.SetActive(true);
            }
            else
            {
                if(targetHit.GetComponent<TileScript>().oreDeposit)
                informationText.text = "Empty Ore Deposit";
                else informationText.text = "Emtpy Tile";
                informationDestroyButton.gameObject.SetActive(false);
            }
        }
        else
        {
            
            informationText.text = actualTarget.name;
            hitMain = actualTarget.GetComponent<Collider2D>();

            if(actualTarget.GetComponent<BuildingScript>() != null) 
            {
                BuildingScript building = actualTarget.GetComponent<BuildingScript>();
                hitMain = actualTarget.GetComponent<Collider2D>();

                informationDestroyButton.gameObject.SetActive(true);

                informationText.text += "\n" + building.health + " HP\n";

                if (actualTarget.name.Contains("Turret"))
                    informationText.text += "\n" + building.ammo + " Ammo\n";
            }
            
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
            if(hitMain.tag == "Building")
            {
                gameControlScript.money += gameDataBaseScript.FindBuildingClassById(hitMain.GetComponent<BuildingScript>().building.id).cost / 2;
            }
            Destroy(hitMain.gameObject);
        }
        isInformationBoxActive = false;
    }
    void Update()
    {

        Vector2 mousePosUI = Input.mousePosition;
        Keyboard keyboard = Keyboard.current;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if ((keyboard.tKey.isPressed || scroll > 0) && camera.orthographicSize >= 1f)
            camera.orthographicSize -= 0.05f;

        if((keyboard.yKey.isPressed || scroll < 0 ) && camera.orthographicSize <= 10)
            camera.orthographicSize += 0.05f;

        if(keyboard.gKey.isPressed)
            camera.orthographicSize = 5;

        // controlling troops
        if(keyboard.rKey.wasPressedThisFrame)
        {
            isControllingTroops = !isControllingTroops;
        }

        // selection information box
        int index = 0, mouseTouched = 0;
        foreach(Transform selectionContainerIndex in gameControlScript.selectionContainer.transform)
        {
            
            if (RectTransformUtility.RectangleContainsScreenPoint(
                selectionContainerIndex.GetComponent<Image>().rectTransform, 
                mousePosUI, 
                gameControlScript.selectionContainer.GetComponentInParent<Canvas>().worldCamera))
            {
                UpdateInformationBoxSelection(gameDataBaseScript.buildings[index], selectionContainerIndex.GetComponent<RectTransform>().anchoredPosition);

                isSelectionInformationBoxActive = true;
                break;
            }
            else
            {
                mouseTouched++;
                isSelectionInformationBoxActive = false;
            }
            index++;
        }
        
        if(mouseTouched == gameDataBaseScript.buildings.Count) canEdit = true;
        else canEdit = false;


        //camera 
        transform.position = player.transform.position + new Vector3(0,0,-5);


        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);
        informationBox.SetActive(isInformationBoxActive);
        selectionInformationBox.SetActive(isSelectionInformationBoxActive);
        

        if(!canEdit) return;

        if(hit != null && ((hit.transform.parent != null && hit.transform.parent.name.Contains("Tile")) || hit.name.Contains("Tile")))
        {
            crossChair.transform.position = hit.transform.position;
            crossChair.SetActive(true);
        }
        else
            crossChair.SetActive(false);

        // children on tile
        if(hit != null && hit.transform.parent != null && hit.transform.parent.name.Contains("Tile") && keyboard.xKey.wasPressedThisFrame && !hit.transform.name.Contains("Ram"))
        {
            Destroy(hit.gameObject);
            return;
        }

        if (Input.GetMouseButtonDown(1) && hit != null && hit.transform.parent != null)
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
        bool isInformationBoxHovered = RectTransformUtility.RectangleContainsScreenPoint(
                informationBox.GetComponent<Image>().rectTransform, 
                mousePosUI, 
                gameControlScript.selectionContainer.GetComponentInParent<Canvas>().worldCamera);

        if(Input.GetMouseButtonDown(0) && hit.transform.childCount == 0 && gameControlScript.currentSelectionId != -1 && (isInformationBoxActive && !isInformationBoxHovered||!isInformationBoxActive) && !isControllingTroops)
        {
            if(gameDataBaseScript.buildings[gameControlScript.currentSelectionId].name.Contains("Gold") && !hit.GetComponent<TileScript>().oreDeposit)
            {
                gameControlScript.NotificationText("Can't place Gold Digger here! Place it on ore deposits.");
                return;
            }
            if(500 - gameControlScript.amountOfLand < tileScript.id && gameControlScript.money >= gameDataBaseScript.FindBuildingClassById(gameControlScript.currentSelectionId).cost)
            {
                gameControlScript.money -= gameDataBaseScript.FindBuildingClassById(gameControlScript.currentSelectionId).cost;
                BuildingBuildOnTile(gameControlScript.currentSelectionId, hit.gameObject);
                gameControlScript.currentSelectionId = -1;
            }
            else if(500 - gameControlScript.amountOfLand >= tileScript.id)
                gameControlScript.NotificationText("Can't place here!");

            else if(gameControlScript.money < gameDataBaseScript.FindBuildingClassById(gameControlScript.currentSelectionId).cost)
            {
                gameControlScript.NotificationText("Invalid funds!");
                gameControlScript.currentSelectionId = -1;
            }
            if(isInformationBoxActive)
                isInformationBoxActive = false;
            foreach(GameObject i in gameControlScript.weeds)
            {
                if(i.transform.position == hit.transform.position) {
                    Destroy(i);
                    break;
                }
            }
        }

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
        {
            if(troop != null) troop.GetComponent<EnemyScript>().isControlled = true;
        }

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
