using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;

namespace mp3yt_discord_bot;

// Create a module with no prefix
public class TestModules : ModuleBase<SocketCommandContext>
{
	// ~say hello world -> hello world
	[Command("say")]
	[Summary("Echoes a message.")]
	public Task SayAsync([Remainder] [Summary("The text to echo")] string echo) => ReplyAsync(echo);

	// ReplyAsync is a method on ModuleBase 
}

// Create a module with the 'sample' prefix
[Group("sample")]
public class SampleModule : ModuleBase<SocketCommandContext>
{
	// ~sample square 20 -> 400
	[Command("square")]
	[Summary("Squares a number.")]
	public async Task SquareAsync([Summary("The number to square.")] int num)
	{
		// We can also access the channel from the Command Context.
		await Context.Channel.SendMessageAsync($"{num}^2 = {Math.Pow(num, 2)}");
	}

	// ~sample userinfo --> foxbot#0282
	// ~sample userinfo @Khionu --> Khionu#8708
	// ~sample userinfo Khionu#8708 --> Khionu#8708
	// ~sample userinfo Khionu --> Khionu#8708
	// ~sample userinfo 96642168176807936 --> Khionu#8708
	// ~sample whois 96642168176807936 --> Khionu#8708
	[Command("userinfo")]
	[Summary("Returns info about the current user, or the user parameter, if one passed.")]
	[Alias("user", "whois")]
	public async Task UserInfoAsync([Summary("The (optional) user to get info from")] SocketUser user = null)
	{
		var userInfo = user ?? Context.Client.CurrentUser;
		await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
	}
}

public class MusicModule : ModuleBase<SocketCommandContext>
{
	private static Utilities _util;

	[Command("play")]
	[Summary("Plays a youtube clip as mp3")]
	public async Task PlayAsync([Summary("Paste youtube link")] string ytLink = "", IVoiceChannel channel = null)
	{
		channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
		if (channel == null)
		{
			await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument.");
			return;
		}
		_ = Task.Run(async () =>
		{
			try
			{
				var audioClient = await channel.ConnectAsync();
				SendAsync(audioClient, "C:\\Users\\lynin\\RiderProjects\\mp3yt-discord-bot\\DJ Splash This Is My Life [zyBDF2hgqcc].mp3");
			} catch (Exception ex)
			{
				await Context.Channel.SendMessageAsync($"Error: {ex.Message}");
			}
		});
	}

	private async Task SendAsync(IAudioClient client, string path)
	{
		// Create FFmpeg using the previous example
		using (var ffmpeg = _util.CreateStream(path))
			using (var output = ffmpeg.StandardOutput.BaseStream)
				using (var discord = client.CreatePCMStream(AudioApplication.Mixed))
				{
					try
					{
						Console.WriteLine("Itgs doin it");
						await output.CopyToAsync(discord);
						Console.WriteLine("Itgs done doin it");
					} finally
					{
						await discord.FlushAsync();
					}
				}
	}
}