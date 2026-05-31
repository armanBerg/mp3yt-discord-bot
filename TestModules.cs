using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using System.Diagnostics;

namespace mp3yt_discord_bot;

public class MusicModule : ModuleBase<SocketCommandContext>
{
	private static Utilities _util = new Utilities();

	[Command("play")]
	[Summary("Plays a youtube clip as mp3")]
	public async Task PlayAsync([Summary("Paste youtube link")] string url = "", IVoiceChannel channel = null)
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


				//ytdlp download  
				//TODO: implement into Utilities class
				var process = Process.Start("yt-dlp.exe", $"-x --audio-format mp3 -o \"cache/%(id)s.%(ext)s\" {url}");

				process.WaitForExit();

				// foreach(char _ in url)
				// {
				// 	Console.WriteLine(_);
				// }

				string filename = url.Substring(43 - 11, 11);
				Console.WriteLine($"Console: {filename}");

				await SendAsync(audioClient, $"cache\\{filename}.mp3");
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
				using (var discord = client.CreatePCMStream(AudioApplication.Music))
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