var GigDetailsController = function(followingService) {
    var followButton;

    var fail = function () {
        alert("Something failed");
    };

    var done = function () {
        var text;

        if ($.trim(followButton.text()) === "Follow")
            text = "Following";
        else if ($.trim(followButton.text()) === "Following")
            text = "Follow";

        followButton.toggleClass("btn-default").toggleClass("btn-info").text(text);
    };

    var toggleFollowing = function(e) {
        followButton = $(e.target);
        var followeeId = followButton.attr("data-user-id");

        if (followButton.hasClass("btn-default")) 
            followingService.createFollowing(followeeId, done, fail);
        else
            followingService.deleteFollowing(followeeId, done, fail);
    };

    var init = function() {
        $(".js-toggle-follow").click(toggleFollowing);
    };

    return {
        init: init
    };

}(FollowingService)