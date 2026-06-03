using UnityEngine;

public class FaceObject : MonoBehaviour
{
    public Transform target;
    Transform cachedTransform;

    void Awake()
    {
        cachedTransform = transform;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 directionToTarget = cachedTransform.position - target.position;
        if (directionToTarget.sqrMagnitude > 0.0001f)
        {
            cachedTransform.rotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
        }
    }
}
