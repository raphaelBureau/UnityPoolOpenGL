function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for(let i = 0; i <ca.length; i++) {
      let c = ca[i];
      while (c.charAt(0) == ' ') {
        c = c.substring(1);
      }
      if (c.indexOf(name) == 0) {
        return c.substring(name.length, c.length);
      }
    }
    return "";
  }
  // https://www.w3schools.com/js/js_cookies.asp

  function ajaxJSON(path, method, object) {
    let val;
    if (method == "GET") {
      val = object;
    } else {
      val = JSON.stringify(object)
    }
      return new Promise(resolve => {
          $.ajax({
              url : API_URL + path,
              method: method,
              data: val,
              contentType: "application/json",
              dataType: "json",
              success: (data) => { resolve(data) },
              error: (xhr, status, error) => {
                  resolve(false);
              }
          });
      });   
  }

  let API_URL = "https://bureau.blue:8000/api";

var auth = false;

var user = {"id":-1,"gamertag":"guest","img":"default.png","lobby":""};
async function loadProfile() {

let idUser = getCookie("id");
let authToken = getCookie("authToken");
if(idUser == "" || authToken == "") {
    //guest user, demander d'entrer un nom
}
else{
user = await ajaxJSON(`/user/${idUser}`, "GET", { id: idUser, authToken: authToken });
if(user!=false) {
  auth = true;
}
}
console.log(user);
}
loadProfile();

function GetUserData() {
  return JSON.stringify(user);
}

function SendConnectionRequest() {
  if(!auth) {
    //demander les infos au user
    let name = prompt("Entrez votre nom", "");
    if (name.length > 0) {
      user.gamertag = name;
    }
  }
  let lobby = prompt("entrez le code de partie privee (laissez vide pour partie publique)");
  if(lobby.length > 0 && lobby.length < 30) {
    user.lobby = lobby;
  }
  unityInstance.SendMessage("Main", "JoinMatchmaking");
}



