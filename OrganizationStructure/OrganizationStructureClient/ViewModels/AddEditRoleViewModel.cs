using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using OrganizationStructureShared.Models;
using OrganizationStructureShared.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationStructureClient.ViewModels
{
    public  class AddEditRoleViewModel : BaseModalViewModel
    {
        #region Private Properties

        private IMessenger messenger;
        public Guid messageToken = Guid.NewGuid();

        private bool _hasChanges = false;

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
                if (Role.Id == 0)
                {
                    var result = await HttpClient.PostAsJsonAsync("api/Role/Create-Role", Role);

                    var response = await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();

                    if (response == null || !response.Success) throw new Exception($"Unable to create {Role.Name}");
                }
                else
                {
                    var result = await HttpClient.PostAsJsonAsync("api/Role/Update-Role", Role);

                    var response = await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();

                    if (response == null || !response.Success) throw new Exception($"Unable to update {Role.Name}");
                }

                await base.Confirm();

                HasChanges = false;

            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}", "Save Role Informations Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Public Properties

        public string AssemblyVersion
        {
            get => $"ver. {GetType().Assembly.GetName().Version}";
        }
        public HttpClient HttpClient { get; set; }
        public RoleDTO Role { get; set; } = new RoleDTO();

        public bool HasChanges
        {
            get => _hasChanges;

            set
            {
                SetProperty(ref _hasChanges, value);
                ConfirmCommand.NotifyCanExecuteChanged();
            }
        }

        public string Name
        {
            get => Role.Name;

            set => HasChanges = SetProperty(Role.Name, value, Role, (r, n) => r.Name = n);
        }

        #endregion

        #region Constructor

        public AddEditRoleViewModel()
        {

        }

        public AddEditRoleViewModel(HttpClient httpClient)
        {
            try
            {
                messenger = Ioc.Default.GetService<IMessenger>();

                HttpClient = httpClient;

            }
            catch (Exception e)
            {
                MessageBox.Show($"{e}", "Load settings Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                App.Current.MainWindow.Close();
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
                    case "Name":

                        if (string.IsNullOrWhiteSpace(Name))
                        {
                            result = $"Please insert a valid Name";
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
