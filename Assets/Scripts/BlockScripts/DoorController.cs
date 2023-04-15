using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BlockScripts
{
    public class DoorController : MonoBehaviour
    {
        public List<Coordinates> keys;

        public Color lightColor;

        private void Start()
        {
            InstantiateKeys();
            SetColor();
        }

        private void InstantiateKeys()
        {
            foreach (Coordinates key in keys)
            {
                GameObject keyObject = Resources.Load<GameObject>("Key");
                keyObject = Instantiate(keyObject, new Vector2(key.x, key.y), Quaternion.identity);
                keyObject.GetComponent<KeyController>().doorController = this;

                keyObject.GetComponent<Light2D>().color = lightColor;
            }
        }

        private void SetColor()
        {
            gameObject.GetComponent<Light2D>().color = lightColor;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            int keysNeeded = KeyController.KeysCount[this];
            if (col.gameObject.CompareTag(Constants.PlayerTag) && keysNeeded == 0)
                Destroy(gameObject);
        }
    }
}
