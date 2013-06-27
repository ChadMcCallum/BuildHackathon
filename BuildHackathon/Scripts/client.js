$(document).bind("mobileinit", function () {
    $.mobile.defaultPageTransition = 'turn';
});

$().ready(function () {
    window.hubProxy = $.connection.gameHub;
    window.buttonTemplate = _.template($('#button-template').html());
    window.onbeforeunload = function() {
        $.connection.hub.stop();
    };

    bodyMinHeightFix();

    //client methods
    hubProxy.client.NewQuestion = function (question) {
        //init UI with data
        var lastRow = null;
        $('#buttons').empty();
        $.each(question.PlayerOptions, function(i, e) {
            if (i % 2 == 0) {
                lastRow = $("<div class='ui-grid-a'></div>");
                $('#buttons').append(lastRow);
            }
            var button = $(buttonTemplate(e));
            if (i % 2 != 0) {
                button.removeClass('ui-block-a').addClass('ui-block-b');
            }
            lastRow.append(button);
        });
        $('#buttons').trigger('create');
        
        $.mobile.changePage($('#guess'), { changeHash: false });
    };
    hubProxy.client.Wait = function() {
        $.mobile.changePage($('#wait-round'), { changeHash: false });
    };
    hubProxy.client.Timeout = function (args) {
        updateResult(args);
        $.mobile.changePage($('#timeout'), { changeHash: false });
    };
    hubProxy.client.Right = function (args) {
        updateResult(args);
        $.mobile.changePage($('#correct'), { changeHash: false });
    };
    hubProxy.client.Wrong = function (args) {
        updateResult(args);
        $.mobile.changePage($('#fail'), { changeHash: false });
    };
    hubProxy.client.EndGame = function (msg) {
        if (msg.indexOf(window.team) >= 0) {
            $('#win').show();
            $('#lose').hide();
        } else if (msg.indexOf("Won") >= 0) {
            $('#win').hide();
            $('#lose').show();
        } else {
            $('#win').hide();
            $('#lose').hide();
        }
        $('#game-over-message').html(msg);
        $.mobile.changePage($('#gameover'), { changeHash: false });
    };
    hubProxy.client.ResetScore = function() {
        $("[data-role=footer]").html("<p style='text-align: right; padding-right: 10px;'>Your Score: 0 - Team Score: 0</p>");
    };

    $.connection.hub.start().done(function () {
        $("[data-role=footer]").html("<p>Connected</p>");
    });

    $('#start-playing').click(function () {
        JoinGame();
    });
    
    $(document).on('click', '.guess', function () {
        var nameOfGuess = this.id;
        hubProxy.server.guess(nameOfGuess).done(function() {
            $.mobile.changePage($('#waiting'), { changeHash: false });
        });
        
        $.mobile.changePage($('#waiting'), { changeHash: false });
    });
});

function updateResult(args) {
    $('.actual > img').attr('src', args.Actual.ImageURL);
    $('.actual > .name').html(args.Actual.Name);
    $("[data-role=footer]").html("<p style='text-align: right; padding-right: 10px;'>Your Score: " + args.MyScore + " - Team Score: " + args.TeamScore + "</p>");

}

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

function JoinGame() {
    var gameID = getParameterByName("id");
    hubProxy.server.joinGame(gameID, $('#twitterName').val()).done(function (team) {
        window.team = team;
        if (team == "Red") {
            //set background to red
            $('[data-role=content]').css('background-color', '#E86850');
        } else {
            $('[data-role=content]').css('background-color', '#587498');
        }
        $.mobile.changePage($('#wait-round'), { changeHash: false });
    }).fail(function (e) { alert(e); });
}

//function layoutPage() {
//    var width = $(window).width();
//    if (width < 480) {
//        //vertical layout
//        $('#buttons')
//    } else {
//        //horizontal layout
//    }
//}

function bodyMinHeightFix() {
    var isWp7 = window.navigator.userAgent.indexOf("IEMobile/9.0") != -1;

    if (!isWp7) return;

    // portrait mode only
    if (window.innerHeight <= window.innerWidth) return;

    var zoomFactorW = document.body.clientWidth / screen.availWidth;

    // default value (web browser app)
    var addrBarH = 72;

    // no app bar in web view control
    if (typeof window.external.Notify !== "undefined") {
        addrBarH = 0;
    }

    var divHeightInDoc = (screen.availHeight - addrBarH) * zoomFactorW;
    //$("body")[0].style.minHeight = divHeightInDoc + 'px';

    var page = $("div[data-role='page']");
    if (page.length > 0)
        page[0].style.setProperty("min-height", divHeightInDoc + "px", 'important');

}