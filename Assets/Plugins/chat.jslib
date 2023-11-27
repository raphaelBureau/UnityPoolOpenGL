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
   GetUserInfo: function () {
      var str = GetUserData();
      var bufferSize = lengthBytesUTF8(str) + 1;
      var buffer = _malloc(bufferSize);
      stringToUTF8(str, buffer, bufferSize);
      return buffer;
  },
});