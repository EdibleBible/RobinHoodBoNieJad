public interface IInteractableStop
{
    public bool IsInteracting { get; set; }
    public void ShowStopUI();
    public void StopInteracting();
    
}