var FollowingService = function() {
    var createFollowing = function(followeeId, done, fail) {
        $.post("/api/Following", { FolloweeId: followeeId })
            .done(done)
            .fail(fail);
    };

    var deleteFollowing = function (followeeId, done, fail) {
        $.post("/Account/UnFollowArtist", { artistId: followeeId })
            .done(done)
            .fail(fail);
    };

    return {
        createFollowing: createFollowing,
        deleteFollowing: deleteFollowing
    };
}();