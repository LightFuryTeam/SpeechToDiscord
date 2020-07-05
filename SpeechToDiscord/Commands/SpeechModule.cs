using System;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using NAudio.Wave;
using Serilog;

namespace SpeechToDiscord.Commands
{
    [Group("TestBot")]
    public class SpeechModule : ModuleBase<SocketCommandContext>
    {
        [Command("speak", RunMode = RunMode.Async)]
        public async Task JoinAsync(IVoiceChannel channel = null)
        {
            // Get the audio channel
            channel ??= (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }

            // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
            var audioClient = await channel.ConnectAsync();
            await SpeakAsync(audioClient);
        }

        private async Task SpeakAsync(IAudioClient client)
        {
            try
            {

                using (var reader = new WaveFileReader("Audio/Test123.wav"))
                {
                    var naudio = WaveFormatConversionStream.CreatePcmStream(reader);

                    //using (var waveOut = new WaveOutEvent())
                    //{
                    //    waveOut.Init(naudio);
                    //    Log.Logger.Debug("Playing sounds...");
                    //    waveOut.Play();
                    //    while (waveOut.PlaybackState == PlaybackState.Playing)
                    //    {
                    //        Thread.Sleep(1000);
                    //    }
                    //}

                    var dstream = client.CreatePCMStream(AudioApplication.Music);
                    await naudio.CopyToAsync(dstream);

                    dstream.Flush();
                    client.StopAsync().Wait();
                }
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Error while sending to discord");
            }
        }
    }
}
