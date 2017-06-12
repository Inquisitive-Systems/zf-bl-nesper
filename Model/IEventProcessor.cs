using System;

namespace ZF.BL.Nesper.Model
{
    public interface IEventProcessor : IDisposable
    {
        long EventsCounter { get; }
        void Load(string id, string epl);
        void Validate(string id, string epl);
        void Unload(string ruleId);
        void UnloadAll();

        void PropagateEvent(ActivityEvent e);
        void PropagateEvents(ActivityEvent[] events);

        void Initialize();
        void WaitUntilRuleAdded();
    }
}