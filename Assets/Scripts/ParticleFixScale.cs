using UnityEngine;

public class ParticleFixScale : MonoBehaviour
{
    void Update()
    {
        if (transform.localScale.x <= 0f)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
        }
    }
}
