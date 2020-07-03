﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;

using TastyBot.Contracts;
using TastyBot.Utility;

using System;
using System.Reflection;
using System.Threading.Tasks;
 
namespace TastyBot.Services
{
    public class StartupService : IStartupService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly Config _config;

        // DiscordSocketClient, CommandService, and IConfigurationRoot are injected automatically from the IServiceProvider
        public StartupService(IServiceProvider provider, DiscordSocketClient discord, CommandService commands, Config config)
        {
            _provider = provider;
            _config = config;
            _discord = discord;
            _commands = commands;
        }

        public async Task StartAsync()
        {
            string discordToken = _config.DiscordToken;                                     // Get the discord token from the config file
            if (string.IsNullOrWhiteSpace(discordToken))
                throw new Exception("Please enter your bot's token into the `config.json` file found in the applications root directory.");

            await _discord.LoginAsync(TokenType.Bot, discordToken);                         // Login to discord
            await _discord.StartAsync();                                                    // Connect to the websocket

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);        // Load commands and modules into the command service
        }
    }
}