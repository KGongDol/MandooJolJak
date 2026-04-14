using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target & Speed")]
    public Transform target;
    public float smoothSpeed = 5.0f;
    
    [Header("Camera Offset")]
    public Vector3 offset = new Vector3(0f, 2f, -10f);

    void LateUpdate()
    {
        if (target == null) return;

        // 목표 위치 계산 (Y축은 플레이어를 따라가지 않고 offset.y로 고정)
        Vector3 desiredPosition = new Vector3(target.position.x + offset.x, offset.y, target.position.z + offset.z);
        
        // 현재 위치에서 목표 위치로 부드럽게 이동 (선형 보간)
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
