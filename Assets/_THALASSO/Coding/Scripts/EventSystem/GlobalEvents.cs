
public class GlobalEvents
{
    public class UI // 0 - 99
    {
        public const uint MenuOpened = 0;
        public const uint MenuClosed = 1;
    }

    public class Game // 100 - 299
    {
        public const uint TestTriggered = 100;
        public const uint ProgressionCompleted = 101;
        public const uint HasBeenSolved = 102;
    }

    public class Player // 300 - 499
    {
        public const uint GroundedStateChanged = 300;
        public const uint GroundSoundMaterialChanged = 301;
        public const uint InteractiveTargetChanged = 302;
    }
}
