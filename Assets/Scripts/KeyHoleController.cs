using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyHoleController : MonoBehaviour
{
    private const string PlayerTag = "Player";
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(PlayerTag) && col.gameObject.GetComponent<Player>().keysNeeded == 0)
            Destroy(gameObject);
    }
}
