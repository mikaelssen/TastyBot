﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using TastyBot.FutureHeadPats;
using TastyBot.Services;

namespace TastyBot.Modules
{
	/// <summary>
	/// The Module that handles all FHP related interactions
	/// </summary>
	[Name("FutureHeadpat")]
	public class FhpModule : ModuleBase<SocketCommandContext>
	{
		private HeadpatService _HeadpatService;

		public FhpModule(HeadpatService headpatService)
		{
			_HeadpatService = headpatService;
		}

		/// <summary>
		/// Enables Earan to hand out freshly printed headpats
		/// </summary>
		/// <param name="receiveUser">The user that will receive the FHP</param>
		/// <param name="amount">The amount of FHP the user will receive.</param>
		/// <returns></returns>
		[Command("give")]
		[Summary("Hands out headpats to the user")]
		public async Task Give(IUser receiveUser, long amount)
		{
			if (Context.User.Id != 277038010862796801)
			{
				await ReplyAsync("Only the Headpat overlord can use this ancient spell");
				return;
			}

			FhpUser sender = _HeadpatService.GetUser(Context.User);
			FhpUser receiver = _HeadpatService.GetUser(receiveUser);

			receiver.Wallet += amount;

			await ReplyAsync($"{sender.Name} gave {receiver.Name} {amount}FHP");
		}


		/// <summary>
		/// Deletes the wallet of <paramref name="deleteUser"/> if the wallet exists
		/// </summary>
		/// <param name="deleteUser">The user which's wallet shall be deleted</param>
		/// <returns></returns>
		[Command("delete")]
		[Summary("Deletes the wallet of the tagged user, if the user has a wallet")]
		public async Task Delete(IUser deleteUser)
		{
			if (Context.User.Id == 277038010862796801 || Context.User.Id == 83183880869253120)
			{
				if (_HeadpatService.DeleteUser(deleteUser))
				{
					await ReplyAsync("Wallet deleted");
				}
				else
				{
					await ReplyAsync("User has no wallet ");
				}
			}
			else
			{
				await ReplyAsync("NO!");
			}
		}


		/// <summary>
		/// Saves the dictionary to the json file.
		/// </summary>
		/// <returns></returns>
		[Command("save")]
		[Summary("Instantly saves the current users")]
		public async Task Save()
		{
			if (Context.User.Id == 277038010862796801 || Context.User.Id == 83183880869253120)
			{
				_HeadpatService.Save();
				await ReplyAsync("Saved");
			}
			else
			{
				await ReplyAsync("NO!");
			}
		}

		/// <summary>
		/// Gives FHP from one user to another user
		/// </summary>
		/// <param name="receiveUser">The user that receives the headpats</param>
		/// <param name="amount">The amount of headpats to transfer</param>
		/// <returns></returns>
		[Command("pat")]
		[Summary("headpat another person")]
		public async Task Pat(IUser receiveUser, uint amount = 1)
		{
			if (receiveUser.IsBot)
			{
				await ReplyAsync("Can't pat a bot! Baka!");
				return;
			}

			if (receiveUser.Id == 277038010862796801)
			{
				await ReplyAsync("You cannot give headpats to the one giving out the headpats");
				return;
			}

			FhpUser sender = _HeadpatService.GetUser(Context.User);
			FhpUser receiver = _HeadpatService.GetUser(receiveUser);

			if (amount == 0)
			{
				await ReplyAsync($"<@{sender.Id}> tried to pat <@{receiver.Id}> but lacked the resolve to do so.");
				return;
			}

			if (sender.Wallet < amount)
			{
				await ReplyAsync("Sadly you don't have enough headpats :sob:");
				return;
			}

			sender.Wallet -= amount;
			receiver.Wallet += amount;

			await ReplyAsync($"<@{sender.Id}> patted <@{receiver.Id}> and sent over {amount}FHP");
		}

		/// <summary>
		/// Prints out the current wallet either for the invoking, or the mentioned user
		/// </summary>
		/// <param name="otheruser">The user the wallet should be printed out for. If null the wallet of the invoking user is printed</param>
		/// <returns></returns>
		[Command("wallet")]
		[Summary("show your current wallet balance")]
		public async Task Wallet(IUser otheruser = null)
		{
			FhpUser user = _HeadpatService.GetUser(Context.User);
			if (otheruser != null)
			{
				user = _HeadpatService.GetUser(otheruser);
			}
			Random rColor = new Random((int)user.Wallet);
			var builder = new EmbedBuilder()
			{
				Color = new Color(rColor.Next(0, 256), rColor.Next(0, 256), rColor.Next(0, 256)),
				Title = $"{user.Name}'s Wallet"
			};
			builder.AddField("FHP", user.Wallet);
			await ReplyAsync(embed: builder.Build());
		}


		/// <summary>
		/// Prints out a leaderboard with all people that currently are tracked by the FHP Module
		/// </summary>
		/// <returns></returns>
		[Command("leaderboard")]
		[Summary("shows the current FHP leaderboard")]
		public async Task Leaderboard()
		{
			List<FhpUser> users = _HeadpatService.GetLeaderboard();
			List<Embed> embeds = new List<Embed>();

			var builder = new EmbedBuilder()
			{
				Color = new Color(0, 255, 0),
				Title = "Leaderboard",
				Description = $"Total FHP: {users.Sum(x => x.Wallet)}"
			};

			StringBuilder leaderboard = new StringBuilder();
			string current = string.Empty;
			for (int i = 0; i < users.Count; i++)
			{
				current = $"{i + 1}. {users[i].Name} {users[i].Wallet} FHP";
				if (leaderboard.Length + current.Length >= 1024)
				{
					builder.AddField((builder.Fields.Count + 1).ToString(), leaderboard.ToString().TrimEnd('\n', '\r'));
					leaderboard.Clear();
				}
				leaderboard.AppendLine(current);
			}
			builder.AddField((builder.Fields.Count + 1).ToString(), leaderboard.ToString().TrimEnd('\n', '\r'));
			await ReplyAsync(embed: builder.Build());
		}


		[Command("explain")]
		[Summary("Explains what FHP means")]
		public async Task Explain()
		{
			await ReplyAsync("FHP stands for \"Future HeadPats\" and is the currency of this Server. " +
				"\r\n\r\n" +
				"At this moment you can do the following things:\r\n" +
				"!wallet\t\t\t\t\t\t\t\t - check your own wallet\r\n" +
				"!wallet @user\t\t\t\t\t- check somebody elses wallet\r\n" +
				"!leaderboard\t\t\t\t\t  - see the current FHP leaderboard\r\n" +
				"!pat @user <amount>\t - headpat other people and send them <amount> of headpats" +
				"\r\n\r\n" +
				"At some point in the future FHP holders will be able to turn them in for Headpats in VRC. There is no command for that yet.\r\n" +
				"The current exchange rate is 1FHP = 1 minute of uninterrupted headpatting.\r\n" +
				"The headpats are given out by <@277038010862796801>, up to 30 Minutes at a time." +
				"\r\n\r\n" +
				"Since <@277038010862796801> is a derp and gives out headpats for free to anyone showing up in VR, don't take this too seriously. It's just for funsies <:Tastyderp:669202378095984640>");
		}
	}
}