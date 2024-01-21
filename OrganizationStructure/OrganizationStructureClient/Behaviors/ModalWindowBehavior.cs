using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Xaml.Behaviors;
using OrganizationStructureClient.Messages.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OrganizationStructureClient.Behaviors
{
    public class ModalWindowBehavior : Behavior<Window>
    {
        private IMessenger messenger;

        protected override void OnAttached()
        {
            base.OnAttached();

            messenger = Ioc.Default.GetService<IMessenger>();

            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
            this.AssociatedObject.Unloaded += AssociatedObject_Unloaded;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.Loaded -= AssociatedObject_Loaded;
            this.AssociatedObject.Unloaded -= AssociatedObject_Unloaded;

            base.OnDetaching();
        }

        protected virtual void AssociatedObject_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            messenger.Register<ConfirmMessage>(this, (r, m) => ConfirmMessage_Handler(m));
            messenger.Register<CloseMessage>(this, (r, m) => CloseMessage_Handler(m));
        }

        protected virtual void AssociatedObject_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            messenger.Unregister<ConfirmMessage>(this);
            messenger.Unregister<CloseMessage>(this);

            this.Detach();
        }

        private void CloseMessage_Handler(CloseMessage msg)
        {
            this.AssociatedObject.DialogResult = false;
            this.AssociatedObject.Close();
        }

        private void ConfirmMessage_Handler(ConfirmMessage msg)
        {
            this.AssociatedObject.DialogResult = true;
            this.AssociatedObject.Close();
        }
    }
}
