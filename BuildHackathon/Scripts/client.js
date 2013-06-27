$().ready(function () {
    window.hubProxy = $.connection.gameHub;
    window.buttonTemplate = _.template($('#button-template').html());

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
        
        $.mobile.changePage($('#guess'));
    };
    hubProxy.client.Timeout = function() {
        $.mobile.changePage($('#timeout'));
    };
    hubProxy.client.Right = function() {
        $.mobile.changePage($('#correct'));
    };
    hubProxy.client.Wrong = function() {
        $.mobile.changePage($('#fail'));
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
            $.mobile.changePage($('#waiting'));
        });
        
        $.mobile.changePage($('#waiting'));
    });
});

function JoinGame() {
    hubProxy.server.joinGame("E2865CED-1543-430D-BF43-2D5F56E5FE16", $('#twitterName').val()).done(function (team) {
        if (team == "Red") {
            //set background to red
            $('[data-role=content]').css('background-color', '#E86850');
        } else {
            $('[data-role=content]').css('background-color', '#587498');
        }
        $.mobile.changePage($('#wait-round'));
    }).fail(function (e) { alert(e); });
}