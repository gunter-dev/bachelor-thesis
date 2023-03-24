using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public List<ElectricityInfo> affectedBlocks;
    public List<GameObject> affectedGameObjects;

    private SpriteRenderer _spriteRenderer;

    public Sprite unpressedTexture;
    public Sprite pressedTexture;

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
                GameObject affected = Resources.Load<GameObject>("Grounds/Grass Ground 1");
                affected = Instantiate(affected, position, Quaternion.identity);
                
                affectedGameObjects.Add(affected);
            }
        }
    }

    private void HandlePressed()
    {
        _spriteRenderer.sprite = pressedTexture;
        
        foreach (var affectedObject in affectedGameObjects)
            affectedObject.SetActive(false);
    }

    private void HandleRelease()
    {
        _spriteRenderer.sprite = unpressedTexture;
        
        foreach (var affectedObject in affectedGameObjects)
            affectedObject.SetActive(true);
    }
    
    private void OnCollisionEnter2D()
    {
        HandlePressed();
    }
    
    private void OnCollisionExit2D()
    {
        HandleRelease();
    }
}
