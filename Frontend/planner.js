/* Events */
$(function () {
    $(".features").sortable(
    {
        connectWith: '.features',
        cancel: '.release'
    });
    $(".userstories").sortable(
    {
        connectWith: '.userstories'
    });
    $(".backlogs").sortable(
    {
        connectWith: '.backlogs'
    });
    $(".tasks").sortable();

    $('.collapsable > .fa-plus-square-o').click(function () {
        $(this).parent().toggleClass('collapsed');
    })

});