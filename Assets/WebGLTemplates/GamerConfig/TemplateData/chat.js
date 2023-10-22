
let messages = []; //message.name, message.text, message.channel
var currentChannel = 0; //0 == all, 1== game chat

var nom = "Joueur Anonyme";

function SetName(name = "Joueur Anonyme") {
    nom = name;
}

function SendMessage() {
    let message = {};
    message.nom = nom;
    message.text = $(".chatInput").val();
    message.channel = currentChannel;
    $(".chatInput").val("");

    if (message.text.length > 0) {
        message = JSON.stringify(message);
        if(currentChannel != 0) {
        ReceiveMessage(message);
        }
        unityInstance.SendMessage("Main", "EmitMessage", message);
    }
}

function AppendMessage(message) {
    let html = "<div><span class='messageName'>" + message.nom + " :</span><span class='messageText'>" + message.text + "</span></div>";
    $(".chatMessages").append(html);
}
function AppendAllMessage(message) {
    let chan = message.channel;
    switch (chan) {
        case 0:
            chan = "Tous";
            break;
        case 1:
            chan = "Jeu";
            break;
    }
    let html = "<div><span class='messageName'>" + message.nom + "</span><span class='messageName'> [" + chan + "] : </span><span class='messageText'>" + message.text + "</span></div>";
    $(".chatMessages").append(html);
}

function JoinChannel() {
    let channel = prompt("Entrez le nom du canal", "");
    if (channel.length > 0) {
        unityInstance.SendMessage("Main", "JoinChannel", channel);
        let html = `<div class="channelLabel" onclick="ChangeChannel('` + channel + `')" id="` + channel + `">` + channel + `</div>`;
        $(".chatChannels").append(html);
    }
}

function ChangeChannel(channel = 0) {
    $("#" + currentChannel).removeClass('channelSelected');
    $("#" + channel).addClass('channelSelected');
    $(".chatMessages").empty();
    currentChannel = channel;
    if (channel == 0) //all
    {
        messages.forEach((mess) => {
            AppendAllMessage(mess);
        });
    }
    else {
        messages.forEach((mess) => {
            if (mess.channel == currentChannel) {
                AppendMessage(mess);
            }
        });
    }
    ScrollChat();
}

$(".chatInput").keypress((e) => {
    if (e.which == 13) {
        SendMessage();
    }
});

function ReceiveMessage(messageJson = "") {
    console.log(messageJson);
    if (messageJson.length > 2) {
        let message = JSON.parse(messageJson);

        if (message.hasOwnProperty("nom") && message.hasOwnProperty("text") && message.hasOwnProperty("channel")) {
            messages.push(message);
            if (message.channel == currentChannel || currentChannel == 0) {
                if (currentChannel == 0) {
                    AppendAllMessage(message);
                }
                else {
                    AppendMessage(message);
                }
                ScrollChat();
            }
        }
    }
}

function ScrollChat() {
    $(".chatMessages").scrollTop($(".chatMessages")[0].scrollHeight);
}

function ToggleChat() {
    $(".chatBox").toggle();
}

console.log("chat.js loaded");