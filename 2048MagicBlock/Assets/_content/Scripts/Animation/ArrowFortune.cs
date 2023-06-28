using UnityEngine;

public class ArrowFortune : MonoBehaviour
{
    public float maxRotation = 90f;

    public float minRotation = 0f;

    private float targetRotation = 0f;

    private float oldRotation = 0f;

    public bool isActive = true;

    public int lastCollider = 0;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != "Wheel" || !isActive)
            return;

        //transform.rotation = 
        targetRotation = Mathf.Lerp(targetRotation, maxRotation, Time.deltaTime*15f);

        lastCollider = int.Parse(collision.gameObject.name);
    }

    private void Update()
    {
        if (isActive) { 

        }

        targetRotation = Mathf.Lerp(targetRotation, minRotation, Time.deltaTime * 5f);
        
        var newRotation = Mathf.Lerp(oldRotation, targetRotation, 5f * Time.deltaTime);

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, newRotation)); 

        oldRotation = newRotation;
    }

}

