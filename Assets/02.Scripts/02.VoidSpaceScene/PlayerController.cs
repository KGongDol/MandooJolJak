using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f; // 이동 속도
    public float rotateSpeed = 100.0f;

    [Header("Interaction (상호작용)")]
    public LayerMask interactableLayer; // 상호작용 가능한 대상 레이어 (예: NPC)
    public Vector3 indicatorOffset = new Vector3(0, 2f, 0); // 대상 오브젝트 위에 띄울 인디케이터의 오프셋
    public GameObject interactionIndicator; // R키 상호작용 가능성을 알려주는 UI (말풍선 등)
    public GameObject dialogueBox; // 대화창 창
    public TextMeshProUGUI dialogueNameText; // 대화창의 이름 텍스트
    public TextMeshProUGUI dialogueBodyText; // 대화창의 내용 텍스트

    private Animator animator;
    private List<Collider> nearbyNPCs = new List<Collider>(); // 반경 내에 들어온 NPC 목록

    // 대화 상태 관리
    public bool isAnimateMove = false;
    private bool isTalking = false;
    private NPCController currentInteractNPC;
    private int currentDialogueIndex = 0;
    public float holdFInterval = 0.2f;      // F 꾹 누름 시 초기 대화 진행 간격 (초)
    public float holdFAcceleration = 0.3f;  // 누르고 있을수록 간격이 줄어드는 속도 (초당)
    private float holdFTimer = 0f;
    private float currentHoldInterval;      // 현재 동적으로 변하는 간격

    void Start()
    {
        animator = GetComponent<Animator>();

        // [최적화 - 프레임 드랍 방지] 
        // TextMeshPro와 UI Canvas는 '최초 등쟝' 시 폰트 아틀라스를 렌더링하고 레이아웃을 재구성하느라 일시적인 렉(스파이크)을 유발합니다.
        // 이를 방지하기 위해 게임 시작 직후 보이지 않는 찰나의 순간 강제로 한 번 켰다가 꺼서(Pre-warm) 캐싱을 미리 끝냅니다.
        if (interactionIndicator != null)
        {
            interactionIndicator.SetActive(true);
            var tmps = interactionIndicator.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var tmp in tmps)
            {
                tmp.ForceMeshUpdate();
            }
            interactionIndicator.SetActive(false);
        }
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");

        // 대화 중일 경우 플레이어 조작 차단 및 F키 진행 로직 처리
        if (isTalking)
        {
            animator.SetBool("IsMove", false); // 걷기 애니메이션 정지

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EndDialogue();
                return;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                holdFTimer = 0f;
                currentHoldInterval = holdFInterval;
                DisplayNextDialogue();
            }
            else if (Input.GetKey(KeyCode.F))
            {
                // 누르고 있을수록 간격을 줄여 가속, 최소 0.05초
                currentHoldInterval = Mathf.Max(0.05f, currentHoldInterval - holdFAcceleration * Time.deltaTime);

                holdFTimer += Time.deltaTime;
                if (holdFTimer >= currentHoldInterval)
                {
                    holdFTimer = 0f;
                    DisplayNextDialogue();
                }
            }
            else
            {
                holdFTimer = 0f;
                currentHoldInterval = holdFInterval;
            }

            return; // 이동 및 새로운 상호작용은 차단
        }

        // 입력이 있으면 이동 중(true), 없으면(false)
        bool isMove = h != 0;
        animator.SetBool("IsMove", isMove);

        if (isMove)
        {
            // 입력값 방향으로 바라보도록 타겟 회전값 설정 (좌: -X, 우: +X)
            Vector3 direction = new Vector3(-h, 0, 0);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            
            // 지정한 rotateSpeed(초당 회전 각도)를 기준으로 부드럽게 회전
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            
            // 실제 위치 이동 (transform.position 직접 제어)
            if(!isAnimateMove)
                transform.position += new Vector3(h, 0, 0).normalized * moveSpeed * Time.deltaTime;
        }

        // 상호작용 관련 로직 처리
        HandleInteraction();
    }

    private void HandleInteraction()
    {
        // 리스트 정리 (게임 도중 NPC가 파괴되었을 수 있으므로 null 제거)
        nearbyNPCs.RemoveAll(npc => npc == null);

        Collider closestNPC = null;
        float minDistance = float.MaxValue;

        // Player의 콜라이더와 겹쳐진 NPC들 중 가장 가까운 대상을 찾습니다.
        foreach (var hit in nearbyNPCs)
        {
            if (hit.gameObject == gameObject) continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestNPC = hit;
            }
        }

        // 2. 상호작용 대상이 1개라도 있는 경우
        if (closestNPC != null)
        {
            // 하위 콜라이더일 수 있으므로 자신 및 부모 계층에서 NPCController를 검색
            NPCController npcController = closestNPC.GetComponentInParent<NPCController>();
            Transform npcRoot = npcController != null ? npcController.transform : closestNPC.transform;

            // 인디케이터 활성화 및 대상 머리 위쪽으로 위치 이동
            if (interactionIndicator != null)
            {
                interactionIndicator.SetActive(true);

                // 대상 오브젝트의 3D 월드 좌표를 화면상(2D 스크린) 좌표로 변환하여 UI 캔버스에 맞춥니다.
                Vector3 worldPos = npcRoot.position + indicatorOffset;
                interactionIndicator.transform.position = Camera.main.WorldToScreenPoint(worldPos);
            }

            // R키로 상호작용 시
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (npcController != null && npcController.dialogues != null && npcController.dialogues.Count > 0)
                {
                    StartDialogue(npcController);
                }
                else
                {
                    Debug.Log($"대상 {closestNPC.name}에 유효한 NPCController 또는 대화 내용이 없습니다.");
                }
            }
        }
        else
        {
            // 반경 내에 대상이 없으면 인디케이터 끄기
            if (interactionIndicator != null)
            {
                interactionIndicator.SetActive(false);
            }
        }
    }

    private void StartDialogue(NPCController npc)
    {
        currentInteractNPC = npc;
        currentDialogueIndex = 0;
        isTalking = true;

        if (interactionIndicator != null)
            interactionIndicator.SetActive(false);

        if (dialogueBox != null)
            dialogueBox.SetActive(true);

        Vector3 lookDir = npc.transform.position - transform.position;
        lookDir.y = 0;
        StartCoroutine(SmoothRotateTowards(Quaternion.LookRotation(lookDir)));

        DisplayNextDialogue();
    }

    private IEnumerator SmoothRotateTowards(Quaternion targetRotation, float duration = 0.3f)
    {
        Quaternion startRotation = transform.rotation;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            yield return null;
        }
        transform.rotation = targetRotation;
    }

    private void DisplayNextDialogue()
    {
        if (currentInteractNPC == null || currentDialogueIndex >= currentInteractNPC.dialogues.Count)
        {
            EndDialogue();
            return;
        }

        NPCDialogue dialogue = currentInteractNPC.dialogues[currentDialogueIndex];
        
        if (dialogueNameText != null)
            dialogueNameText.text = dialogue.npcName;
            
        if (dialogueBodyText != null)
            dialogueBodyText.text = dialogue.scripts;

        TriggerTalkAnimation(dialogue.npcName);

        currentDialogueIndex++;
    }

    private void TriggerTalkAnimation(string speakerName)
    {
        Animator targetAnimator = null;

        if (speakerName == "Chikitoka")
        {
            targetAnimator = this.animator; // 치키토카의 경우 플레이어 본인의 애니메이터 사용
        }
        else
        {
            // 다른 NPC일 경우 씬에서 이름으로 검색
            GameObject speakerObj = GameObject.Find(speakerName);
            
            // 씬에 Millo로 등록되어 있을 경우를 대비한 예외 처리
            if (speakerObj == null && speakerName == "Milo") 
            {
                speakerObj = GameObject.Find("Millo");
            }

            if (speakerObj != null)
            {
                targetAnimator = speakerObj.GetComponent<Animator>();
            }
        }

        if (targetAnimator != null)
        {
            targetAnimator.SetTrigger("Talk");
        }
    }

    private void EndDialogue()
    {
        isTalking = false;
        currentInteractNPC = null;
        
        if (dialogueBox != null)
            dialogueBox.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 객체가 interactableLayer에 포함되는 레이어인지 확인
        if (((1 << other.gameObject.layer) & interactableLayer) != 0)
        {
            if (!nearbyNPCs.Contains(other))
            {
                nearbyNPCs.Add(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & interactableLayer) != 0)
        {
            nearbyNPCs.Remove(other);
        }
    }

    void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 3);
    }
}
