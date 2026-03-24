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
    public bool isInformationBoxActive = false;
    private Vector2 moveInput;
    public Collider2D hitMain = null;

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
        Button informationButton = informationBox.transform.GetChild(1).GetComponent<Button>();
        Button informationDestroyButton = informationBox.transform.GetChild(2).GetComponent<Button>();
        if(targetHit.transform.parent != null)
        {
            hitMain = targetHit.GetComponent<Collider2D>();
            informationDestroyButton.gameObject.SetActive(true);

            informationText.text = targetHit.name;
            informationText.text += "\n" + targetHit.GetComponent<BuildingScript>().health + " HP\n";

            if(targetHit.name.Contains("Camp"))
                informationButton.gameObject.SetActive(true);
            else
                informationButton.gameObject.SetActive(false);
        }
        else
        {

            if(targetHit.transform.childCount > 0)
            {

                informationDestroyButton.gameObject.SetActive(true);    
                hitMain = targetHit.transform.GetChild(0).GetComponent<Collider2D>();
                informationText.text = targetHit.transform.GetChild(0).name;
                informationText.text += "\n" + targetHit.transform.GetChild(0).GetComponent<BuildingScript>().health + " HP\n";
                if(targetHit.transform.GetChild(0).name.Contains("Camp"))
                    informationButton.gameObject.SetActive(true);
                else
                    informationButton.gameObject.SetActive(false);
            }
            else
            {
                informationText.text = "Empty";
                informationDestroyButton.gameObject.SetActive(true);
                informationButton.gameObject.SetActive(false);
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
        Destroy(hitMain.gameObject);
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
            BuildOnTile(gameControlScript.currentSelectionId, hit.gameObject);


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

    public GameObject BuildOnTileMisc(GameObject prefab, int tileId)
    {
        GameObject targetTile = gameControlScript.FindTile(tileId);
        GameObject targetChild = Instantiate(prefab, targetTile.transform.position, Quaternion.identity, targetTile.transform);
        Debug.Log(targetTile + " " + targetChild);
        return targetChild;
    }
}
