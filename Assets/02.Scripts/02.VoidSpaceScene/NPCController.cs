using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class NPCDialogue
{
    public string npcName;
    public string scripts;
}

public class NPCController : MonoBehaviour, IInteractive
{
    public List<NPCDialogue> dialogues;
    
    public bool IsAccess => isAccess;
    bool isAccess = true;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Interaction()
    {
        
    }

    public void IsCanInteract()
    {
        
    }

    public void UnsetInteract()
    {
        
    }
}
