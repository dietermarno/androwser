using System.Collections.Generic;
using System.Windows.Forms;

namespace androwser
{
    public class ScheduleData
    {
        public string url { get; set; }
        public string wakeup { get; set; }
        public string die { get; set; }
        public Form browser { get; set; }
    }
    public class Schedule
    {
        public List<ScheduleData> urls { get; set; }
    }
}
