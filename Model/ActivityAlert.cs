using System;

namespace ZF.BL.Nesper.Model
{
    public class ActivityAlert
    {
        public ActivityAlert()
        {
        }

        //retain for serialiser

        public ActivityAlert(string ruleId, DateTime occurredOn, ActivityEvent[] events)
        {
            RuleId = ruleId;
            OccurredOn = occurredOn;
            Events = events;
        }

        public string RuleId { get; set; }
        public DateTime OccurredOn { get; set; }
        public ActivityEvent[] Events { get; set; }
    }
}