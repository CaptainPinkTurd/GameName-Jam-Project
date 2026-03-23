using CaptainPinkTurd.Core.Predicate;

namespace CaptainPinkTurd.Core.DesignPattern
{
    public interface ITransition
    {
        IState To { get; }
        IPredicate Condition { get; }
    }
}
