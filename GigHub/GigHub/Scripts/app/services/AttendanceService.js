var AttendanceService = function () {
    var createAttendance = function (gigId, done, fail) {
        $.post("/api/attendances/Attend", { gigId: gigId })
            .done(done)
            .fail(fail);
    };

    var deleteAttendance = function (gigId, done, fail) {
        $.post("/Gigs/Miss", { gigId: gigId })
            .done(done)
            .fail(fail);
    };

    return {
        createAttendance: createAttendance,
        deleteAttendance: deleteAttendance
    };
}();
