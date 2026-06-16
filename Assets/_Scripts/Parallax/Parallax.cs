using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Camera cam;
    public Transform subject;
    public bool moveY = true;

    [Range(0f, 1f)]
    [Tooltip("Scales ONLY the vertical parallax, independently of horizontal. " +
             "1 = same strength as horizontal, 0 = no vertical movement. " +
             "Lower it if layers separate and reveal gaps/edges when jumping up.")]
    public float verticalParallaxScale = 0.35f;

    Vector2 startPosition;
    float startZ;
    Vector2 camStartPosition;

    // Camera displacement since Start, measured from the CAMERA's own start position.
    // (The old version measured from the layer's position, which made every layer
    //  teleport on load and exaggerated the vertical drift.)
    Vector2 travel => (Vector2)cam.transform.position - camStartPosition;

    float distanceFromSubject => transform.position.z - subject.position.z;
    float clippingPlane => cam.transform.position.z + (distanceFromSubject > 0 ? cam.farClipPlane : cam.nearClipPlane);

    float parallaxFactor => Mathf.Abs(distanceFromSubject) / clippingPlane;

    private void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (cam == null || subject == null)
        {
            Debug.LogError($"[Parallax] '{name}' needs both 'cam' and 'subject' assigned. Disabling.", this);
            enabled = false;
            return;
        }

        startPosition = transform.position;
        startZ = transform.position.z;
        camStartPosition = cam.transform.position;
    }

    // LateUpdate so the layer reads the camera AFTER it has moved this frame
    // (CameraSmothFollow also runs in LateUpdate), avoiding parallax jitter.
    private void LateUpdate()
    {
        float factor = parallaxFactor;
        float x = startPosition.x + travel.x * factor;
        float y = moveY
            ? startPosition.y + travel.y * factor * verticalParallaxScale
            : startPosition.y;

        transform.position = new Vector3(x, y, startZ);
    }
}
