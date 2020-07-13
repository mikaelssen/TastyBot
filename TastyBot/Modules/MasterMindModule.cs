﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MasterMind.Contracts;
using System;
using System.Threading.Tasks;
using System.IO;
using Discord.Rest;
using Utilities.TasksUtilities;

namespace DiscordUI.Modules
{
    /// <summary>
    /// The Module that handles all MasterMind Things
    /// </summary>
    [Name("MasterMind")]
    public class MasterMindModule : ModuleBase<SocketCommandContext>
    {
        private readonly IMasterMindModule _module;
        private readonly DiscordSocketClient _discord;
        RestUserMessage imageMessage;
        public MasterMindModule(IMasterMindModule module, DiscordSocketClient discord) 
        {
            _module = module;
            _discord = discord;
            _discord.ReactionAdded += OnReactionAddedAsync;
        }

        private Task OnReactionAddedAsync(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            throw new NotImplementedException();
        }

        [Command("start")]
        [Summary("starts game of mastermind\nto start a game do !start\nfor additional customization do !start {height} {width}")]
        
        public async Task StartMasterMind(int height = 12, int width = 4)
        {
            MemoryStream stream;
            if (Context.Channel.Name.ToLower() != "master-mind") return;
            if (height > 30)
            {
                await Context.Channel.SendMessageAsync("The height has been clamped to 30 dots, because why the hell would you need more?");
            }
            if (width > 10)
            {
                await Context.Channel.SendMessageAsync("The width has been clamped to 10 dots, because why the hell would you need more?");
            }
        
            height = Math.Clamp(height, 0, 30);
            width = Math.Clamp(width, 0, 10);

            if (_module.IsGameRunning())
            {
                await Context.Channel.SendMessageAsync("Very much error has occured, Don't start a game while a game is in process");
                return;
            }

            _module.StartGame();
            stream = _module.StartBoardMaker(height, width);
            imageMessage = await Context.Channel.SendFileAsync(stream, "MasterMind.png");
            SendEmote();
            /* Task.Delay(6000);
            await imageMessage.ModifyAsync(msg => msg.Content = "test [edited]"); */

        }
        public void SendEmote()
        {
            Emoji YourEmoji = new Emoji("⚫");
            imageMessage.AddReactionAsync(YourEmoji).PerformAsyncTaskWithoutAwait();
            Emoji YourEmoji1 = new Emoji("🟡");
            imageMessage.AddReactionAsync(YourEmoji1).PerformAsyncTaskWithoutAwait();
            Emoji YourEmoji2 = new Emoji("🟠");
            imageMessage.AddReactionAsync(YourEmoji2).PerformAsyncTaskWithoutAwait();
            Emoji YourEmoji3 = new Emoji("🟣");
            imageMessage.AddReactionAsync(YourEmoji3).PerformAsyncTaskWithoutAwait();
            Emoji YourEmoji4 = new Emoji("🟢");
            imageMessage.AddReactionAsync(YourEmoji4).PerformAsyncTaskWithoutAwait();
            Emoji YourEmoji5 = new Emoji("🔵");
            imageMessage.AddReactionAsync(YourEmoji5).PerformAsyncTaskWithoutAwait();
            Emoji YourEmoji6 = new Emoji("🔴");
            imageMessage.AddReactionAsync(YourEmoji6).PerformAsyncTaskWithoutAwait();
        }
        
    }
}
