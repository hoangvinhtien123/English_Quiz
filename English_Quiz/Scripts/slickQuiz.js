$(document).ready(function () {
    $('.Quiz').slick({
        slidesToShow: 1,
        slidesToScroll: 1,
        arrows: false,
        fade: true,
        asNavFor: '.nav-item'
    });
    $('.nav-item').slick({
        slidesToShow: 3,
        slidesToScroll: 1,
        asNavFor: '.Quiz',
        dots: true,
        focusOnSelect: true
    });
});