using System.Collections;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public GameControlScript gameControlScript;
    public CameraScript cameraScript;
    public SpriteRenderer spriteRenderer;
    public int id = 0;

    void Start() {
        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();
        cameraScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void BlinkBoundary()
    {
        StopAllCoroutines();
        StartCoroutine(BlinkBoundaryTime());
    }
    
    public IEnumerator BlinkBoundaryTime()
    {
        if(500 - gameControlScript.amountOfLand > id)
            spriteRenderer.color = Color.orange;
            
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = Color.white;
    }
}
