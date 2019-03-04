using System;

namespace WebApi.Entities
{
    public class Queue
    {
        public string slotId { get; set; }
        public DateTime slotDate { get; set; }
        public DateTime slotTime { get; set; }
        public Boolean waitingList { get; set; }
        public string queueStatusId { get; set; }

    }
}
