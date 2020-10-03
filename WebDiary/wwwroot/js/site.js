// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {

    $('#mainContent').on('click', '.pager a', function () {
        var url = $(this).attr('href');

        $('#mainContent').load(url);

        return false;
    })

});