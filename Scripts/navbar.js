$(document).ready(function () {
    // Get the current URL path
    var path = window.location.pathname;
    // Select all nav links
    $('.nav-link').each(function () {
        // Get the href attribute of the link
        var href = $(this).attr('href');
        // Check if the current path matches the link's href
        if (path === href) {
            // Add the active-link class to the matching link
            $(this).addClass('active-link');
        }
    });
});
