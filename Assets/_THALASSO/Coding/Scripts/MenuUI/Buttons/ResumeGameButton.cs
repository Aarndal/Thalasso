
public class ResumeGameButton : ButtonClick
{
    protected override void OnClicked()
    {
        base.OnClicked();

        GlobalEventBus.Raise(GlobalEvents.Game.IsPaused, false);
    }
}
