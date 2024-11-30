using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LabProject3.ViewModels
{
    public class AddClassViewModel : INotifyPropertyChanged
    {
        private string _subject;
        private string _groupNumber;
        private string _specialization;
        private string _lecturer;
        private string _course;
        private string _schedule;

        public string Subject
        {
            get => _subject;
            set => SetProperty(ref _subject, value);
        }

        public string GroupNumber
        {
            get => _groupNumber;
            set => SetProperty(ref _groupNumber, value);
        }

        public string Specialization
        {
            get => _specialization;
            set => SetProperty(ref _specialization, value);
        }

        public string Lecturer
        {
            get => _lecturer;
            set => SetProperty(ref _lecturer, value);
        }

        public string Course
        {
            get => _course;
            set => SetProperty(ref _course, value);
        }

        public string Schedule
        {
            get => _schedule;
            set => SetProperty(ref _schedule, value);
        }

        public ICommand AddCommand { get; }
        public ICommand CancelCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Action<UniversityClass> _onClassAdded;

        public AddClassViewModel(Action<UniversityClass> onClassAdded)
        {
            _onClassAdded = onClassAdded;

            AddCommand = new Command(async () => await AddClass());
            CancelCommand = new Command(async () => await Cancel());
        }

        private async Task AddClass()
        {
            if (string.IsNullOrWhiteSpace(Subject) || string.IsNullOrWhiteSpace(GroupNumber) ||
                string.IsNullOrWhiteSpace(Specialization) || string.IsNullOrWhiteSpace(Lecturer) ||
                string.IsNullOrWhiteSpace(Course) || string.IsNullOrWhiteSpace(Schedule))
            {
                await App.Current.MainPage.DisplayAlert("Error", "All fields are required.", "OK");
                return;
            }

            if (!int.TryParse(Course, out int courseNumber))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Course must be a number.", "OK");
                return;
            }

            var scheduleList = Schedule.Split(',').Select(s => s.Trim()).ToList();
            var newClass = new UniversityClass(Subject, scheduleList, GroupNumber, Specialization, courseNumber, Lecturer);

            _onClassAdded?.Invoke(newClass);
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async Task Cancel()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        private void SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(backingField, value))
            {
                backingField = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
