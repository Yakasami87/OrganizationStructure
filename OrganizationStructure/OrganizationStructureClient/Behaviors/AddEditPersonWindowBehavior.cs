using OrganizationStructureClient.ViewModels;
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
