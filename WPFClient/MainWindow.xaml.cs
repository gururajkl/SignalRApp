using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace WPFClient
{
    public partial class MainWindow : Window
    {
        HubConnection hubConnection;

        public MainWindow()
        {
            InitializeComponent();
            hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7063/chathub")
                .WithAutomaticReconnect()
                .Build();

            // If the client has lost connection and trying to reconnect.
            hubConnection.Reconnecting += (sender) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var newMessage = "Attempting to reconnect...";
                    messages.Items.Add(newMessage);
                });

                return Task.CompletedTask;
            };

            // If the connection is back and it's connected.
            hubConnection.Reconnected += (sender) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var newMessage = "Reconnected to the server..";
                    messages.Items.Clear();
                    messages.Items.Add(newMessage);
                });

                return Task.CompletedTask;
            };

            // If the connection is ended.
            hubConnection.Closed += (sender) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var newMessage = "Connection Closed";
                    messages.Items.Add(newMessage);
                    sendMessage.IsEnabled = false;
                    openConnection.IsEnabled = true;
                });

                return Task.CompletedTask;
            };
        }

        private void openConnection_Click(object sender, RoutedEventArgs e)
        {
            hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    messages.Items.Add($"{user}: {message}");
                });
            });

            try
            {
                hubConnection.StartAsync();
                messages.Items.Add("Connection Started");
                openConnection.IsEnabled = false;
                sendMessage.IsEnabled = true;
            }
            catch (Exception ex)
            {
                messages.Items.Add(ex.Message);
            }
        }

        private async void sendMessage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await hubConnection.InvokeAsync("SendMessage", "WPF Client", messageInput.Text);
            }
            catch (Exception ex)
            {
                messages.Items.Add(ex.Message);
            }
        }
    }
}
