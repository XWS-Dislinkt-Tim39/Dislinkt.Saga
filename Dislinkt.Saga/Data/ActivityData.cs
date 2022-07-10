using System;

namespace Dislinkt.Saga.Data
{
    public class ActivityData
    {
        public Guid UserId { get; set; }
        public string Text { get; set; }

        public DateTime Date { get; set; }
        public ActivityType Type { get; set; }
    }
}
