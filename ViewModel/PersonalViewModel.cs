using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Module07DataAccess.Services;
using Module07DataAccess.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Module07DataAccess.ViewModel
{
    public class PersonalViewModel : INotifyPropertyChanged
    {
        private readonly PersonalService _personalService;

        public ObservableCollection<Personal> PersonalList { get; set; }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        private Personal _selectedEmployee;
        public Personal SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                if (_selectedEmployee != null)
                {
                    NewPersonalName = _selectedEmployee.Name;
                    NewPersonalemail = _selectedEmployee.email;
                    NewPersonalContactNo = _selectedEmployee.ContactNo;
                    NewPersonalAddress = _selectedEmployee.Address;  // New property for Address
                    IsPersonSelected = true;
                }
                else
                {
                    IsPersonSelected = false;
                }
                OnPropertyChanged();
            }
        }

        private bool _isPersonSelected;
        public bool IsPersonSelected
        {
            get => _isPersonSelected;
            set
            {
                _isPersonSelected = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        // New Personal entry for name, email, contact no, and address
        private string _newPersonalName;
        public string NewPersonalName
        {
            get => _newPersonalName;
            set
            {
                _newPersonalName = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonalemail;
        public string NewPersonalemail
        {
            get => _newPersonalemail;
            set
            {
                _newPersonalemail = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonalContactNo;
        public string NewPersonalContactNo
        {
            get => _newPersonalContactNo;
            set
            {
                _newPersonalContactNo = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonalAddress;  // New property for Address
        public string NewPersonalAddress
        {
            get => _newPersonalAddress;
            set
            {
                _newPersonalAddress = value;
                OnPropertyChanged();
            }
        }

        // Search keyword property
        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged();
                FilterPersonalList();
            }
        }

        public ICommand LoadDataCommand { get; }
        public ICommand AddPersonalCommand { get; }
        public ICommand SelectedEmployeeCommand { get; }
        public ICommand DeletePersonCommand { get; }
        public ICommand UpdatePersonCommand { get; }

        // Personal ViewModel Constructor
        public PersonalViewModel()
        {
            _personalService = new PersonalService();
            PersonalList = new ObservableCollection<Personal>();
            LoadDataCommand = new Command(async () => await LoadData());
            AddPersonalCommand = new Command(async () => await AddPerson());
            SelectedEmployeeCommand = new Command<Personal>(person => SelectedEmployee = person);
            DeletePersonCommand = new Command(async () => await DeletePersonal(), () => SelectedEmployee != null);
            UpdatePersonCommand = new Command(async () => await UpdatePerson(), () => SelectedEmployee != null);

            LoadData();
        }

        // Load data from the service
        public async Task LoadData()
        {
            if (IsBusy) return;
            IsBusy = true;
            StatusMessage = "Loading personal data...";
            try
            {
                var personals = await _personalService.GetAllEmployeeAsync();
                PersonalList.Clear();
                foreach (var personal in personals)
                {
                    PersonalList.Add(personal);
                }
                StatusMessage = "Data loaded successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Add new person
        private async Task AddPerson()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(NewPersonalName) || string.IsNullOrWhiteSpace(NewPersonalemail) ||
                string.IsNullOrWhiteSpace(NewPersonalContactNo) || string.IsNullOrWhiteSpace(NewPersonalAddress))  // Check for Address
            {
                StatusMessage = "Please fill in all the fields before adding";
                return;
            }
            IsBusy = true;
            StatusMessage = "Adding new person...";

            try
            {
                var newPerson = new Personal
                {
                    Name = NewPersonalName,
                    email = NewPersonalemail,
                    ContactNo = NewPersonalContactNo,
                    Address = NewPersonalAddress  // Include Address in the new person
                };
                var isSuccess = await _personalService.InsertEmployeeAsync(newPerson);
                if (isSuccess)
                {
                    NewPersonalName = string.Empty;
                    NewPersonalemail = string.Empty;
                    NewPersonalContactNo = string.Empty;
                    NewPersonalAddress = string.Empty;  // Clear Address field
                    StatusMessage = "New person added successfully";
                }
                else
                {
                    StatusMessage = "Failed to add the new person";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to add the new person: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
                await LoadData();
            }
        }

        // Delete person
        private async Task DeletePersonal()
        {
            if (SelectedEmployee == null) return;
            var answer = await Application.Current.MainPage.DisplayAlert("Confirm Delete",
                $"Are you sure you want to delete {SelectedEmployee.Name}?", "Yes", "No");
            if (!answer) return;
            IsBusy = true;
            StatusMessage = "Deleting person...";

            try
            {
                var success = await _personalService.DeleteEmployeeAsync(SelectedEmployee.EmployeeID);
                StatusMessage = success ? "Person deleted successfully!" : "Failed to delete person";

                if (success)
                {
                    PersonalList.Remove(SelectedEmployee);
                    SelectedEmployee = null;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting person: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Update person
        private async Task UpdatePerson()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(NewPersonalName) || string.IsNullOrWhiteSpace(NewPersonalemail) ||
                string.IsNullOrWhiteSpace(NewPersonalContactNo) || string.IsNullOrWhiteSpace(NewPersonalAddress))  // Check for Address
            {
                StatusMessage = "Please fill in all the fields before updating";
                return;
            }

            IsBusy = true;
            StatusMessage = "Updating person...";

            try
            {
                var updatedPerson = new Personal
                {
                    EmployeeID = SelectedEmployee.EmployeeID,
                    Name = NewPersonalName,
                    email = NewPersonalemail,
                    ContactNo = NewPersonalContactNo,
                    Address = NewPersonalAddress  // Include Address in the update
                };

                var isSuccess = await _personalService.UpdateEmployeeAsync(updatedPerson);
                if (isSuccess)
                {
                    StatusMessage = "Person updated successfully";
                    await LoadData(); // Refresh the list after update
                }
                else
                {
                    StatusMessage = "Failed to update the person";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to update the person: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Filter list based on search keyword
        private void FilterPersonalList()
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword))
            {
                LoadData(); // Reload all data if no search keyword
                return;
            }

            var filteredList = PersonalList.Where(p => p.Name.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                                        p.email.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                                                        p.ContactNo.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase))
                                           .ToList();

            PersonalList.Clear();
            foreach (var personal in filteredList)
            {
                PersonalList.Add(personal);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
