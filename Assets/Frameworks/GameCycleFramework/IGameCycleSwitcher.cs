namespace GameCycleFramework
{
    public interface IGameCycleSwitcher
    {
        public GameCycleState CurrentState { get; }

        public void SwitchState(GameCycleState newCycleState, bool hardSet = false);
    }
}