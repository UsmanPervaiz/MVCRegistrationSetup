/// <reference path="jquery-1.10.2.js" />

$(function () {
    $("#searchTerm").autocomplete({
        source: "/LogInLogOut/Home/AutoCompleteSearch"
    });
});

