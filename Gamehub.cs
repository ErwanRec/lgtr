using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WerewolfGM.Web.Services;

namespace WerewolfGM.Web.Hubs;

/// <summary>
/// Hub temps réel unique pour le chat par salons et la diffusion en direct
/// des dépouillements de vote. Chaque salon de chat et chaque session de
/// vote a son propre "groupe SignalR" (chat-{id} / vote-{id}), ce qui
/// évite d'envoyer les messages d'un salon à tout le monde.
/// </summary>
[Authorize]
public class GameHub : Hub
{
    private readonly ChatService _chatService;
    private readonly VoteService _voteService;

    public GameHub(ChatService chatService, VoteService voteService)
    {
        _chatService = chatService;
        _voteService = voteService;
    }

    private int CurrentPlayerId =>
        int.Parse(Context.User!.FindFirst("PlayerId")!.Value);

    private bool CurrentIsGameMaster =>
        Context.User!.FindFirst("IsGameMaster")?.Value == "true";

    // ---------- Chat ----------

    public async Task JoinChatGroup(int chatGroupId)
    {
        if (!CurrentIsGameMaster && !await _chatService.IsMemberAsync(chatGroupId, CurrentPlayerId))
            throw new HubException("Vous ne faites pas partie de ce salon.");

        await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-{chatGroupId}");
    }

    public async Task LeaveChatGroup(int chatGroupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat-{chatGroupId}");
    }

    public async Task SendMessage(int chatGroupId, string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return;
        if (!CurrentIsGameMaster && !await _chatService.IsMemberAsync(chatGroupId, CurrentPlayerId))
            throw new HubException("Vous ne faites pas partie de ce salon.");

        var message = await _chatService.PostMessageAsync(chatGroupId, CurrentPlayerId, content);

        await Clients.Group($"chat-{chatGroupId}").SendAsync("ReceiveMessage", new
        {
            message.Id,
            message.ChatGroupId,
            SenderName = message.SenderPlayer?.FullName ?? "Système",
            message.Content,
            message.SentAt,
            message.IsSystemMessage
        });
    }

    // ---------- Votes ----------

    public async Task JoinVoteSession(int voteSessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"vote-{voteSessionId}");
    }

    public async Task LeaveVoteSession(int voteSessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"vote-{voteSessionId}");
    }

    public async Task CastVote(int voteSessionId, int targetPlayerId)
    {
        await _voteService.CastVoteAsync(voteSessionId, CurrentPlayerId, targetPlayerId);
        var tally = await _voteService.GetTallyAsync(voteSessionId);

        // Le MJ (abonné au groupe de la session) reçoit le dépouillement en direct.
        await Clients.Group($"vote-{voteSessionId}").SendAsync("VoteTallyUpdated", voteSessionId);
    }
}