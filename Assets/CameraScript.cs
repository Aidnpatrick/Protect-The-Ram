using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;

public class CameraScript : MonoBehaviour
{
    public GameControlScript gameControlScript;
    public GameDataBaseScript gameDataBaseScript;
    public Canvas gameCanvas;
    public GameObject player;
    public GameObject informationBox;
    public bool isInformationBoxActive = false;
    private Vector2 moveInput;
    public Collider2D hitMain = null;
    public bool canEdit = true;

    //prefabs
    public GameObject buildingPrefab;
    void Start()
    {
        canEdit = true;
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
        GameObject button1 = boxTransform.GetChild(1).gameObject;
        GameObject button2 = boxTransform.GetChild(2).gameObject;

        infoText.text = $"{building.name}" + $"\nDamage: {building.damage}" + $"\nFireRate: {building.fireRate}" + $"\nInformation: {building.information}";

        button1.SetActive(false);
        button2.SetActive(false);
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
        Button informationButton = informationBox.transform.GetChild(1).GetComponent<Button>();
        Button informationDestroyButton = informationBox.transform.GetChild(2).GetComponent<Button>();

        GameObject actualTarget = targetHit;

        if (targetHit.transform.parent == null && targetHit.transform.childCount > 0)
        {
            actualTarget = targetHit.transform.GetChild(0).gameObject;
        }

        if (actualTarget == null || actualTarget.GetComponent<BuildingScript>() == null)
        {
            informationText.text = "Empty";
            informationDestroyButton.gameObject.SetActive(true);
            informationButton.gameObject.SetActive(false);
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

            informationButton.gameObject.SetActive(actualTarget.name.Contains("Camp"));
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

        transform.position = player.transform.position + new Vector3(0,0,-5);

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);
        informationBox.SetActive(isInformationBoxActive);

        if(!canEdit) return; 

        if(hit != null && hit.transform.parent != null && hit.transform.parent.name.Contains("Tile") && keyboard.xKey.wasPressedThisFrame && !hit.transform.name.Contains("Ram"))
        {
            Destroy(hit.gameObject);
            return;
        }

        if (Input.GetMouseButtonDown(1) && hit.transform.parent != null)
        {
            OpenInformationBox(hit.gameObject);
            return;
        }

        if(hit == null || !hit.gameObject.name.Contains("Tile")) return;
        
        if (Input.GetMouseButtonDown(1))
        {
            OpenInformationBox(hit.gameObject);
            return;
        }

        if(Input.GetMouseButtonDown(0) && hit.transform.childCount == 0 && gameControlScript.currentSelectionId != -1 && !isInformationBoxActive)
        {
            BuildingBuildOnTile(gameControlScript.currentSelectionId, hit.gameObject);
            gameControlScript.currentSelectionId = -1;
        }


        if(hit.transform.childCount > 0 && keyboard.xKey.wasPressedThisFrame && !hit.transform.GetChild(0).name.Contains("Ram"))
            Destroy(hit.transform.GetChild(0).gameObject);
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
