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
    $(".tasks").sortable();

    $('.collapsable > .fa-plus-square-o').click(function () {
        $(this).parent().toggleClass('collapsed');
    })

});