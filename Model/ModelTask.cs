namespace TaskManagerSimulator.Model
{
    public class ModelTask : IComparable<ModelTask>
    {
        public enum Status { Default, Waiting, Pause, Running, Cancelled, Completed }

        private string? _description = string.Empty;
        private Status _status = Status.Default;
        private short _taskPriority = 0;
        private short _taskProgress = 0;
        private DateTime _startTime = DateTime.MinValue;

        public string? Description { get => _description; set => _description = value; }
        public short TaskPriority { get => _taskPriority; set => _taskPriority = value; }
        public short TaskProgress { get => _taskProgress; set => _taskProgress = value; }
        public Status TaskStatus { get => _status; set => _status = value; }
        public DateTime StartTime { get => _startTime; set => _startTime = value; }

        public ModelTask() { }

        public ModelTask(string description, short taskPriority, short taskProgress, Status status, DateTime startTime)
        {
            Description = description;
            TaskPriority = taskPriority;
            TaskProgress = taskProgress;
            TaskStatus = status;
            StartTime = startTime;
        }

        public int CompareTo(ModelTask? other)
        {
            if (other == null)
                return 0;

            return TaskPriority.CompareTo(other.TaskPriority);
        }
    }
}
