using UnityEngine;

public class Building : MonoBehaviour, IInteractive
{
    public bool isAccess = false;
    public bool IsAccess => isAccess;

    public Color accessableColor;
    public Color unAccessableColor;
    
    void Awake()
    {
        UnsetInteract();
    }
    
    public void Interaction()
    {
        if(isAccess)
        {
            Debug.Log("접근가능상호작용");
        }
        else
        {
            Debug.Log("접근불가상호작용");
        }
    }

    void SetMaterial(bool isAccess)
    {
        Renderer renderer = GetComponent<Renderer>();
        Material myMat = renderer.material;
        if(isAccess)
        {
            myMat.SetColor("_SolidOutline", accessableColor);
        }
        else
        {
            myMat.SetColor("_SolidOutline", unAccessableColor);
        }
    }
    void SetOutline(bool isSet)
    {
        Renderer renderer = GetComponent<Renderer>();
        Material myMat = renderer.material;

        if(isSet)
            myMat.SetFloat("_Thickness", 35);
        else
            myMat.SetFloat("_Thickness", 0);
    }

    public void IsCanInteract()
    {
        SetOutline(true);
        if(isAccess)
        {
            SetMaterial(true);
        }
        else
        {
            SetMaterial(false);
        }
    }

    public void UnsetInteract()
    {
        SetOutline(false);
    }
}