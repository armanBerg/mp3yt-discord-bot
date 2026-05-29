using System.Diagnostics;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using DotNetEnv;

namespace mp3yt_discord_bot;

public class Program
{
	private static CommandHandler _handler;
	private static DiscordSocketClient _client;
	private static CommandService _commands;

	private static Task Log(LogMessage msg)
	{
		Console.WriteLine(msg.ToString());
		return Task.CompletedTask;
	}

	public static async Task Main()
	{
		Env.Load();
		_client = new DiscordSocketClient(new DiscordSocketConfig
		{
			GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
		});
		_client.Log += Log;

		_commands = new CommandService();
		_handler = new CommandHandler(_client, _commands);

		//  You can assign your bot token to a string, and pass that in to connect.
		//  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
		var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

		// Some alternative options would be to keep your token in an Environment Variable or a standalone file.
		// var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
		// var token = File.ReadAllText("token.txt");
		// var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

		await _client.LoginAsync(TokenType.Bot, token);
		await _client.StartAsync();

		// Block this task until the program is closed.
		await Task.Delay(-1);
	}
}