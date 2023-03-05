$(document).ready(() => {
    $('#nav').css('overflow-y', 'auto');
    $('.open').click(() => {
        $('.open').css('display', 'none');
        $('.backimg').css('margin-right', '-300px');
    });
    $('.close').click(() => {
        $('.open').css('display', 'flex');
        $('.backimg').css('margin-right', '0px');
    });
});