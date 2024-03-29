﻿using CommunityToolkit.Mvvm.ComponentModel;
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
using System.ComponentModel.Design.Serialization;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Xml.Linq;
using MessageBox = System.Windows.Forms.MessageBox;

namespace OrganizationStructureClient.ViewModels
{
    public class MainViewModel : ObservableRecipient
    {
        #region Private Properties

        private IMessenger messenger;
        public Guid messageToken = Guid.NewGuid();

        private HttpClient _httpClient;
        private HubConnection _connectionHub;
        private ObservableCollection<PersonDTO> _personsTree = null;
        private PersonDTO _selectedPerson = null;

        private IAsyncRelayCommand _addPersonCommand;
        private IAsyncRelayCommand _editPersonCommand;
        private IAsyncRelayCommand _removePersonCommand;

        #endregion      

        #region Public Properties

        public string AssemblyVersion
        {
            get => $"ver. {GetType().Assembly.GetName().Version}";
        }

        public ObservableCollection<PersonDTO> PersonsTree
        {
            get => _personsTree;

            set => SetProperty(ref _personsTree, value);
        }

        public PersonDTO SelectedPerson
        {
            get => _selectedPerson;

            set
            {
                SetProperty(ref _selectedPerson, value);
                EditPersonCommand.NotifyCanExecuteChanged();
                RemovePersonCommand.NotifyCanExecuteChanged();
            }
        }

        private bool CanEditPerson()
        {
            return SelectedPerson != null;
        }

        private bool CanRemovePerson()
        {
            return SelectedPerson != null;
        }

        #endregion


        #region Constructor

        public MainViewModel()
        {
            messenger = Ioc.Default.GetService<IMessenger>();
            messageToken = Guid.NewGuid();

            Initialize();
        }

        #endregion

        #region Commands

        public IAsyncRelayCommand AddPersonCommand
        {
            get => _addPersonCommand ?? (_addPersonCommand =
                new AsyncRelayCommand(AddPersonAsync));
        }

        public IAsyncRelayCommand EditPersonCommand
        {
            get => _editPersonCommand ?? (_editPersonCommand =
                new AsyncRelayCommand(EditPersonAsync, CanEditPerson));
        }

        public IAsyncRelayCommand RemovePersonCommand
        {
            get => _removePersonCommand ?? (_removePersonCommand =
                new AsyncRelayCommand(RemovePersonAsync, CanRemovePerson));
        }

        #endregion

        #region Public Methods

        public async void Initialize()
        {
            try
            {
                var orgStrWSEndpoint = ConfigurationManager.AppSettings.Get("OrgStrWSEndpoint") ??
                    throw new ConfigurationErrorsException("WS Endpoint");

                _httpClient = new HttpClient { BaseAddress = new Uri(orgStrWSEndpoint) };

                _connectionHub = new HubConnectionBuilder()
                .WithUrl("https://localhost:7221/messageHub")
                .Build();

                _connectionHub.On("Refresh", new Action<string>(async (arg) =>
                {
                    switch (arg)
                    {
                        case "Persons":
                            await LoadPersons();
                            break;
                    }
                }));

                await _connectionHub.StartAsync();

                await LoadPersons();
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
                var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<PersonDTO>>>("api/Person/Get-Persons");

                if (response == null || response.Data == null) throw new Exception("Unable to load Persons.");

                PersonsTree = new ObservableCollection<PersonDTO>();

                CreateTree(response.Data);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "Load Persons Error",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task AddPersonAsync()
        {
            try
            {
                await Dispatcher.CurrentDispatcher.InvokeAsync(() => messenger.Send(new AddPersonMessage(_httpClient, _connectionHub), messageToken));
            }
            catch (Exception)
            {
                MessageBox.Show($"Unable to add person", "Add Person Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task EditPersonAsync()
        {
            try
            {
                await Dispatcher.CurrentDispatcher.InvokeAsync(() => messenger.Send(
                    new EditPersonMessage(_httpClient, _connectionHub, SelectedPerson), messageToken));
            }
            catch (Exception)
            {
                MessageBox.Show($"Unable to edit person", "Edit Person Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private async Task RemovePersonAsync()
        {
            try
            {
                var confirmation = MessageBox.Show($"Are you sure you want to permanently remove {SelectedPerson.FirstName} {SelectedPerson.LastName}?",
                    "Remove Person Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirmation != DialogResult.Yes) return;

                var result = await _httpClient.PostAsJsonAsync("api/Person/Delete-Person", SelectedPerson);

                var response = await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();

                if (response == null || !response.Success) throw new Exception($"Unable to remove {SelectedPerson.FirstName} {SelectedPerson.LastName}");
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}", "Remove Person Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Helpers

        private void CreateTree(List<PersonDTO> persons)
        {
            foreach (var employee in persons)
            {
                var employees = persons.Where(x => x.Manager?.Id == employee.Id);

                employee.Employees = new List<PersonDTO>(employees);
            }

            PersonsTree = new ObservableCollection<PersonDTO>(persons.Where(x => x.Manager == null));
        }

        #endregion
    }
}
