using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OrganizationStructureClient.Messages.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace OrganizationStructureClient.ViewModels
{
    public class BaseModalViewModel : ObservableRecipient, IDataErrorInfo
    {
        private IMessenger messenger;
        private bool _hasError;
        public Dictionary<string, string> ErrorCollection { get; private set; } = new Dictionary<string, string>();

        private IAsyncRelayCommand _closeCommand;
        private IAsyncRelayCommand _confirmCommand;

        public bool HasError
        {
            get => _hasError;

            set => SetProperty(ref _hasError, value);
        }

        public string Error
        {
            get => null;
        }

        public BaseModalViewModel()
        {
            messenger = Ioc.Default.GetService<IMessenger>();
        }

        public IAsyncRelayCommand CloseCommand
        {
            get => _closeCommand ?? (_closeCommand =
                    new AsyncRelayCommand(async () =>
                    {
                        await Dispatcher.CurrentDispatcher.InvokeAsync(() => messenger.Send(new CloseMessage()));
                    }));
        }

        public IAsyncRelayCommand ConfirmCommand
        {
            get => _confirmCommand ?? (_confirmCommand =
                    new AsyncRelayCommand(Confirm, CanConfirm));
        }

        protected virtual async Task Confirm()
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() => messenger.Send(new ConfirmMessage()));
        }

        protected virtual bool CanConfirm()
        {
            return true;
        }


        public virtual string this[string columnName]
        {
            get => string.Empty;
        }
    }
}
