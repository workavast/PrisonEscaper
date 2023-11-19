public interface IInteractive
{
    public bool Interactable { get; }
    public UnityEngine.Transform transform { get; }
    public void Interact();
}