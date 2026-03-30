using ExternPropertyAttributes;

public interface IInteractive
{
    public bool IsAccess {get;}
    public void Interaction();
    public void IsCanInteract();
    public void UnsetInteract();
}
