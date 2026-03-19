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

    //prefabs
    public GameObject buildingPrefab;
    void Start()
    {
        isInformationBoxActive = false;
    }
    void UpdateInformationBoxSelection(Building building, Vector2 screenPosition)
    {
        RectTransform canvasRect = gameCanvas.GetComponent<RectTransform>();
        RectTransform infoRect = informationBox.GetComponent<RectTransform>();

        Camera cam = gameCanvas.renderMode == RenderMode.ScreenSpaceOverlay 
            ? null 
            : gameCanvas.worldCamera;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            cam,
            out localPoint
        );

        infoRect.anchoredPosition = localPoint + new Vector2(100, 100);

        TMP_Text infoText = informationBox.transform.GetChild(0).GetComponent<TMP_Text>();
        infoText.text =
            building.name +
            "\nDamage: " + building.damage +
            "\nFireRate: " + building.fireRate +
            "\nInformation: " + building.information;
    }

    void OpenInformationBox(Collider2D targetHit)
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
        if(targetHit.transform.parent != null)
        {
            informationText.text = targetHit.name;
        }
        else
        {
            if(targetHit.transform.childCount > 0)
                informationText.text = targetHit.transform.GetChild(0).name;
            else
                informationText.text = "Empty";
        }
        informationBox.GetComponent<RectTransform>().anchoredPosition = pos + new Vector2(200, 100);

        isInformationBoxActive = !isInformationBoxActive;

    }
    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        int index = 0;
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
            index++;
        }

        transform.position = player.transform.position + new Vector3(0,0,-5);

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);

        informationBox.SetActive(isInformationBoxActive);
        if(hit != null && hit.transform.parent != null && hit.transform.parent.name.Contains("Tile") && keyboard.xKey.wasPressedThisFrame)
        {

            Destroy(hit.gameObject);
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            OpenInformationBox(hit);
            return;
        }

        if(hit == null || !hit.gameObject.name.Contains("Tile")) return;
        
        if (Input.GetMouseButtonDown(1))
        {
            OpenInformationBox(hit);
            return;
        }

        if(Input.GetMouseButtonDown(0) && hit.transform.childCount == 0 && gameControlScript.currentSelectionId != -1)
        {
            BuildOnTile(gameControlScript.currentSelectionId, hit.gameObject);
        }


        if(hit.transform.childCount > 0 && keyboard.xKey.wasPressedThisFrame)
            Destroy(hit.transform.GetChild(0).gameObject);
    }

    public void BuildOnTile(int targetId, GameObject targetTile)
    {
        Building targetBuilding = gameDataBaseScript.FindBuildingClassById(targetId);
        
        GameObject buildingClone = Instantiate(buildingPrefab, targetTile.transform.position, Quaternion.identity);
        buildingClone.name = targetBuilding.name;
        buildingClone.transform.parent = targetTile.transform;
        buildingClone.GetComponent<BuildingScript>().building = new Building(targetBuilding);
    }
}
