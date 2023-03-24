using UnityEngine;

public class KeyController : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(Constants.PlayerTag))
        {
            Destroy(gameObject);
            col.gameObject.GetComponent<Player>().keysNeeded--;
        }
    }
}
