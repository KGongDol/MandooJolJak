using UnityEngine;

public class WaypointMove : MonoBehaviour
{
    public static WaypointMove instance;
    public static WaypointMove Instance
    {
        get
        {
            if(instance == null)
                return null;
            else
                return instance;
        }
    }

    public Transform[] waypoints;
    public Transform mainChar;
    public float moveSpeed = 5f;
    public float arrivalThreshold = 0.05f;

    internal int currentPointIdx = 0;
    internal int targetPointIdx = 0;

    // 현재 이동 중인 다음 웨이포인트 인덱스
    private int nextPointIdx = 0;

    [Header("방향별 Y축 회전값")]
    public float rotXnYn = -210f;  // x < 0, y < 0
    public float rotXnYp =   60f;  // x < 0, y > 0
    public float rotXpYp =  300f;  // x > 0, y > 0
    public float rotXpYn =  150f;  // x > 0, y < 0
    public float rotateSpeed = 360f;

    void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (currentPointIdx == targetPointIdx) return;

        int direction = (targetPointIdx > currentPointIdx) ? 1 : -1;
        nextPointIdx = currentPointIdx + direction;

        Transform nextWaypoint = waypoints[nextPointIdx];
        Vector3 nextPos = new(nextWaypoint.position.x, nextWaypoint.position.y, 0f);
        Vector3 charPos = new(mainChar.position.x, mainChar.position.y, 0f);

        Vector3 moveDir = (nextPos - charPos).normalized;
        if (moveDir != Vector3.zero)
        {
            float targetY;
            if      (moveDir.x < 0 && moveDir.y < 0) targetY = rotXnYn;
            else if (moveDir.x < 0 && moveDir.y > 0) targetY = rotXnYp;
            else if (moveDir.x > 0 && moveDir.y > 0) targetY = rotXpYp;
            else                                      targetY = rotXpYn;

            Quaternion targetRot = Quaternion.Euler(0f, targetY, 0f);
            mainChar.rotation = Quaternion.RotateTowards(mainChar.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }

        mainChar.position = Vector3.MoveTowards(mainChar.position, nextWaypoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(charPos, nextPos) <= arrivalThreshold)
        {
            mainChar.position = nextWaypoint.position;
            currentPointIdx = nextPointIdx;
        }
    }
}
