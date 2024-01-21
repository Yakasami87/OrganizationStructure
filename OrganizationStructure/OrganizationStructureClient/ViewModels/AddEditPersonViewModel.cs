using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.VisualBasic.ApplicationServices;
using OrganizationStructureShared.Models;
using OrganizationStructureShared.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OrganizationStructureClient.ViewModels
{
    public class AddEditPersonViewModel : BaseModalViewModel
    {
        #region Private Properties

        private IMessenger messenger;
        public Guid messageToken;

        private bool _hasChanges = false;    

        private ObservableCollection<PersonDTO> _persons = null;
        private ObservableCollection<RoleDTO> _roles = null;

        #endregion

        #region Protected Properties

        protected override bool CanConfirm()
        {
            return _hasChanges && !HasError;
        }

        protected override async Task Confirm()
        {
            try
            {              
                var response = await HttpClient.PostAsJsonAsync("api/Person/Create-Person", Person);

                await base.Confirm();

                HasChanges = false;

            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}", "Save User Informations Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Public Properties

        public string AssemblyVersion
        {
            get => $"ver. {GetType().Assembly.GetName().Version}";
        }
        public HttpClient HttpClient { get; set; }
        public PersonDTO Person { get; set; } = new PersonDTO();

        public bool HasChanges
        {
            get => _hasChanges;

            set
            {
                SetProperty(ref _hasChanges, value);
                ConfirmCommand.NotifyCanExecuteChanged();
            }
        }

        public string FirstName
        {
            get => Person.FirstName;

            set => HasChanges = SetProperty(Person.FirstName, value, Person, (p, fn) => p.FirstName = fn);
        }

        public string LastName
        {
            get => Person.LastName;

            set => HasChanges = SetProperty(Person.LastName, value, Person, (p, ln) => p.LastName = ln);
    }

        public PersonDTO Manager
        {
            get => Person.Manager;

            set => HasChanges = SetProperty(Person.Manager, value, Person, (p, m) => p.Manager = m);
        }

        public RoleDTO Role
        {
            get => Person.Role;

            set => HasChanges = SetProperty(Person.Role, value, Person, (p, r) => p.Role = r);
        }

        public ObservableCollection<PersonDTO> Persons
        {
            get => _persons;

            set => SetProperty(ref _persons, value);
        }

        public ObservableCollection<RoleDTO> Roles
        {
            get => _roles;

            set => SetProperty(ref _roles, value);
        }

        #endregion

        #region Constructor

        public AddEditPersonViewModel()
        {

        }

        public AddEditPersonViewModel(HttpClient httpClient)
        {
            try
            {
                messenger = Ioc.Default.GetService<IMessenger>();

                HttpClient = httpClient;

                Initialize();

            }
            catch (Exception e)
            {
                MessageBox.Show($"{e}", "Load settings Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                App.Current.MainWindow.Close();
            }
        }
        #endregion

        #region Public Methods

        public async void Initialize()
        {
            try
            {
                await LoadPersons();
                await LoadRoles();

                Manager = Person.Manager == null ? Persons.First() : Person.Manager;
                Role = Person.Role == null ? Roles.First() : Person.Role;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to initialize components {ex}.", "Initilize components Error",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                App.Current.MainWindow.Close();
            }
        }

        #endregion

        #region Private Methods

        private async Task LoadPersons()
        {
            try
            {
                var response = await HttpClient.GetFromJsonAsync<ServiceResponse<List<PersonDTO>>>("api/Person/Get-Persons");

                if (response == null || response.Data == null) throw new Exception("Unable to load Persons.");

                Persons = new ObservableCollection<PersonDTO>(response.Data);
                Persons.Insert(0, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "Load Persons Error",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadRoles()
        {
            try
            {
                var response = await HttpClient.GetFromJsonAsync<ServiceResponse<List<RoleDTO>>>("api/Role/Get-Roles");

                if (response == null || response.Data == null) throw new Exception("Unable to load Roles.");

                Roles = new ObservableCollection<RoleDTO>(response.Data);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "Load Roles Error",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Helpers

        public override string this[string columnName]
        {
            get
            {
                var result = string.Empty;

                switch (columnName)
                {
                    case "FirstName":

                        if (string.IsNullOrWhiteSpace(FirstName))
                        {
                            result = $"Please insert a valid First Name";
                        }
                        break;

                    case "LastName":

                        if (string.IsNullOrWhiteSpace(LastName))
                        {
                            result = $"Please insert a valid Last Name";
                        }

                        break;
                }

                if (ErrorCollection.ContainsKey(columnName))
                {
                    ErrorCollection[columnName] = result;
                }
                else if (result != null)
                {
                    ErrorCollection.Add(columnName, result);
                }

                HasError = ErrorCollection.Values.Any(x => x != string.Empty);

                OnPropertyChanged(nameof(ErrorCollection));
                ConfirmCommand.NotifyCanExecuteChanged();

                return result;
            }
        }
        #endregion

    }
}
