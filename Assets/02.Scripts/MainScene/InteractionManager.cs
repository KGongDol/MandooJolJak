using UnityEngine;
using System.Linq;

public class InteractionManager : MonoBehaviour
{
    IInteractive curInt;
    IInteractive prevInt;
    int interIdx = 0;

    public Building[] buildingList;
    IInteractive[] interList;
    Vector3 prevMousePos = Vector3.zero;
    void Awake()
    {
        interList = buildingList
            .Select(b => b.GetComponent<IInteractive>())
            .Where(i => i != null)
            .ToArray();
    }

    void Start()
    {
        InputManager.instance.OnClicked += OnInteract;
        InputManager.instance.OnReturn += OnInteract;
        InputManager.instance.OnRightArrow += SelectPrevInteract;
        InputManager.instance.OnLeftArrow += SelectNextInteract;
    }

    void Update()
    {
        // 마우스 포인터가 UI위에 있다면 Return
        if(InputManager.instance.IsPointerOverUI())
            return;

        SenseInteractive();

        if(curInt != prevInt)
        {
            if(prevInt != null)
                prevInt.UnsetInteract();
            prevInt = curInt;    
    
            if(curInt !=null)
                MainManager.instance.PlayPointerSFX();
        }

        if(curInt != null)
        {
            curInt.IsCanInteract();
        }
    }

    void SenseInteractive()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;

        if(mousePos == prevMousePos)
            return;

        prevMousePos = mousePos;

        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        if(hit.collider != null)
        {
            if(hit.collider.TryGetComponent<IInteractive>(out var interactive))
            {
                curInt = interactive;
            }
        }
        else
        {
            curInt = null;
        }
    }

    void OnInteract()
    {
        if(curInt != null)
            curInt.Interaction();
    }

    void SelectNextInteract()
    {
        interIdx++;
        if(interIdx >= interList.Length)
            interIdx = 0;
        curInt = interList[interIdx];
    }
    void SelectPrevInteract()
    {
        interIdx--;
        if(interIdx< 0)
            interIdx = interList.Length-1;

        curInt = interList[interIdx];
    }
}
