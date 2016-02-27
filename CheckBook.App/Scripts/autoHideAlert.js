$(function() {
    var alertTimeout;
    dotvvm.events.afterPostback.subscribe(function () {
        if (alertTimeout) {
            window.clearTimeout(alertTimeout);
        }
        $(".alert:visible").slideDown();
        alertTimeout = window.setTimeout(function() {
            $(".alert").slideUp();
        }, 5000);
    });
});