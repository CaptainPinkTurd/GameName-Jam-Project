using CaptainPinkTurd.Core.Predicate;

namespace CaptainPinkTurd.Core.DesignPattern
{
    public class Transition : ITransition
    {
        public IState To { get; }
        public IPredicate Condition { get; }
        
        public Transition(IState to, IPredicate condition)
        {
            To = to;
            Condition = condition;
        }
    }
}