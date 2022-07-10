using System;

namespace Dislinkt.Saga.Data
{
    public class NotificationData
    {
        public Guid UserId { get; set; }
        public bool MessageOn { get; set; }
        public bool PostOn { get; set; }
        public bool JobOn { get; set; }
        public bool FriendRequestOn { get; set; }
    }
}
