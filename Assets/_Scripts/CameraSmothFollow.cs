using UnityEngine;

// Runs before default-order scripts so the camera settles in LateUpdate
// before Parallax reads its position (prevents a 1-frame lag / jitter).
[DefaultExecutionOrder(-100)]
public class CameraSmothFollow : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] float _smoothFactor = 1.4f;

    // LateUpdate (not FixedUpdate): camera follow runs once per rendered frame so it
    // tracks smoothly at any framerate instead of stuttering at the physics step.
    void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }

        // 1 - exp(-k*dt) is a frame-rate independent smoothing step.
        float t = 1f - Mathf.Exp(-_smoothFactor * Time.deltaTime);
        Vector3 pos = Vector3.Lerp(transform.position, _target.position, t);
        pos.z = transform.position.z;
        transform.position = pos;
    }
}
