using CommunityToolkit.Mvvm.ComponentModel;
using OrganizationStructureShared.Models;
using OrganizationStructureShared.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design.Serialization;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Linq;
using MessageBox = System.Windows.Forms.MessageBox;

namespace OrganizationStructureClient.ViewModels
{
    public class MainViewModel : ObservableRecipient
    {
        #region Private Properties

        private HttpClient _httpClient;
        private ObservableCollection<PersonDTO> _personsTree = null;

        #endregion

        #region Constructor

        public MainViewModel()
        {
            Initialize();
        }

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

        #endregion

        #region Commands

        #endregion

        #region Public Methods

        public async void Initialize()
        {
            try
            {
                var orgStrWSEndpoint = ConfigurationManager.AppSettings.Get("OrgStrWSEndpoint") ??
                    throw new ConfigurationErrorsException("WS Endpoint");

                _httpClient = new HttpClient { BaseAddress = new Uri(orgStrWSEndpoint) };

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
