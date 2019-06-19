﻿using System.ComponentModel;
using System.Windows;

using ExampleContracts.Models;
using ExampleContracts.Requests;
using ExampleContracts.Topics;

using Pigeon;
using Pigeon.Addresses;
using Pigeon.NetMQ;
using Pigeon.Unity;

using Unity;

namespace ExampleClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private UnityContainer container;
        private Router router;
        private ViewModel viewModel;
        

        public MainWindow()
        {
            container = new UnityContainer();
            container.RegisterSingleton<ViewModel>();
            
            router =
                UnityBuilder
                    .FromContainer(container)
                    .WithName("ExampleClient")
                    .WithTransport<NetMQTransport>(t =>
                    {
                        t.WithSender(TcpAddress.Localhost(5555))
                            .For<UserConnecting>()
                            .For<UserDisconecting>()
                            .For<Message>();

                        t.WithSubscriber(TcpAddress.Localhost(5556))
                            .Handles<UserConnected>()
                            .Handles<UserDisconnected>()
                            .Handles<Message>();
                    })
                    .BuildAndStart();

            DataContext = viewModel = container.Resolve<ViewModel>();

            InitializeComponent();
        }


        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            await viewModel.Connect();
        }


        private async void InputButton_Click(object sender, RoutedEventArgs e)
        {
            await viewModel.PostMessage();
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            router.Stop();
        }
    }
}
