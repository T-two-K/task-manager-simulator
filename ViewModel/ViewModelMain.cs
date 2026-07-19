using System;
using System.Collections.Generic;
using TaskManagerSimulator.Model;
using System.Collections.ObjectModel;
using System.Text;

namespace TaskManagerSimulator.ViewModel
{
    public class ViewModelMain
    {
        public ObservableCollection<ViewModelTask> Tasks { get; set; }

        public static event Func<Task>? StartNext;
        public static ObservableCollection<ViewModelTask> RunningTasks { get; set; } = new ObservableCollection<ViewModelTask>();
        public static Queue<ViewModelTask> WaitingTasks { get; set; } = new Queue<ViewModelTask>();

        public ViewModelMain()
        {
            Tasks = new ObservableCollection<ViewModelTask>()
            {
                new ViewModelTask(new ModelTask("First task", 1, 0, ModelTask.Status.Default, DateTime.Now)),
                new ViewModelTask(new ModelTask("Second task", 2, 0, ModelTask.Status.Default, DateTime.Now)),
                new ViewModelTask(new ModelTask("Third task", 4, 0, ModelTask.Status.Default, DateTime.Now)),
                new ViewModelTask(new ModelTask("Fourth task", 3, 0, ModelTask.Status.Default, DateTime.Now)),
                new ViewModelTask(new ModelTask("Fifth task", 1, 0, ModelTask.Status.Default, DateTime.Now)),
            };
        }

        public static void NotifyStartNext()
        {
            if (StartNext != null) StartNext.Invoke() ;
        }
    }
}
