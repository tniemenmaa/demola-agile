/* Events */
$(function () {
    $(".planner").sortable();
    $(".planner").disableSelection
    $('#story').click(function () {
        $('.task').toggle();
    })
});