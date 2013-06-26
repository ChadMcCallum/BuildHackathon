$().ready(function () {
    window.hubProxy = $.connection.gameHub;
    hubProxy.client.NewQuestion = function(question) {

    };
    $.connection.hub.start().done(function() { alert("Connected!"); });

    $('#start-playing').click(function () {
        JoinGame();
    });
    
    $('.guess').click(function() {
        $.mobile.changePage($('#waiting'));
        setTimeout(function() {
            $.mobile.changePage($('#correct'));
            setTimeout(function() {
                $.mobile.changePage($('#guess'));
            }, 2000);
        }, 2000);
    });
});

function JoinGame() {
    hubProxy.server.joinGame("E2865CED-1543-430D-BF43-2D5F56E5FE16", $('#twitterName').val()).done(function (team) {
        alert(team);
        $.mobile.changePage($('#wait-round'));
    }).fail(function (e) { alert(e); });
}