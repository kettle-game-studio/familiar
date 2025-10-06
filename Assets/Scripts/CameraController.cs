
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Animator animator;
    public Player player;
    public float playerViewK = 0.1f;
    public float playerViewSpeed = 0.1f;

    Vector3 basePosition;

    void Start()
    {
        basePosition = transform.position;
    }

    void FixedUpdate()
    {
        float targetShift = (player.transform.position.x - basePosition.x) * playerViewK;
        float currentShift = transform.position.x - basePosition.x;
        float x = Mathf.Lerp(currentShift, targetShift, playerViewSpeed);
        transform.position = basePosition + Vector3.right * x;
    }

    public void ShakeDown() => animator.SetTrigger("ShakeDown");
    public void ShakeUp() => animator.SetTrigger("ShakeUp");
}
