using CaptainPinkTurd.Core.Predicate;

namespace CaptainPinkTurd.Core.DesignPattern.State
{
    public interface ITransition
    {
        IState To { get; }
        IPredicate Condition { get; }
    }
}
