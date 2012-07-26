(function() {
  var Bootstrapper;

  Bootstrapper = (function() {

    function Bootstrapper() {}

    Bootstrapper.prototype.setup = function() {
      var bodyId, startPage;
      bodyId = $("body").attr('id');
      if (bodyId === "Startpage") {
        startPage = new Startpage;
        startPage.load();
      }
    };

    return Bootstrapper;

  })();

  ($(document)).ready(function() {
    var strapper;
    strapper = new Bootstrapper;
    return strapper.setup();
  });

}).call(this);
