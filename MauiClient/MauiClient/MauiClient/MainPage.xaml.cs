using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace MauiClient;

public partial class MainPage : ContentPage
{
    private readonly HubConnection connection;
    public MainPage()
    {
        InitializeComponent();

        //The url it's defined in the Blazor server under 
        //Properties/launchsettings.json
        connection = new HubConnectionBuilder()
            //.WithUrl("https://localhost:7224/chat") 
            .WithUrl("http://192.168.100.2:5036/chat")
            .WithAutomaticReconnect()
            .Build();

        // Handling Reconnecting
        connection.Reconnecting += (sender) =>
        {
            this.Dispatcher.Dispatch(() =>
            {
                var newMessage = "Attempting to reconnect ....";
                chatMessages.Text += $"{Environment.NewLine} {newMessage}";
            });
            return Task.CompletedTask;
        };

        // Handling Reconnected
        connection.Reconnected += (sender) =>
        {
            this.Dispatcher.Dispatch(() =>
            {
                var newMessage = "Reconnected to the server ...";
                chatMessages.Text = String.Empty;
                chatMessages.Text += $"{Environment.NewLine} {newMessage}";
            });
            return Task.CompletedTask;
        };

        // Handling Closed
        connection.Closed += (sender) =>
        {
            this.Dispatcher.Dispatch(() =>
            {
                var newMessage = "Connection Closed";
                chatMessages.Text += $"{Environment.NewLine} {newMessage}";
                openConnection.IsEnabled = true;
                sendButton.IsEnabled = false;

            });
            return Task.CompletedTask;
        };


    }

    private async void OnSendMessageClicked(object sender, EventArgs e)
    {
        try
        {
            await connection.SendAsync("SendMessage", userName.Text, userChatMessage.Text);
            userChatMessage.Text = String.Empty;
        }
        catch (Exception ex) {
            chatMessages.Text += ex.Message;
        }
    }

    private async void OnOpenConnectionClicked(object sender, EventArgs e)
    {
        //Set the connection with the following event.
        connection.On<string, string>("MessageReceived", (user, message) =>
        {
            this.Dispatcher.Dispatch(() =>
                    {
                        chatMessages.Text += $"{Environment.NewLine} {user}:{message}";
                    });
          });
        try
        {
            //Start the connection.
            await connection.StartAsync();
            chatMessages.Text += $"{Environment.NewLine} Connection Started ...";
            openConnection.IsEnabled = false;
            sendButton.IsEnabled = true;
            closeConnection.IsEnabled = true;
        }
        catch (Exception ex) {
            chatMessages.Text += ex.Message;
        }
    }

    private async void OnCloseConnectionClicked(object sender, EventArgs e) {
        try
        {
            //Start the connection.
            await connection.StopAsync();
            chatMessages.Text += $"{Environment.NewLine} Connection Stoped ...";
            openConnection.IsEnabled = true;
            sendButton.IsEnabled = false;
            closeConnection.IsEnabled = false;
        }
        catch (Exception ex)
        {
            chatMessages.Text += ex.Message;
        }

    }
}

