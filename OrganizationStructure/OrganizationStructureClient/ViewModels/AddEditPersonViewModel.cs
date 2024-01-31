using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.VisualBasic.ApplicationServices;
using OrganizationStructureClient.Messages.Messages;
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
using System.Windows.Threading;
using System.Xml.Linq;

namespace OrganizationStructureClient.ViewModels
{
    public class AddEditPersonViewModel : BaseModalViewModel
    {
        #region Private Properties

        private IMessenger messenger;
        public Guid messageToken = Guid.NewGuid();

        private bool _hasChanges = false;    

        private ObservableCollection<PersonDTO> _persons = null;
        private ObservableCollection<RoleDTO> _roles = null;

        private IAsyncRelayCommand _addRoleCommand;
        private IAsyncRelayCommand _editRoleCommand;
        private IAsyncRelayCommand _removeRoleCommand;


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
                if(Person.Id == 0)
                {
                    var result = await HttpClient.PostAsJsonAsync("api/Person/Create-Person", Person);
                    
                    var response = await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();

                    if (response == null || !response.Success) throw new Exception($"Unable to create {Person.FirstName} {Person.LastName}");
                }
                else
                {
                    var result = await HttpClient.PostAsJsonAsync("api/Person/Update-Person", Person);

                    var response = await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();

                    if (response == null || !response.Success) throw new Exception($"Unable to update {Person.FirstName} {Person.LastName}");
                }

                await base.Confirm();

                HasChanges = false;

            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}", "Save Person Informations Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Public Properties

        public string AssemblyVersion
        {
            get => $"ver. {GetType().Assembly.GetName().Version}";
        }
        public HttpClient HttpClient { get; set; }
        public HubConnection ConnectionHub { get; set; }
        public PersonDTO Person { get; set; } = new PersonDTO();

        public bool HasChanges
        {
            get => _hasChanges;

            set
            {
                SetProperty(ref _hasChanges, value);
                EditRoleCommand.NotifyCanExecuteChanged();
                RemoveRoleCommand.NotifyCanExecuteChanged();
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

        private bool CanEditRole()
        {
            return Role != null;
        }

        private bool CanRemoveRole()
        {
            return Role != null;
        }

        #endregion

        #region Constructor

        public AddEditPersonViewModel()
        {

        }

        public AddEditPersonViewModel(HttpClient httpClient, HubConnection connectionHub)
        {
            try
            {
                messenger = Ioc.Default.GetService<IMessenger>();

                HttpClient = httpClient;

                ConnectionHub = connectionHub;

                Initialize();

            }
            catch (Exception e)
            {
                MessageBox.Show($"{e}", "Load settings Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                App.Current.MainWindow.Close();
            }
        }
        #endregion

        #region Commands

        public IAsyncRelayCommand AddRoleCommand
        {
            get => _addRoleCommand ?? (_addRoleCommand =
                new AsyncRelayCommand(AddRoleAsync));
        }

        public IAsyncRelayCommand EditRoleCommand
        {
            get => _editRoleCommand ?? (_editRoleCommand =
                new AsyncRelayCommand(EditRoleAsync, CanEditRole));
        }

        public IAsyncRelayCommand RemoveRoleCommand
        {
            get => _removeRoleCommand ?? (_removeRoleCommand =
                new AsyncRelayCommand(RemoveRoleAsync, CanRemoveRole));
        }

        #endregion

        #region Public Methods

        public async void Initialize()
        {
            try
            {
                ConnectionHub.On("Refresh", new Action<string>(async (arg) =>
                {
                    switch (arg)
                    {
                        case "Persons":
                            await LoadPersons();
                            break;
                        case "Roles":
                            await LoadRoles();
                            break;
                    }
                }));

                await LoadPersons();
                await LoadRoles();

                Manager = Person.Manager == null ? Persons.First() : Persons.First(x => x?.Id == Person.Manager?.Id);
                Role = Person.Role == null ? Roles.First() : Roles.First(x => x?.Id == Person.Role?.Id);
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
                Roles.Insert(0, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "Load Roles Error",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task AddRoleAsync()
        {
            try
            {
                await Dispatcher.CurrentDispatcher.InvokeAsync(() => messenger.Send(new AddRoleMessage(HttpClient), messageToken));
            }
            catch (Exception)
            {
                MessageBox.Show($"Unable to add role", "Add Role Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task EditRoleAsync()
        {
            try
            {
                await Dispatcher.CurrentDispatcher.InvokeAsync(() => messenger.Send(
                    new EditRoleMessage(HttpClient, Role), messageToken));
            }
            catch (Exception)
            {
                MessageBox.Show($"Unable to edit role", "Edit Role Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private async Task RemoveRoleAsync()
        {
            try
            {
                var confirmation = MessageBox.Show($"Are you sure you want to permanently remove {Role.Name}?",
                    "Remove Role Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirmation != DialogResult.Yes) return;

                var result = await HttpClient.PostAsJsonAsync("api/Role/Delete-Role", Role);

                var response = await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();

                if (response == null || !response.Success) throw new Exception($"Unable to remove {Role.Name}");
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}", "Remove Person Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
