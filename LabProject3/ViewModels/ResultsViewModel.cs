using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;

namespace LabProject3.ViewModels
{
    public class ResultsViewModel : INotifyPropertyChanged
    {
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

        public ICommand AddClassCommand { get; }
        public ICommand DeleteClassCommand { get; }
        public ICommand SaveJsonCommand { get; }
        public ICommand ReturnMainPage { get; }

        public ResultsViewModel()
        {
            AddClassCommand = new Command(async () => await NavigateToAddClassPage());
            DeleteClassCommand = new Command(DeleteSelectedClass, () => SelectedClass != null);
            SaveJsonCommand = new Command(SaveJson);
            ReturnMainPage = new Command(async () => await NavigateBack());
        }

        private UniversityClass _selectedClass;
        public UniversityClass SelectedClass
        {
            get => _selectedClass;
            set
            {
                if (_selectedClass != value)
                {
                    _selectedClass = value;
                    OnPropertyChanged();
                    (DeleteClassCommand as Command)?.ChangeCanExecute();
                }
            }
        }

        private async Task NavigateToAddClassPage()
        {
            var addClassPage = new AddClassPage
            {
                BindingContext = new AddClassViewModel(OnNewClassAdded)
            };

            try
            {
                await Application.Current.MainPage.Navigation.PushAsync(addClassPage);
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Navigation Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private void OnNewClassAdded(UniversityClass newClass)
        {
            if (newClass != null)
            {
                Classes.Add(newClass);
            }
        }

        private async Task NavigateBack()
        {
            try
            {
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Navigation Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private void DeleteSelectedClass()
        {
            if (SelectedClass != null)
            {
                Classes.Remove(SelectedClass);
                SelectedClass = null;
            }
        }

        private void SaveJson()
        {
            try
            {
                string projectPath = AppContext.BaseDirectory;

                for (int i = 0; i < 6; i++)
                {
                    projectPath = Path.GetDirectoryName(projectPath);
                }

                string fileName = "search_results.json";
                string filePath = Path.Combine(projectPath, fileName);

                var json = JsonSerializer.Serialize(Classes, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                File.WriteAllText(filePath, json);

                App.Current.MainPage.DisplayAlert("Success", "Data successfully saved to search_results.json", "OK");
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Error", $"Failed to save JSON: {ex.Message}", "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}




