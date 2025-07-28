using UnityEngine;

public class CameraDistance : MonoBehaviour
{
    [SerializeField] float maximumCameraDistance = 7;
    [SerializeField] LayerMask layers;

    [SerializeField] float pillow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 forward = -transform.forward;

        RaycastHit raycast;
        Physics.Linecast(transform.parent.position, transform.parent.position + (forward * maximumCameraDistance), out raycast, layers);

        float distance = raycast.collider != null ? raycast.distance - pillow : maximumCameraDistance;

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -distance);
    }
}
