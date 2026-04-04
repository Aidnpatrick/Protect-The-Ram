using UnityEngine;

public class TileScript : MonoBehaviour
{
    public GameControlScript gameControlScript;
    public CameraScript cameraScript;
    public SpriteRenderer spriteRenderer;
    public int id = 0;
    public bool isBlocked = false, oreDeposit = false;

    void Start() {
        isBlocked = false;
        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();
        cameraScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(oreDeposit)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("Images/DepositTile");
        }
    }

    void Update()
    {
        if(oreDeposit)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("Images/DepositTile");
        }
        if(gameControlScript.currentSelectionId != -1 && 500 - gameControlScript.amountOfLand > id)
            spriteRenderer.color = Color.orange;
        else
            spriteRenderer.color = Color.white;

        if(transform.childCount > 0 && !transform.GetChild(0).name.Contains("Weed"))
            isBlocked = true;
    }

}
