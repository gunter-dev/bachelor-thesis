using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public List<ElectricityInfo> affectedBlocks;
    public List<GameObject> affectedGameObjects;

    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;

    public Sprite unpressedTexture;
    public Sprite pressedTexture;

    // Start is called before the first frame update
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
        InstantiateAffectedBlocks();
    }

    private void InstantiateAffectedBlocks()
    {
        foreach (var block in affectedBlocks)
        { 
            if (block.colorCode != 0)
            {
                affectedGameObjects.Add(
                Instantiate(Resources.Load("Grounds/Grass Ground") as GameObject, new Vector3(block.x, block.y, 1), Quaternion.identity)
                );
            }
        }
    }

    private void HandlePressed()
    {
        _spriteRenderer.sprite = pressedTexture;
        
        foreach (var affectedObject in affectedGameObjects)
        {
            affectedObject.SetActive(false);
        }
    }

    private void HandleRelease()
    {
        _spriteRenderer.sprite = unpressedTexture;
        
        foreach (var affectedObject in affectedGameObjects)
        {
            affectedObject.SetActive(true);
        }
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
