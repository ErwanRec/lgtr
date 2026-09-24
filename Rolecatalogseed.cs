using WerewolfGM.Web.Models;

namespace WerewolfGM.Web.Data;

/// <summary>
/// Catalogue des rôles, repris des documents de règles fournis
/// (Loup-garou en temps réel). Modifiable librement : c'est une simple
/// liste de départ insérée en base au premier lancement.
/// </summary>
public static class RoleCatalogSeed
{
    public static List<RoleDefinition> GetRoles() => new()
    {
        // ---------- LOUPS ----------
        new() { Name = "Loup-garou", Camp = Camp.Loups, Emoji = "🐺",
            ShortDescription = "Fait partie de la meute dès le départ.",
            FullDescription = "Membre de base de la meute des loups. Vote chaque soir à 22h avec les autres loups pour désigner une victime (30 minutes, seul le dernier nom envoyé compte, et la cible doit avoir été préalablement repérée dans la journée).",
            HasFixedTimeAction = true },

        new() { Name = "Loup-presque-garou", Camp = Camp.Loups, Emoji = "🐺",
            ShortDescription = "Doit rejoindre la meute pour pouvoir voter.",
            FullDescription = "Ne fait pas encore partie de la meute. Reçoit le nom d'un loup-garou (sans connaître son rôle) et doit le retrouver ; seul le loup-garou alpha peut ensuite demander au MJ de l'intégrer officiellement à la meute pour qu'il puisse voter le soir.",
            HasFixedTimeAction = true },

        new() { Name = "Loup-garou alpha", Camp = Camp.Loups, Emoji = "🐺👑",
            ShortDescription = "Chef de meute, seul à pouvoir intégrer un nouveau loup.",
            FullDescription = "Dirige la meute. C'est le seul à pouvoir demander au MJ l'intégration d'un loup-presque-garou dans la meute. Vote normalement au vote des loups.",
            HasFixedTimeAction = true },

        new() { Name = "Grand méchant loup", Camp = Camp.Loups, Emoji = "😈",
            ShortDescription = "Peut faire une victime supplémentaire.",
            FullDescription = "Tant qu'aucun loup n'est mort, il peut provoquer une victime supplémentaire en plus du vote classique des loups.",
            HasFixedTimeAction = true },

        new() { Name = "Loup garou blanc", Camp = Camp.Loups, Emoji = "🤍🐺",
            ShortDescription = "Joue en solo, doit gagner seul.",
            FullDescription = "Doit gagner seul. Une nuit sur deux, il peut, s'il le souhaite, éliminer un autre loup.",
            HasFixedTimeAction = true },

        new() { Name = "Loup-garou amnésique", Camp = Camp.Loups, Emoji = "❓🐺",
            ShortDescription = "Est loup sans le savoir.",
            FullDescription = "Est loup-garou mais l'ignore. Un autre loup doit lui révéler la vérité pour qu'il confirme ensuite auprès du MJ. C'est l'un des deux noms transmis au loup-garou alpha.",
            HasFixedTimeAction = false },

        new() { Name = "Infect père des loups", Camp = Camp.Loups, Emoji = "🧪🐺",
            ShortDescription = "Peut transformer un villageois en loup.",
            FullDescription = "Peut, au moment du vote des loups, infecter un villageois pour le transformer en loup. Le joueur infecté conserve son propre pouvoir en plus de rejoindre le camp des loups.",
            HasFixedTimeAction = true },

        new() { Name = "Loup poète", Camp = Camp.Loups, Emoji = "📜🐺",
            ShortDescription = "Doit placer une phrase imposée sous peine de mort.",
            FullDescription = "N'a pas de pouvoir actif, mais doit glisser dans la journée une phrase précise choisie par le MJ (preuve par capture d'écran), sous peine d'élimination immédiate.",
            HasFixedTimeAction = false },

        new() { Name = "Loup-garou voyant", Camp = Camp.Loups, Emoji = "👁️🐺",
            ShortDescription = "Peut découvrir le rôle exact d'un joueur.",
            FullDescription = "Chaque soir, peut demander au MJ le rôle d'un joueur. Une fois par jour, il peut aussi prendre discrètement une photo d'un joueur de son choix pour obtenir son rôle exact.",
            HasFixedTimeAction = true },

        new() { Name = "Loup feutré", Camp = Camp.Loups, Emoji = "🥷🐺",
            ShortDescription = "Apparaît comme simple villageois aux yeux des enquêteurs.",
            FullDescription = "Si un autre rôle enquête sur lui (voyante, voyant, juge...), il apparaît comme un simple villageois plutôt que comme loup.",
            HasFixedTimeAction = false },

        // ---------- VILLAGEOIS ----------
        new() { Name = "Simple villageois (Cogniticien)", Camp = Camp.Villageois, Emoji = "🧑‍🌾",
            ShortDescription = "Aucun pouvoir particulier.",
            FullDescription = "N'a pas de pouvoir actif, sauf s'il fait partie des morts ayant voté : dans ce cas il est le seul à connaître ce vote des morts.",
            HasFixedTimeAction = false },

        new() { Name = "Petite fille", Camp = Camp.Villageois, Emoji = "👧",
            ShortDescription = "Espionne une partie du vote des loups.",
            FullDescription = "Reçoit un extrait (environ 5 minutes) de la conversation anonymisée du vote des loups, à son choix, parmi la plage horaire du vote.",
            HasFixedTimeAction = false },

        new() { Name = "Boulanger", Camp = Camp.Villageois, Emoji = "🥖",
            ShortDescription = "Peut piéger un joueur qui parle au mauvais moment.",
            FullDescription = "Chaque jour entre minuit et 8h, choisit un joueur cible et une heure. Si la cible parle en sa présence dans les 30 minutes suivant l'heure choisie, le boulanger peut l'éliminer en révélant son rôle.",
            HasFixedTimeAction = true },

        new() { Name = "Voyante", Camp = Camp.Villageois, Emoji = "🔮",
            ShortDescription = "Découvre le rôle exact d'un joueur.",
            FullDescription = "Une fois par jour, prend discrètement en photo un joueur de son choix pour obtenir son rôle exact.",
            HasFixedTimeAction = false },

        new() { Name = "Enquêteur", Camp = Camp.Villageois, Emoji = "🕵️",
            ShortDescription = "Enquête ou espionne, un pouvoir à la fois.",
            FullDescription = "Deux pouvoirs distincts, un seul actif à la fois : Enquête (cibler une personne et prendre une photo des deux ensemble sur 3 jours différents pour obtenir son rôle, 3 utilisations max) ou Espionnage (désigner deux personnes présentes sur une photo pour savoir le lendemain si elles sont dans le même camp, 1 utilisation max).",
            HasFixedTimeAction = false },

        new() { Name = "Sorcière", Camp = Camp.Villageois, Emoji = "🧙",
            ShortDescription = "Dispose d'une potion de vie et d'une potion de mort.",
            FullDescription = "Peut utiliser sa potion de vie et sa potion de mort une seule fois chacune sur toute la partie (max une potion par soir). Doit faire \"boire\" la cible dans la journée pour que l'effet s'applique la nuit ; peut se cibler elle-même.",
            HasFixedTimeAction = true },

        new() { Name = "Chasseur", Camp = Camp.Villageois, Emoji = "🏹",
            ShortDescription = "Élimine quelqu'un au moment de sa propre mort.",
            FullDescription = "S'il a préalablement désigné une cible (bout de papier), il peut l'éliminer au moment où il meurt. Peut redésigner une nouvelle cible chaque jour.",
            HasFixedTimeAction = false },

        new() { Name = "Salvateur", Camp = Camp.Villageois, Emoji = "🛡️",
            ShortDescription = "Protège un joueur différent chaque jour.",
            FullDescription = "Protège une personne par jour (jamais deux fois de suite la même) en déposant un objet convenu dans ses affaires la veille ; la protection est validée par vérification de l'objet.",
            HasFixedTimeAction = false },
        
        new() { Name = "Juge", Camp = Camp.Villageois, Emoji = "🧑‍⚖️",
            ShortDescription = "",
            FullDescription = "",
            HasFixedTimeAction = false },
        
        new() { Name = "Trouduc", Camp = Camp.Villageois, Emoji = "",
            ShortDescription = "",
            FullDescription = "",
            HasFixedTimeAction = false },

        new() { Name = "Chien-Loup", Camp = Camp.Villageois, Emoji = "🐕",
            ShortDescription = "Choisit son camp (villageois par défaut).",
            FullDescription = "Peut choisir dès le début s'il est Loup-Garou ou Cogniticien (villageois par défaut). S'il offre un cadeau à un des MJ, il devient Loup-Garou.",
            HasFixedTimeAction = false },

        new() { Name = "Cupidon", Camp = Camp.Villageois, Emoji = "💘",
            ShortDescription = "Forme le couple du début de partie.",
            FullDescription = "Offre une lettre romantique signée à deux joueurs de son choix pour former le couple avant le 3ᵉ jour (sinon le couple est tiré au sort). Révèle le couple au MJ. Le couple ne peut gagner qu'ensemble, avec Cupidon.",
            HasFixedTimeAction = false },

        new() { Name = "Le squatteur", Camp = Camp.Villageois, Emoji = "🛋️",
            ShortDescription = "S'installe chez un autre joueur pour 24h.",
            FullDescription = "Choisit chaque jour un joueur différent chez qui \"squatter\" pour 24h : chez un villageois, il meurt aussi si celui-ci est mangé ; chez un solo, il obtient un de ses pouvoirs ; chez un loup, il ne peut pas être mangé cette nuit-là.",
            HasFixedTimeAction = true },

        new() { Name = "Rival", Camp = Camp.Villageois, Emoji = "💔",
            ShortDescription = "Peut prendre la place d'un membre du couple.",
            FullDescription = "Peut offrir une rose à un membre du couple pour prendre la place de l'autre, ou récupérer automatiquement la place s'il a voté pour éliminer un membre du couple lors du vote du village.",
            HasFixedTimeAction = false },

        new() { Name = "Le pot de colle", Camp = Camp.Villageois, Emoji = "🤗",
            ShortDescription = "Gagne des votes en faisant des câlins.",
            FullDescription = "Une fois par jour, fait un câlin à une nouvelle personne (jamais deux fois la même) pour gagner un vote supplémentaire, à annoncer au MJ.",
            HasFixedTimeAction = false },

        new() { Name = "Idiot du village", Camp = Camp.Villageois, Emoji = "🤡",
            ShortDescription = "Survit à son propre vote du village, une fois révélé.",
            FullDescription = "S'il est voté par le village, il ne meurt pas mais ne peut plus être voté ni voter lui-même par la suite. Une fois révélé, il ne peut plus mourir par un killer.",
            HasFixedTimeAction = false },

        new() { Name = "Voleur", Camp = Camp.Villageois, Emoji = "🥷",
            ShortDescription = "Peut voler le rôle d'un autre joueur.",
            FullDescription = "Peut voler le rôle d'un autre joueur une seule fois dans la partie, en envoyant un selfie avec le téléphone de la victime à un MJ.",
            HasFixedTimeAction = false },

        new() { Name = "Enfant sauvage", Camp = Camp.Villageois, Emoji = "🐾",
            ShortDescription = "Devient loup si son modèle meurt.",
            FullDescription = "Choisit un modèle le jour 1 (via une poignée de main ou un check). Si ce joueur meurt, l'enfant sauvage devient loup-garou ; sinon il reste simple villageois.",
            HasFixedTimeAction = false },

        new() { Name = "Ancien", Camp = Camp.Villageois, Emoji = "👴",
            ShortDescription = "Résiste une fois de plus aux loups.",
            FullDescription = "Possède deux vies face à une élimination par les loups, mais une seule face à un vote du village.",
            HasFixedTimeAction = false },

        new() { Name = "Sœurs", Camp = Camp.Villageois, Emoji = "👭",
            ShortDescription = "Peuvent partager leur rôle entre elles.",
            FullDescription = "Seules à avoir le droit de se révéler leur rôle mutuellement. Quand l'une meurt, l'autre peut apprendre l'identité de son tueur.",
            HasFixedTimeAction = false },

        new() { Name = "Frère", Camp = Camp.Villageois, Emoji = "👨‍👦‍👦",
            ShortDescription = "Peuvent partager leur rôle entre eux. ils auront chacun l'identiter d'un seul de leur deux autres frères",
            FullDescription = "Seules à avoir le droit de se révéler leur rôle mutuellement. Quand l'un meurt, l'autre peut apprendre l'identité de son tueur.",
            HasFixedTimeAction = false },

        new() { Name = "Montreur d'ours", Camp = Camp.Villageois, Emoji = "🐻",
            ShortDescription = "Révèle qui était autour de lui.",
            FullDescription = "Chaque jour, à l'heure de son choix, annonce au moins 5 noms de personnes présentes autour de lui (dont le sien), déclenchant des \"grrr\" dans le groupe commun.",
            HasFixedTimeAction = false },

        new() { Name = "Corbeau", Camp = Camp.Villageois, Emoji = "🐦‍⬛",
            ShortDescription = "Dispose de votes bonus à distribuer.",
            FullDescription = "Possède plusieurs votes supplémentaires (indiqués par les MJ, typiquement 3 à 5) à ajouter librement sur la cible de son choix, en le signalant au MJ avant le vote du village.",
            HasFixedTimeAction = false },

        new() { Name = "Servante dévouée", Camp = Camp.Villageois, Emoji = "🙇",
            ShortDescription = "Peut prendre la place d'un joueur voté avant sa mort.",
            FullDescription = "Peut révéler son propre rôle avant la mort du joueur désigné par le vote du village pour récupérer son rôle et ses pouvoirs à sa place.",
            HasFixedTimeAction = false },

        new() { Name = "Trublion", Camp = Camp.Villageois, Emoji = "🎭",
            ShortDescription = "Peut deviner un tiers des rôles de la partie.",
            FullDescription = "Quand les MJ l'y autorisent, peut deviner un tiers des rôles des joueurs. S'il a raison, les rôles concernés sont échangés.",
            HasFixedTimeAction = false },

        // ---------- SOLO ----------
        new() { Name = "Assassin", Camp = Camp.Solo, Emoji = "🗡️",
            ShortDescription = "Joue seul, tue s'il a marqué sa cible.",
            FullDescription = "Doit gagner seul. Peut tuer chaque nuit à condition d'avoir remis un bout de papier marqué à un joueur dans la journée, et que ce joueur l'ait vu et signalé au MJ.",
            HasFixedTimeAction = false },

        new() { Name = "Hypnotiseur", Camp = Camp.Solo, Emoji = "🌀",
            ShortDescription = "Force un joueur à voter pour une cible précise.",
            FullDescription = "Chaque tour, désigne un joueur hypnotisé et une cible : l'hypnotisé sera contraint de voter pour la cible au vote du village (et au vote des loups s'il en fait partie) le lendemain.",
            HasFixedTimeAction = false },

        new() { Name = "Joueur de flûte", Camp = Camp.Solo, Emoji = "🎼",
            ShortDescription = "Enchante des joueurs pour gagner seul.",
            FullDescription = "Doit gagner seul. Chaque jour, remet un objet défini par les MJ à un joueur pour l'enchanter. Il gagne s'il ne reste plus en jeu que lui-même et des joueurs enchantés.",
            HasFixedTimeAction = false },

        new() { Name = "Ange", Camp = Camp.Solo, Emoji = "👼",
            ShortDescription = "Ange protecteur ou ange déchu, une cible unique.",
            FullDescription = "Choisit dès le début une cible (qui est prévenue) et un camp personnel : Ange protecteur (gagne des votes bonus cumulables tant que sa cible reste protégée ou en vie) ou Ange déchu (gagne des votes bonus après avoir lui-même éliminé sa cible). Son rôle est révélé s'il meurt en ayant protégé sa cible, ou si sa cible meurt autrement que par lui en mode déchu.",
            HasFixedTimeAction = false },
    };
}