using CommunityToolkit.Mvvm.Messaging;
using OrganizationStructureClient.Messages.Messages;
using OrganizationStructureClient.ViewModels;
using OrganizationStructureClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OrganizationStructureClient.Behaviors
{
    public class AddEditPersonWindowBehavior : ModalWindowBehavior
    {
        public AddEditPersonViewModel AEPVM
        {
            get => this.AssociatedObject.DataContext as AddEditPersonViewModel;
        }

        public AddEditPersonWindow AEPW
        {
            get => this.AssociatedObject as AddEditPersonWindow;
        }

        protected override void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            base.AssociatedObject_Loaded(sender, e);
            if (AEPVM == null) return;

            messenger.Register<AddRoleMessage, Guid>(this, AEPVM.messageToken, (r, m) => AddRoleMessage_Handler(m));
            messenger.Register<EditRoleMessage, Guid>(this, AEPVM.messageToken, (r, m) => EditRoleMessage_Handler(m));

        }

        protected override void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            messenger.Unregister<AddRoleMessage, Guid>(this, AEPVM.messageToken);
            messenger.Unregister<EditRoleMessage, Guid>(this, AEPVM.messageToken);

            base.AssociatedObject_Unloaded(sender, e);
        }

        private void AddRoleMessage_Handler(AddRoleMessage msg)
        {

            AddEditRoleWindow addEditRoleWindow = new AddEditRoleWindow()
            {
                Owner = AEPW,
                DataContext = new AddEditRoleViewModel(msg.HttpClient)
            };

            addEditRoleWindow.ShowDialog();
        }

        private void EditRoleMessage_Handler(EditRoleMessage msg)
        {

            AddEditRoleWindow addEditRoleWindow = new AddEditRoleWindow()
            {
                Owner = AEPW,
                DataContext = new AddEditRoleViewModel(msg.HttpClient) { Role = msg.Role}
            };

            addEditRoleWindow.ShowDialog();
        }
    }
}
