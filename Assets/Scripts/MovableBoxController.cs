using UnityEngine;

public class MovableBoxController : MonoBehaviour
{
    private float _xMovement;

    private bool _reversedGravity;
    private bool _onAccelerator;
    
    private const float AcceleratorSpeed = 1.6f;
    
    private const string GravityBlockTag = "Gravity";
    private const string AcceleratorTag = "Accelerator";

    // Update is called once per frame
    private void Update()
    {
        BoxMovement();
    }

    private void BoxMovement()
    {
        transform.position += Time.deltaTime * new Vector3(_xMovement, 0f, 0f);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        _xMovement = col.gameObject.CompareTag(AcceleratorTag) ? AcceleratorSpeed : 0;
        if (col.gameObject.CompareTag(GravityBlockTag)) HandleGravityChange();
    }

    private void HandleGravityChange()
    {
        _reversedGravity = !_reversedGravity;
        Physics2D.gravity = new Vector2(0, 9.8f * (_reversedGravity ? 1 : -1));
    }
}
