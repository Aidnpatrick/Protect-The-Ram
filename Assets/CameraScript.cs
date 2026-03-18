using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class CameraScript : MonoBehaviour
{
    public GameControlScript gameControlScript;
    public GameDataBaseScript gameDataBaseScript;
    public GameObject player;
    public GameObject informationBox;
    public bool isInformationBoxActive = false;
    private Vector2 moveInput;
    void Start()
    {
        isInformationBoxActive = false;
    }
    void UpdateInformationBox(Turret turret, Vector2 location)
    {
        Debug.Log("asdasdas");
        informationBox.GetComponent<RectTransform>().anchoredPosition = location + new Vector2(-100,-50);
        isInformationBoxActive = true;
        informationBox.transform.GetChild(0).GetComponent<TMP_Text>().text = turret.name + "\nDamage:" + turret.damage + "\nFireRate: " + turret.fireRate + "\nInformation: " + turret.information;
    }

    void Update()
    {
        int index = 0;
        foreach(Transform selectionContainerIndex in gameControlScript.selectionContainer.transform)
        {
            Vector2 mousePosUI = Input.mousePosition;
            if (RectTransformUtility.RectangleContainsScreenPoint(
                selectionContainerIndex.GetComponent<Image>().rectTransform, 
                mousePosUI, 
                gameControlScript.selectionContainer.GetComponentInParent<Canvas>().worldCamera))
            {
                UpdateInformationBox(gameDataBaseScript.turrets[index], selectionContainerIndex.GetComponent<RectTransform>().anchoredPosition);
                break;
            }
            else
            {
                isInformationBoxActive = false;
            }
            index++;
        }

        transform.position = player.transform.position + new Vector3(0,0,-5);

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);
        Keyboard keyboard = Keyboard.current;

        informationBox.SetActive(isInformationBoxActive);

        if(hit == null || !hit.gameObject.name.Contains("Tile")) return;
        
        if(Input.GetMouseButtonDown(1))
        {
            informationBox.GetComponent<RectTransform>().anchoredPosition = new Vector2(mousePos.x*15,mousePos.y*15);
            isInformationBoxActive = !isInformationBoxActive;
        }

        if(Input.GetMouseButtonDown(0) && hit.transform.childCount == 0)
        {
            
        }
        
    }

}
