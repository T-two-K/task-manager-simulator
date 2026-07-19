using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using TaskManagerSimulator.Model;

namespace TaskManagerSimulator.ViewModel
{
    public partial class ViewModelTask : ObservableObject
    {
        public HashSet<short> Priority { get; } = new HashSet<short>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        private CancellationTokenSource _cancellationTokenSource;

        [ObservableProperty]
        private ModelTask _itemTask;

        [ObservableProperty]
        private short _selectedItem;

        public string Description { get => ItemTask.Description ?? "Empty task"; set { ItemTask.Description = value; OnPropertyChanged(); } }
        public DateTime StartTime
        {
            get => ItemTask.StartTime;
            set
            { ItemTask.StartTime = value; OnPropertyChanged(); }
        }
        public short TaskPriority
        {
            get => ItemTask.TaskPriority;
            set
            { ItemTask.TaskPriority = value; OnPropertyChanged(); }
        }
        public short TaskProgress
        {
            get => ItemTask.TaskProgress;
            set
            { ItemTask.TaskProgress = value; OnPropertyChanged(); }
        }
        public ModelTask.Status TaskStatus
        {
            get => ItemTask.TaskStatus;
            set
            { ItemTask.TaskStatus = value; OnPropertyChanged(); }
        }

        public string ButtonContent => ItemTask.TaskStatus switch
        {
            ModelTask.Status.Default => "Start",
            ModelTask.Status.Running => "Pause",
            ModelTask.Status.Cancelled => "Start",
            ModelTask.Status.Pause => "Continue",
            ModelTask.Status.Completed => "Complete",
            ModelTask.Status.Waiting => "Waiting",
            _ => "Error"
        };

        public ViewModelTask(ModelTask itemTask)
        {
            _itemTask = itemTask;

            ViewModelMain.StartNext += StartPendingTask;

            PropertyChanged += ChangePriority;
        }

        public void ChangePriority(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedItem))
                TaskPriority = SelectedItem;

            if (e.PropertyName == nameof(TaskStatus))
                OnPropertyChanged(nameof(ButtonContent));
        }

        [RelayCommand]
        public async Task ChoiceTask()
        {
            switch (TaskStatus)
            {
                case ModelTask.Status.Default:
                    await StartTask();
                    break;
                case ModelTask.Status.Running:
                    await Pause();
                    break;
            }
        }

        public bool CanChoiceTask()
        {
            if (TaskStatus == ModelTask.Status.Waiting)
                return false;

            return true;
        }

        public async Task Pause()
        {
            TaskStatus = ModelTask.Status.Pause;
            ViewModelMain.RunningTasks.Remove(this);
            await Task.Delay(200);
            ViewModelMain.NotifyStartNext();
        }

        public async Task StartTask()
        {
            if (ViewModelMain.RunningTasks.Count < 2)
            {
                _cancellationTokenSource = new();
                ViewModelMain.RunningTasks.Add(this);
                await AsyncUpdateProgress(_cancellationTokenSource.Token);
            }
            else
            {
                TaskStatus = ModelTask.Status.Waiting;
                _cancellationTokenSource = new();
                ViewModelMain.WaitingTasks.Enqueue(this);
            }
        }

        public async Task AsyncUpdateProgress(CancellationToken token)
        {
            try
            {
                TaskStatus = ModelTask.Status.Running;

                while (TaskProgress < 100)
                {
                    if (token.IsCancellationRequested)
                        token.ThrowIfCancellationRequested();

                    if (TaskStatus == ModelTask.Status.Pause)
                        return;

                    TaskProgress++;

                    await Task.Delay(200, token);
                }

                TaskStatus = ModelTask.Status.Completed;
            }
            catch (OperationCanceledException)
            {
                TaskStatus = ModelTask.Status.Cancelled;
                TaskProgress = 0;
            }
            finally
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();

                ViewModelMain.RunningTasks.Remove(this);
                ViewModelMain.NotifyStartNext();
            }
        }

        public async Task StartPendingTask()
        {
            if (CanStartPendingTask())
            {
                ViewModelMain.WaitingTasks.Dequeue();
                await StartTask();
            }
        }

        public bool CanStartPendingTask()
        {
            if (ViewModelMain.WaitingTasks.Count > 0 &&
                ViewModelMain.RunningTasks.Count < 2 &&
                this == ViewModelMain.WaitingTasks.Peek())
            {
                return true;
            }

            return false;
        }
    }
}