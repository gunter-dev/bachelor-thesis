using UnityEngine;

public class KeyController : MonoBehaviour
{
    private const string PlayerTag = "Player";
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(PlayerTag))
        {
            Destroy(gameObject);
            col.gameObject.GetComponent<Player>().keysNeeded--;
        }
    }
}
