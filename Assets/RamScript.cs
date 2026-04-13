using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RamScript : MonoBehaviour
{
    private GameControlScript gameControlScript;
    public GameObject damageNotificationPrefab;
    public int health = 0;

    void Start()
    {
        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();
        health = 300;
    }

    void Update()
    {
        if(health <= 0)
        {
            gameControlScript.GameOver();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("Bullet") && collision.name.Contains("Enemy"))
        {
            
            DamageNotification(25);
            Destroy(collision.gameObject);
            health -= 10;
        }

        if(collision.name.Contains("Cyber"))
        {
            Destroy(collision.gameObject);
            health -= 30;
        }
    }
    private void DamageNotification(float damage)
    {
        
        GameObject damageNofiticationClone = Instantiate(damageNotificationPrefab, transform.position + new Vector3(0,0.2f,0), Quaternion.identity);
        damageNofiticationClone.GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        damageNofiticationClone.GetComponentInChildren<TMP_Text>().text = (damage).ToString();
        Destroy(damageNofiticationClone, 0.5f);
    }
}
