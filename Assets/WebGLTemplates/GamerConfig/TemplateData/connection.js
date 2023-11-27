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

var user = {"id":-1,"gamertag":"guest","img":"default.png"};
async function loadProfile() {

let idUser = getCookie("id");
let authToken = getCookie("authToken");
if(idUser == "" || authToken == "") {
    //guest user, demander d'entrer un nom
}
else{
user = await ajaxJSON(`/user/${idUser}`, "GET", { id: idUser, authToken: authToken });
}
console.log(user);
}
loadProfile();

function GetUserData() {
  return JSON.stringify(user);
}


