using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;

namespace LabProject3.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly LINQSearch _linqSearch = new LINQSearch();
        private bool _isError = false;

        public ObservableCollection<string> SubjectOptions { get; } = new();
        public ObservableCollection<string> GroupNumberOptions { get; } = new();
        public ObservableCollection<string> SpecializationOptions { get; } = new();
        public ObservableCollection<string> LecturerOptions { get; } = new();
        public ObservableCollection<string> CourseOptions { get; } = new();
        public ObservableCollection<string> DayOptions { get; } = new();

        private ObservableCollection<UniversityClass> _classes = new();
        public ObservableCollection<UniversityClass> Classes
        {
            get => _classes;
            set
            {
                if (_classes != value)
                {
                    _classes = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<UniversityClass> SearchResults { get; } = new();

        private List<UniversityClass> _allClasses = new();

        private readonly Dictionary<string, ObservableCollection<string>> _pickersData;

        private UniversityClass _selectedClass;
        public UniversityClass SelectedClass
        {
            get => _selectedClass;
            set
            {
                SetProperty(ref _selectedClass, value);
                (DeleteClassCommand as Command)?.ChangeCanExecute();
            }
        }

        private string _selectedSubject;
        public string SelectedSubject
        {
            get => _selectedSubject;
            set => SetProperty(ref _selectedSubject, value);
        }

        private string _selectedGroupNumber;
        public string SelectedGroupNumber
        {
            get => _selectedGroupNumber;
            set => SetProperty(ref _selectedGroupNumber, value);
        }

        private string _selectedSpecialization;
        public string SelectedSpecialization
        {
            get => _selectedSpecialization;
            set => SetProperty(ref _selectedSpecialization, value);
        }

        private string _selectedLecturer;
        public string SelectedLecturer
        {
            get => _selectedLecturer;
            set => SetProperty(ref _selectedLecturer, value);
        }

        private string _selectedCourse;
        public string SelectedCourse
        {
            get => _selectedCourse;
            set => SetProperty(ref _selectedCourse, value);
        }

        private string _selectedDay;
        public string SelectedDay
        {
            get => _selectedDay;
            set => SetProperty(ref _selectedDay, value);
        }

        public ICommand SelectFileCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ShowResultsCommand { get; }
        public ICommand AddClassCommand { get; }
        public ICommand DeleteClassCommand { get; }
        public ICommand SaveJsonCommand { get; }
        public ICommand ShowJsonCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand ShowInfoCommand { get; }

        public MainViewModel()
        {
            _pickersData = new Dictionary<string, ObservableCollection<string>>
            {
                { "Subject", SubjectOptions },
                { "GroupNumber", GroupNumberOptions },
                { "Specialization", SpecializationOptions },
                { "Lecturer", LecturerOptions },
                { "Course", CourseOptions },
                { "Schedule", DayOptions }
            };


            SelectFileCommand = new Command(async () => await SelectJsonFile());
            ClearCommand = new Command(ClearSelections);
            SearchCommand = new Command(PerformSearch);
            ShowResultsCommand = new Command(() => ShowResultsPage(SearchResults), () => SearchResults.Any());
            AddClassCommand = new Command(AddNewClass);
            DeleteClassCommand = new Command(DeleteSelectedClass, () => SelectedClass != null);
            SaveJsonCommand = new Command(SaveJson);
            ShowJsonCommand = new Command(() => ShowJsonData());
            ExitCommand = new Command(async () => await Exit()); 
            ShowInfoCommand = new Command(() => DisplayLabInfo()); 
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async Task SelectJsonFile()
        {
            _isError = false;

            try
            {
                var customJsonFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/json" } },
                    { DevicePlatform.iOS, new[] { "public.json" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.json" } },
                    { DevicePlatform.WinUI, new[] { ".json" } }
                });

                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a JSON File",
                    FileTypes = customJsonFileType
                });

                if (result == null) return; 

                var filePath = result.FullPath;

                if (!filePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    _isError = true;
                    await DisplayError("Selected file is not a valid JSON file.");
                    return;
                }

                var loadedClasses = JsonUtils.Deserialize(filePath);
                if (loadedClasses == null || !loadedClasses.Any())
                {
                    await DisplayError("The JSON file contains no valid data.");
                    return;
                }

                _allClasses = loadedClasses;
                Classes = new ObservableCollection<UniversityClass>(_allClasses);
                PopulateDropdownOptions(_allClasses);
            }
            catch (Exception ex)
            {
                await DisplayError($"An error occurred: {ex.Message}");
            }
        }

        private void PerformSearch()
        {
            var criterias = new List<string>
            {
                SelectedSubject,
                SelectedGroupNumber,
                SelectedSpecialization,
                SelectedLecturer,
                SelectedCourse,
                SelectedDay
            }.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

            if (!criterias.Any())
            {
                SearchResults.Clear();
                App.Current.MainPage.DisplayAlert("No Criteria", "Please select at least one criterion for the search.", "OK");
                return;
            }

            var results = _linqSearch.Search(criterias, _allClasses);
            SearchResults.Clear();
            foreach (var result in results)
            {
                SearchResults.Add(result);
            }

            ShowResultsPage(SearchResults);
            (ShowResultsCommand as Command)?.ChangeCanExecute();
        }

        private void ShowResultsPage(ObservableCollection<UniversityClass> classesToShow)
        {
            var resultsViewModel = new ResultsViewModel
            {
                Classes = classesToShow
            };

            var resultsPage = new ResultsPage
            {
                BindingContext = resultsViewModel
            };

            Application.Current.MainPage.Navigation.PushAsync(resultsPage);
        }

        private void ShowJsonData()
        {
            ShowResultsPage(new ObservableCollection<UniversityClass>(_allClasses));
        }

        private void ClearSelections()
        {
            SelectedSubject = null;
            SelectedGroupNumber = null;
            SelectedSpecialization = null;
            SelectedLecturer = null;
            SelectedCourse = null;
            SelectedDay = null;
            SearchResults.Clear();
            (ShowResultsCommand as Command)?.ChangeCanExecute();
        }

        private void AddNewClass()
        {
            var newClass = new UniversityClass("New Subject", new List<string> { "Monday" }, "New Group", "New Specialization", 1, "New Lecturer");

            Classes.Add(newClass);
            _allClasses.Add(newClass);
        }

        private void DeleteSelectedClass()
        {
            if (SelectedClass != null)
            {
                Classes.Remove(SelectedClass);
                _allClasses.Remove(SelectedClass);
            }
        }

        private void SaveJson()
        {
            var path = "saved_data.json";
            JsonUtils.Serialize(path, _allClasses);
            App.Current.MainPage.DisplayAlert("Success", $"Data successfully saved to {path}", "OK");
        }

        private async Task DisplayError(string message)
        {
            await App.Current.MainPage.DisplayAlert("Error", message, "OK");
        }

        private void PopulateDropdownOptions(List<UniversityClass> classes)
        {
            foreach (var picker in _pickersData.Values)
            {
                picker.Clear();
            }

            foreach (var uniClass in classes)
            {
                AddPickerValue(uniClass);
            }
        }

        private void AddPickerValue(UniversityClass uniClass)
        {
            string[] selectedProperties = { "Subject", "GroupNumber", "Specialization", "Lecturer", "Course" };

            foreach (var propertyName in selectedProperties)
            {
                if (_pickersData.TryGetValue(propertyName, out var pickerList))
                {
                    var property = uniClass.GetType().GetProperty(propertyName);
                    var value = property?.GetValue(uniClass)?.ToString();
                    if (!string.IsNullOrEmpty(value) && !pickerList.Contains(value))
                    {
                        pickerList.Add(value);
                    }
                }
            }

            foreach (var day in uniClass.Schedule ?? new List<string>())
            {
                if (!DayOptions.Contains(day))
                {
                    DayOptions.Add(day);
                }
            }
        }

        private void DisplayLabInfo()
        {
            App.Current.MainPage.DisplayAlert(
                "Laboratory Information",
                "Lab Owner: Anastasiia Shulhina\nGroup: K-25\nLab Name: JSON Search",
                "OK");
        }

        private async Task Exit()
        {
            var result = await Application.Current.MainPage.DisplayAlert(
                "Exit",
                "Do you really want to exit the application?",
                "Yes",
                "No");

            if (result)
            {
                Application.Current.Quit();
            }
        }

        private void SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(backingField, value))
            {
                backingField = value;
                OnPropertyChanged(propertyName);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
