namespace CaptainPinkTurd.Core.DesignPattern
{
    public interface ICommand 
    {
        void Execute();
        void Undo();
    }
    public interface ICommand<T>
    {
        void Execute(T item);
        void Undo();
    }
}