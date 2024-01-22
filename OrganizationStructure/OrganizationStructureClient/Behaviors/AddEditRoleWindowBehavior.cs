using OrganizationStructureClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OrganizationStructureClient.Behaviors
{
    public class AddEditRoleWindowBehavior : ModalWindowBehavior
    {
        public AddEditRoleViewModel AERVM
        {
            get => this.AssociatedObject.DataContext as AddEditRoleViewModel;
        }

        protected override void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            base.AssociatedObject_Loaded(sender, e);

        }

        protected override void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {

            base.AssociatedObject_Unloaded(sender, e);
        }
    }
}
