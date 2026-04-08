using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoaderScript : MonoBehaviour
{
    //Scene Loader
    public static void LoadGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public static void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    //TUTORIAL
    public GameObject tutorialImage, tutorialCanvas;
    public int pageNum = 1;
    public void Start()
    {
        if(tutorialCanvas != null)
            tutorialCanvas.SetActive(false);
        pageNum = 1;
    }

    public void ChangePageNumUp()
    {
        if(pageNum >= 6) return;
        pageNum++;
        tutorialImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Tutorial" + pageNum);
    }
    public void ChangePageNumDown()
    {
        if(pageNum <= 1) return;
        pageNum--;
        tutorialImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Tutorial" + pageNum);

    }
}
