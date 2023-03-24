using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyHoleController : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(Constants.PlayerTag) && col.gameObject.GetComponent<Player>().keysNeeded == 0)
            Destroy(gameObject);
    }
}
