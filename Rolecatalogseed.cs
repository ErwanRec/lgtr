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
            FullDescription = "Membre de base de la meute des loups. Vous votez chaque jour (entre 22h et 22h30) pour choisir la victime. L’élimination est valide seulement si le loup a déjà, dans la partie, placé un bout de papier dans les affaires de la cible avec le message de son choix.",
            HasFixedTimeAction = true },

        new() { Name = "Loup-presque-garou", Camp = Camp.Loups, Emoji = "🐺",
            ShortDescription = "Doit rejoindre la meute pour pouvoir voter.",
            FullDescription = "Ne fait pas encore partie de la meute. Reçoit le nom du loup-garou alpha et d'un villageois. doit retrouver le loup alpha pour pouvoir rejoindre le groupe de la meute.",
            HasFixedTimeAction = true },

        new() { Name = "Loup-garou alpha", Camp = Camp.Loups, Emoji = "🐺👑",
            ShortDescription = "Chef de meute, seul à pouvoir intégrer un nouveau loup.",
            FullDescription = "Dirige la meute. C'est le seul à pouvoir intégrer un joueur dans le groupe de la meute. Vote normalement au vote des loups.",
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
            FullDescription = "Est loup-garou mais l'ignore. il doit retrouver les loups seul pour pouvoir être ajouter sur le groupe",
            HasFixedTimeAction = false },

        new() { Name = "Infect père des loups", Camp = Camp.Loups, Emoji = "🧪🐺",
            ShortDescription = "Peut transformer un villageois en loup.",
            FullDescription = "Peut, au moment du vote des loups, infecter la victime pour le transformer en loup. Le joueur infecté conserve son propre pouvoir en plus de rejoindre le camp des loups.",
            HasFixedTimeAction = true },

        new() { Name = "Loup poète", Camp = Camp.Loups, Emoji = "📜🐺",
            ShortDescription = "Doit placer une phrase imposée sous peine de mort.",
            FullDescription = "N'a pas de pouvoir actif, mais doit glisser dans la journée une phrase/mot précise choisie par le MJ sur le groupe de promo ou du village (preuve par capture d'écran), sous peine d'élimination immédiate.",
            HasFixedTimeAction = false },

        new() { Name = "Loup-garou voyant", Camp = Camp.Loups, Emoji = "👁️🐺",
            ShortDescription = "Peut découvrir le rôle exact d'un joueur.",
            FullDescription = "Une fois par jour, vous pouvez prendre en photo un joueur afin d’obtenir son rôle exact. Jusqu'à 3 maximum par semaine (du lundi au dimanche ), les photos donné doivent être prise le jour de la demande.",
            HasFixedTimeAction = true },

        new() { Name = "Loup feutré", Camp = Camp.Loups, Emoji = "🥷🐺",
            ShortDescription = "Appara36ît comme simple villageois aux yeux des enquêteurs.",
            FullDescription = "Si un autre rôle enquête sur lui (voyante, voyant, juge...), il apparaît comme un rôle villageois plutôt que comme loup.",
            HasFixedTimeAction = false },

        // ---------- VILLAGEOIS ----------
        new() { Name = "Simple villageois (Cogniticien)", Camp = Camp.Villageois, Emoji = "🧑‍🌾",
            ShortDescription = "Aucun pouvoir particulier.",
            FullDescription = "N'a pas de pouvoir actif.",
            HasFixedTimeAction = false },

        new() { Name = "Petite fille", Camp = Camp.Villageois, Emoji = "👧",
            ShortDescription = "Espionne une partie du vote des loups.",
            FullDescription = "Reçoit un extrait (10 minutes) de la conversation anonymisée du vote des loups, à son choix, parmi la plage horaire du vote (entre 22h et 22h30).",
            HasFixedTimeAction = false },

        new() { Name = "Boulanger", Camp = Camp.Villageois, Emoji = "🥖",
            ShortDescription = "Peut piéger un joueur qui parle au mauvais moment.",
            FullDescription = "Chaque jour entre minuit et 8h, choisit un joueur cible et une heure. Si la cible parle en sa présence dans les 30 minutes suivant l'heure choisie, le boulanger peut l'éliminer en révélant son rôle au MJ.",
            HasFixedTimeAction = true },

        new() { Name = "Voyante", Camp = Camp.Villageois, Emoji = "🔮",
            ShortDescription = "Découvre le rôle exact d'un joueur.",
            FullDescription = "Vous pouvez prendre en secret une photo du joueur de votre choix afin de découvrir son rôle exact.Jusqu'à 3 maximum par semaine (du lundi au dimanche ), les photos données doivent être prises le jour de la demande.",
            HasFixedTimeAction = false },

        new() { Name = "Enquêteur", Camp = Camp.Villageois, Emoji = "🕵️",
            ShortDescription = "Enquête ou espionne, un pouvoir à la fois.",
            FullDescription = "Il peut prendre une photo d’un joueur par jour. Lorsqu’il prend une photo d’un joueur, il découvre une lettre de son rôle. Il peut faire plusieurs photos de la même personne pour obtenir plusieurs lettres de son rôle. Et en 1 journée il peut envoyer plusieurs personnes différentes en photo avec un max de 5 par jour.",
            HasFixedTimeAction = false },

        new() { Name = "Sorcière", Camp = Camp.Villageois, Emoji = "🧙",
            ShortDescription = "Dispose d'une potion de vie et d'une potion de mort.",
            FullDescription = "La sorcière dispose de plusieurs potions qu’elle peut utiliser. Chaque potion est à usage unique : \n Potion de résurrection : permet de ressusciter un joueur.\n Potion de mort : double le nombre des votes contre un joueur.\n Potion de boost : triple le nombre de votes effectués par un joueur.\n Pour utiliser une potion, la sorcière doit faire boire ou servir de l'eau au joueur qu’elle souhaite affecter, elle peut elle-même s’affecter.",
            HasFixedTimeAction = true },

        new() { Name = "Chasseur", Camp = Camp.Villageois, Emoji = "🏹",
            ShortDescription = "Élimine quelqu'un au moment de sa propre mort.",
            FullDescription = "À sa mort, il peut éliminer un joueur, à condition d’avoir auparavant placé un papier « Touché » dans les affaires de cette personne. \n Chaque jour, il peut cibler autant de joueurs qu’il veut et l’annoncer au MJ. Lorsqu’il meurt, il peut alors choisir de tuer l’un des joueurs qu’il avait ciblés",
            HasFixedTimeAction = false },

        new() { Name = "Salvateur", Camp = Camp.Villageois, Emoji = "🛡️",
            ShortDescription = "Protège un joueur différent chaque jour.",
            FullDescription = "Il peut protéger une personne par jour, à condition de ne pas protéger deux fois de suite la même personne.\n Pour cela, il doit poser sa main sur l’épaule droite du joueur qu’il désire protéger. Il peut également se protéger lui-même.\n Le protégé est immunisé contre le vote de tous les Loups et de la sorcière.",
            HasFixedTimeAction = false },
        
        new() { Name = "Juge", Camp = Camp.Villageois, Emoji = "🧑‍⚖️",
            ShortDescription = "compare le camps de deux joueurs",
            FullDescription = "Trois fois par semaine, il peut désigner deux joueurs au MJ en utilisant une photo où seuls les deux apparaissent (il ne peut pas se photographier lui-même). Le MJ lui indique alors si ces deux joueurs appartiennent au même camp ou à des camps différents.",
            HasFixedTimeAction = false },
        
        new() { Name = "Trouduc", Camp = Camp.Villageois, Emoji = "",
            ShortDescription = "vous vous faite passer pour un loup garou",
            FullDescription = "Si vous subissez une inspection, votre rôle de façade apparaît : Simple Loup-garou. \n Vous apparaissez dans les noms proposés à l’un des Loup-garous.",
            HasFixedTimeAction = false },

        new() { Name = "Chien-Loup", Camp = Camp.Villageois, Emoji = "🐕",
            ShortDescription = "Choisit son camp (villageois par défaut).",
            FullDescription = "Le joueur peut choisir s’il souhaite être Loup-Garou ou Cogniticien. Par défaut, il est Cogniticien. S’il offre un petit cadeau (ou un gros) à l’un des MJ, il devient Loup-Garou s’il le souhaite. Son camp est perçu “Loups” dans tous les cas.",
            HasFixedTimeAction = false },

        new() { Name = "Cupidon", Camp = Camp.Villageois, Emoji = "💘",
            ShortDescription = "Forme le couple du début de partie.",
            FullDescription = "Offre une lettre romantique signée de l'autre joueurs à deux joueurs de son choix pour former le couple avant le 4ᵉ jour (sinon le couple est tiré au sort par le MJ). Révèler le couple au MJ. Le couple ne peut gagner qu'ensemble, avec Cupidon si l'un meurt l'autre aussi.",
            HasFixedTimeAction = false },

        new() { Name = "Le squatteur", Camp = Camp.Villageois, Emoji = "🛋️",
            ShortDescription = "S'installe chez un autre joueur pour 24h.",
            FullDescription = "Choisit chaque jour un joueur différent chez qui \"squatter\" pour 24h : chez un villageois, il meurt aussi si celui-ci est mangé ; chez un solo de même et il obtient une partie de ses pouvoirs ; chez un loup, il ne peut pas être mangé cette nuit-là. S’il est voté par les Loups, il n’est pas éliminé tant qu'il squatte.",
            HasFixedTimeAction = true },

        new() { Name = "Rival", Camp = Camp.Villageois, Emoji = "💔",
            ShortDescription = "Peut prendre la place d'un membre du couple.",
            FullDescription = "Peut offrir une rose à un membre du couple pour prendre la place de l'autre qui est éliminé, ou récupérer automatiquement la place s'il a voté pour éliminer un membre du couple lors du vote du village. IL doit donc gagner avec sont nouveau amant et sans la cupidon. Cependant, si le couple meurt sans qu’il ait voté pour le membre éliminé, il perd son droit de vote pendant 3 jours.",
            HasFixedTimeAction = false },

        new() { Name = "Le pot de colle", Camp = Camp.Villageois, Emoji = "🤗",
            ShortDescription = "Gagne des votes en faisant des câlins.",
            FullDescription = "Une fois par jour, vous pouvez faire un câlin à une personne pour doubler votre vote et celui à qui il fait le câlin. Vous ne pouvez pas faire deux fois un câlin à la même personne (ou du moins vous n’en retirerez aucun avantage supplémentaire). Vous devez informer un MJ de la personne à qui vous avez fait un câlin.",
            HasFixedTimeAction = false },

        new() { Name = "Idiot du village", Camp = Camp.Villageois, Emoji = "🤡",
            ShortDescription = "Survit à son propre vote du village, une fois révélé.",
            FullDescription = "S’il est ciblé par le vote du village, il ne meurt pas, mais son rôle est révélé aux yeux de tous. Il perd alors son droit de vote jusqu’au moment où il reste moins de 10 joueurs. S’il est ciblé une deuxième fois par le village, il est éliminé.",
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
            FullDescription = "Les deux sœurs se connaissent et sont alliées de confiance. Lorsqu’une des deux meurt, l’autre peut choisir de découvrir soit l’identité du tueur, soit son rôle.( pour le vote du village la première personne à avoir voter contre elle est celle où l'autre sœur obtient l'information)",
            HasFixedTimeAction = false },

        new() { Name = "Frère", Camp = Camp.Villageois, Emoji = "👨‍👦‍👦",
            ShortDescription = "Peuvent partager leur rôle entre eux. ils auront chacun l'identiter d'un seul de leur deux autres frères",
            FullDescription = "Les trois frère se connaissent et sont alliées de confiance. Lorsqu’un des trois meurt, chaque autre frère peut choisir de découvrir soit l’identité du tueur, soit son rôle.( pour le vote du village la première personne à avoir voter contre elle est celle où l'autre sœur obtient l'information)",
            HasFixedTimeAction = false },

        new() { Name = "Montreur d'ours", Camp = Camp.Villageois, Emoji = "🐻",
            ShortDescription = "Révèle qui était autour de lui.",
            FullDescription = "Chaque jour, à 12h30, il envoie au MJ la liste des personnes présentes autour de lui dans un rayon de 3 mètres. ( il faut au moins 4 personnes annoncés au minimum ). Si au moins un Loup-Garou se trouve parmi ces personnes, des « grrrr » apparaissent dans le groupe du village.( le nombre de grrr est le nombre de loup ). Si le Montreur d’Ours se fait infecter, des « grrrr » apparaissent dans le groupe du village jusqu’à l’élimination de ce dernier.",
            HasFixedTimeAction = false },

        new() { Name = "Corbeaux", Camp = Camp.Villageois, Emoji = "🐦‍⬛",
            ShortDescription = "Dispose de votes bonus à distribuer.",
            FullDescription = "Chaque semaine, il dispose de 5 votes supplémentaires. Pour les utiliser, il doit informer un MJ avant le vote du village. Il doit lui donner la cible et le nombre de votes qu’il souhaite utiliser. Les votes supplémentaires ne peuvent pas être cumulés d’une semaine sur l’autre.",
            HasFixedTimeAction = false },

        new() { Name = "Servante dévouée", Camp = Camp.Villageois, Emoji = "🙇",
            ShortDescription = "Peut prendre la place d'un joueur voté avant sa mort.",
            FullDescription = "Une fois dans la partie, elle peut choisir de révéler son rôle dans le groupe du village avant l’élimination du joueur désigné par le vote ou mort par les loups. Elle récupère alors son rôle et ses pouvoirs actuels, ainsi que les bonus et malus qui y sont associés.",
            HasFixedTimeAction = false },

        new() { Name = "Trublion", Camp = Camp.Villageois, Emoji = "🎭",
            ShortDescription = "Peut deviner un tiers des rôles de la partie.",
            FullDescription = "Quand les MJ l'y autorisent, peut deviner un tiers des rôles des joueurs. S'il a raison, les rôles concernés sont échangés. si il y a plus de 15 joueurs. si il se trompe il meurt instantanément.",
            HasFixedTimeAction = false },

        // ---------- SOLO ----------
        new() { Name = "Assassin", Camp = Camp.Solo, Emoji = "🗡️",
            ShortDescription = "Joue seul, tue s'il a marqué sa cible.",
            FullDescription = "Doit gagner seul. Une fois par jour, vous pouvez tuer un joueur en lui donnant un objet décidé par les MJ chaque jour. S’il y a tenu l’objet en main, il peut décider de le tuer pendant la nuit.",
            HasFixedTimeAction = false },

        new() { Name = "Hypnotiseur", Camp = Camp.Solo, Emoji = "🌀",
            ShortDescription = "Force un joueur à voter pour une cible précise.",
            FullDescription = "Doit gagner seul. Chaque jour, il choisit deux joueurs qu’il envoie au MJ : un hypnotisé et une cible. Durant la journée, lors des votes du village et du vote des loups (s’il appartient à ce camp), l’hypnotisé devra obligatoirement voter contre la cible désignée par l’hypnotiseur.",
            HasFixedTimeAction = false },

        new() { Name = "Joueur de flûte", Camp = Camp.Solo, Emoji = "🎼",
            ShortDescription = "Enchante des joueurs pour gagner seul.",
            FullDescription = "Doit enchanter tout les joueurs restant. Une fois par jour, vous pouvez enchanter un joueur en lui faisant écouter de la musique (avec son accord). Il devient alors enchanté et ne peut ni voter contre vous ni encourager un vote contre vous, lors des votes du village ou des loups, sous peine de mort immédiate.",
            HasFixedTimeAction = false },

        new() { Name = "Ange", Camp = Camp.Solo, Emoji = "👼",
            ShortDescription = "Ange protecteur ou ange déchu, tel est la question.",
            FullDescription = "Au début de la partie, l’Ange choisit une cible (elle en est informée sans connaître l’identité de l’Ange). Il choisit ensuite d’être Ange Protecteur ou Ange Déchu. \n Ange Protecteur : vous devez garder votre cible en vie. Pour la protéger, vous devez lui montrer un papier « Je te protège ». Tant qu’elle est en vie et protégée, vous gagnez +1 vote par jour, cumulable jusqu’à 3. Si la cible meurt, votre rôle est révélé et vous perdez vos votes bonus. Si l’ange meurt le rôle de sa cible est révélé si elle était protégé. \n Ange Déchu : vous devez tuer votre cible. Pour la tuer, un papier « [rôle de la cible], tu es mort(e) » doit se retrouver entre ses mains, peu importe la manière. Une fois la cible morte par vous, vous gagnez +1 vote par jour, cumulable jusqu’à 3. Si elle meurt autrement, votre rôle est révélé et vous perdez vos votes bonus. \n Vous devez annoncer au MJ avant chaque vote si vous utilisez un vote supplémentaire et sur qui. Sinon le vote est simple par défaut.",
            HasFixedTimeAction = false },
    };
}