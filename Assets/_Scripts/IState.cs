public interface IState
{
    void EnterState();
    void UpdateState(float deltaTime);
    void ExitState();
}
