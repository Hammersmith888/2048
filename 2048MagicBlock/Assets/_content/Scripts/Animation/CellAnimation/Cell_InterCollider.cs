using UnityEngine;

public class Cell_InterCollider : MonoBehaviour
{
    private BoxCollider2D _boxCollider2D;

    private void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _boxCollider2D.isTrigger = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _boxCollider2D.isTrigger = false;
    }
}
