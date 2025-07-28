using UnityEngine;

public class SpriteLookAt : MonoBehaviour
{
    [SerializeField] Transform target;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);    
    }
}
