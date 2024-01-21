using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Xaml.Behaviors;
using OrganizationStructureClient.Messages.Messages;
using OrganizationStructureClient.ViewModels;
using OrganizationStructureClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationStructureClient.Behaviors
{
    public class MainWindowBehavior : Behavior<MainWindow>
    {
        private IMessenger messenger;
        private MainViewModel MVM
        {
            get => this.AssociatedObject.DataContext as MainViewModel;
        }

        public MainWindow MW
        {
            get => this.AssociatedObject as MainWindow;
        }

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

        private void AssociatedObject_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MVM == null) return;

            messenger.Register<AddPersonMessage, Guid>(this, MVM.messageToken, (r, m) => AddPersonMessage_Handler(m));

        }

        private void AssociatedObject_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            messenger.Unregister<AddPersonMessage, Guid>(this, MVM.messageToken);

            this.Detach();
        }

        private void AddPersonMessage_Handler(AddPersonMessage msg)
        {

            AddEditPersonWindow addEditPersonWindow = new AddEditPersonWindow()
            {
                Owner = MW,
                DataContext = new AddEditPersonViewModel(msg.HttpClient)
            };

            addEditPersonWindow.ShowDialog();
        }
    }
}
