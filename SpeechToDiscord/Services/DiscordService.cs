﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace SpeechToDiscord.Services
{
    public class DiscordService : IDisposable
    {
        private readonly string _token;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public DiscordService(string token)
        {
            _token = token;
            _client = new DiscordSocketClient();
            _commands = new CommandService();
        }

        public async Task Start()
        {
            _client.Log += OnLog;
            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();

            _client.MessageReceived += ClientOnMessageReceived;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
        }

        private async Task ClientOnMessageReceived(SocketMessage socketMessage)
        {
            // Don't process the command if it was a system message
            if (!(socketMessage is SocketUserMessage message)) return;

            // Create a number to track where the prefix ends and the command begins
            var argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)) || message.Author.IsBot) return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _commands.ExecuteAsync(context, argPos, null);
        }

        public async Task Stop()
        {
            await _client.StopAsync();
            await _client.LogoutAsync();
            _client.Log -= OnLog;
        }

        private Task OnLog(LogMessage msg)
        {
            return Task.Run(() =>
            {
                
            });
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
