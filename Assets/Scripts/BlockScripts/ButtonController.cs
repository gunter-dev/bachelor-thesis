using System.Collections.Generic;
using UnityEngine;

namespace BlockScripts
{
    public class ButtonController : MonoBehaviour
    {
        public List<ElectricityInfo> affectedBlocks = new();
        public List<GameObject> affectedGameObjects;

        private SpriteRenderer _spriteRenderer;

        public Sprite unpressedTexture;
        public Sprite pressedTexture;

        private bool _pressed;
        private GameObject _triggerObject;

        // Start is called before the first frame update
        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            InstantiateAffectedBlocks();
        }

        private void InstantiateAffectedBlocks()
        {
            foreach (var block in affectedBlocks)
            {
                if (block.colorCode != 0)
                {
                    Vector3 position = new Vector3(block.x, block.y, 1);
                    GameObject affected = Resources.Load<GameObject>("Grounds/Electricity");
                    affected = Instantiate(affected, position, Quaternion.identity);
                
                    affectedGameObjects.Add(affected);
                }
            }
        }

        private void HandlePressed(GameObject triggerObject)
        {
            _spriteRenderer.sprite = pressedTexture;
            _pressed = true;
            _triggerObject = triggerObject;
        
            foreach (var affectedObject in affectedGameObjects)
                affectedObject.SetActive(false);
        }

        private void HandleRelease()
        {
            _spriteRenderer.sprite = unpressedTexture;
            _pressed = false;
        
            foreach (var affectedObject in affectedGameObjects)
                affectedObject.SetActive(true);
        }
    
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (!_pressed) HandlePressed(col.gameObject);
        }
    
        private void OnCollisionExit2D(Collision2D col)
        {
            if (col.gameObject == _triggerObject) HandleRelease();
        }
    }
}
