using UnityEngine;

public class SpriteLookAt : MonoBehaviour
{
    [Tooltip("Optional target for the sprite.\nWill target the active (or Main) camera when this is unspecified.")]
    [SerializeField] private Transform target;

    // Update is called once per frame
    void Update()
    {
        if (DoLookAtTransform(out Transform lookAt))
            transform.LookAt(lookAt);
    }

    /// <summary>
    /// Fairly unnecessary but passes on the <see cref="Transform" /> in a way such that the
    /// <see cref="Transform.LookAt(Transform)" /> calls can be reduced to just a single one
    /// in <see cref="Update" /> up above.
    /// </summary>
    /// <param name="lookAt">The <see cref="Transform" /> to be "looked" at.</param>
    /// <returns>A <see cref="bool" /> for whether the <see cref="Transform.LookAt(Transform)" />
    /// call should take place (useful in these types of setups).</returns>
    private bool DoLookAtTransform(out Transform lookAt)
    {
        if (target)
        {
            lookAt = target;
            return true;
        }
        else if (Camera.main)
        {
            lookAt = Camera.main.transform;
            return true;
        }
        else if (Camera.current)
        {
            lookAt = Camera.current.transform;
            return true;
        }

        lookAt = transform;
        return false;
    }
}
