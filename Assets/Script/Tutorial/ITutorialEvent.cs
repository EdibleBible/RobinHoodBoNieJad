public interface ITutorialEvent
{
    string EventName { get; }
    void TriggerSelectedTutorial();
}