namespace CompatibilityAnalyzer.Messaging
{
    public interface IMessage<T>
    {
        T Message { get; }

        void Complete();
    }

}
