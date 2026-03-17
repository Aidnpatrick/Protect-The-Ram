using UnityEngine;
using UnityEngine.InputSystem;

public class CameraScript : MonoBehaviour
{
    public GameObject player;
    public GameObject informationBox;
    public bool isInformationBoxActive = false;
    private Vector2 moveInput;
    void Start()
    {
        
        isInformationBoxActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + new Vector3(0,0,-5);

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);
        Keyboard keyboard = Keyboard.current;

        informationBox.SetActive(isInformationBoxActive);

        if(hit == null || !hit.gameObject.name.Contains("Tile")) return;
        
        if(Input.GetMouseButtonDown(1))
        {
            informationBox.GetComponent<RectTransform>().anchoredPosition = new Vector2(mousePos.x*20,mousePos.y*20);
            isInformationBoxActive = !isInformationBoxActive;
        }
    }

}
