namespace WerewolfGM.Web.Models;

/// <summary>
/// Forme envoyée/reçue via SignalR pour un message de chat. Le protocole
/// JSON par défaut de SignalR sérialise en camelCase des deux côtés
/// (serveur .NET et client .NET), donc cette même classe fonctionne pour
/// l'émission et la réception sans mapping manuel.
/// </summary>
public record ChatMessageDto(
    int Id,
    int ChatGroupId,
    int? SenderPlayerId,
    string SenderName,
    string Content,
    DateTime SentAt,
    bool IsSystemMessage);
    