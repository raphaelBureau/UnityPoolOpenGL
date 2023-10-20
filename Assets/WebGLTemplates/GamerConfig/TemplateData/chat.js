console.log("chat.js");

var nom = "Joueur Anonyme";

function SetName(name = "Joueur Anonyme") {
    nom = name;
}

function SendMessage() {
    let message = {};
    message.nom = nom;
    message.text = $(".chatInput").val();
    $(".chatInput").val("");

    if(message.text.length > 0) {
    message = JSON.stringify(message);
    ReceiveMessage(message);
    unityInstance.SendMessage("Main","EmitMessage",message);
    }
}

$(".chatInput").keypress( (e) => {
    if(e.which == 13) {
        SendMessage();
    }
});

function ReceiveMessage(messageJson = "") {
    console.log(messageJson);
    if(messageJson.length>2) {
        let message = JSON.parse(messageJson);

        let html = "<div><span class='messageName'>"+ message.nom +" :</span><span class='messageText'>" + message.text + "</span></div>";

        $(".chatMessages").append(html);
    }
}

function ToggleChat() {
    $(".chatBox").toggle();
}