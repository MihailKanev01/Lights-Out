using UnityEngine;

public class HeadBobController : MonoBehaviour
{
    [SerializeField] private bool enable = true;
    [SerializeField, Range(0, 0.1f)] private float amplitude = 0.015f;
    [SerializeField, Range(0, 30)] private float frequency = 10.0f;
    [SerializeField] private Transform cameraTransform = null;
    [SerializeField] private Transform cameraHolder = null;
    [SerializeField] private CharacterController controller = null;

    private float toggleSpeed = 3.0f;
    private Vector3 startPos;

    private void Awake()
    {
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
        startPos = cameraTransform.localPosition;
    }

    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * frequency) * amplitude;
        pos.x += Mathf.Cos(Time.time * frequency / 2) * amplitude * 2;
        return pos;
    }

    private void CheckMotion()
    {
        float speed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;
        if (speed < toggleSpeed || !controller.isGrounded) return;
        PlayMotion(FootStepMotion());
    }

    private void PlayMotion(Vector3 motion)
    {
        cameraTransform.localPosition += motion;
    }

    private void ResetPosition()
    {
        if (cameraTransform.localPosition == startPos) return;
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, startPos, 1 * Time.deltaTime);
    }

    public void Update()
    {
        if (!enable) return;
        CheckMotion();
        ResetPosition();
    }
}
