using CompatibilityAnalyzer.Models;
using System;

namespace CompatibilityAnalyzer.Messaging
{
    public interface IRequestObservable : IObservable<IMessage<AnalyzeRequest>>
    {
    }
}
