mergeInto(LibraryManager.library, {

  sendMessage: function (mess) {
    ReceiveMessage(UTF8ToString(mess));
  },
    setName: function (name) {
      SetName(UTF8ToString(name));
  },
   IsMobile: function () {
    return (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent));
  },
});