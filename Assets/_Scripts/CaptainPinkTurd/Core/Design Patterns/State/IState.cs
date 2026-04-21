namespace CaptainPinkTurd.Core.DesignPattern.State
{
    public interface IState
    {
        void OnEnter();
        void Update();
        void FixedUpdate();
        void OnExit();
    }
}
