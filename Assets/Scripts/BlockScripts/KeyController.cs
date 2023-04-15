using System.Collections.Generic;
using UnityEngine;

namespace BlockScripts
{
    public class KeyController : MonoBehaviour
    {
        public static readonly Dictionary<DoorController, int> KeysCount = new();

        public DoorController doorController;
        
        private void Start()
        {
            KeysCount.TryAdd(doorController, 0);
            KeysCount[doorController]++;
        }

        
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag(Constants.PlayerTag))
            {
                Destroy(gameObject);
                KeysCount[doorController]--;
            }
        }
    }
}
