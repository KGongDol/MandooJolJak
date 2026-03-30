using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public static InputManager Instance
    {
        get
        {
            if(instance == null)
                return null;
            else
                return instance;
        }
    }

    public event Action OnClicked;
    public event Action OnRightArrow;
    public event Action OnLeftArrow;
    public event Action OnReturn;

    void Awake()

    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(IsPointerOverUI())
                return;

            OnClicked?.Invoke();
        }

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnRightArrow?.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnLeftArrow?.Invoke();
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            OnReturn?.Invoke();
        }
    }

    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();
}