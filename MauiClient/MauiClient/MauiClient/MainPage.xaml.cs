using Microsoft.AspNetCore.SignalR.Client;

namespace MauiClient;

public partial class MainPage : ContentPage
{
    private readonly HubConnection _connection;

    public MainPage()
    {
        InitializeComponent();

        _connection = new HubConnectionBuilder()
            .WithUrl("http://192.168.100.3:5036/chat") //Hardcoded URL need to be fixed.
            .Build();

        _connection.On<string, string>("MessageReceived", (user, message) =>
        {
            chatMessages.Text += $"{Environment.NewLine} {user}:{message}";
        });

        Task.Run(() =>
        {
            Dispatcher.Dispatch(async () =>
            await _connection.StartAsync());
        });
    }

    private async void OnSendMessageClicked(object sender, EventArgs e)
    {
        await _connection.SendAsync("SendMessage", userName.Text, userChatMessage.Text);

        userChatMessage.Text = String.Empty;
    }
}

