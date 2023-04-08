using System.Collections.Generic;
using UnityEngine;

namespace BlockScripts
{
    public class KeyController : MonoBehaviour
    {
        public static readonly Dictionary<KeyHoleController, int> KeysCount = new();

        public KeyHoleController keyHoleController;
        
        private void Start()
        {
            KeysCount.TryAdd(keyHoleController, 0);
            KeysCount[keyHoleController]++;
        }

        
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag(Constants.PlayerTag))
            {
                Destroy(gameObject);
                KeysCount[keyHoleController]--;
            }
        }
    }
}
