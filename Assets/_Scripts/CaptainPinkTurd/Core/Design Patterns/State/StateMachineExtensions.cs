using CaptainPinkTurd.Core.Predicate;

namespace CaptainPinkTurd.Core.DesignPattern.State
{
    public static class StateMachineExtensions
    {
        public static void At(this StateMachine stateMachine, IState from, IState to, IPredicate condition)
        {
            stateMachine.AddTransition(from, to, condition);
        }

        public static void Any(this StateMachine stateMachine, IState to, IPredicate condition)
        {
            stateMachine.AddAnyTransition(to, condition);       
        }
    }
}
