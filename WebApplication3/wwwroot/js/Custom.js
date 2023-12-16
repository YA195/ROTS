jQuery(document).ready(function ($) {
    // Nice Select
    $('select').niceSelect();

    // Isotope Initialization
    var $grid = $(".grid").isotope({
        itemSelector: ".all",
        percentPosition: false,
        masonry: {
            columnWidth: ".all"
        }
    });
    function myMap() {
        var mapProp = {
            center: new google.maps.LatLng(29.943760116084217, 31.062401306806148),
            zoom: 18,
        };
        var map = new google.maps.Map(document.getElementById("googleMap"), mapProp);
    }

    $(document).ready(function () {
        if (window.location.hash === '#about') {
            scrollToSection('about');
        }

        $('a[href^="#"]').on('click', function (event) {
            var target = $(this.getAttribute('href'));
            if (target.length) {
                event.preventDefault();
                scrollToSection(target.attr('id'));
            }
        });
    });

    function scrollToSection(sectionId) {
        var section = document.getElementById(sectionId);
        if (section) {
            section.scrollIntoView({ behavior: 'smooth' });
        }
    }

    // Filter Menu Click Event
    $('.filters_menu li').click(function () {
        console.log('Filter clicked');

        // Remove 'active' class from all menu items and add it to the clicked item
        $('.filters_menu li').removeClass('active');
        $(this).addClass('active');

        // Get the data-filter attribute of the clicked item
        var data = $(this).attr('data-filter');
        console.log('Filter value:', data);

        // Use Isotope to filter the items
        $grid.isotope({
            filter: data
        });
    });

    // Owl Carousel for the client section
    $(".client_owl-carousel").owlCarousel({
        loop: true,
        margin: 0,
        dots: false,
        nav: true,
        autoplay: true,
        autoplayHoverPause: true,
        navText: [
            '<i><box-icon name=\'chevron-left\' color=\'#ffffff\'></box-icon></i>',
            '<i><box-icon name=\'chevron-right\' color=\'#ffffff\'></box-icon></i>'
        ],

        responsive: {
            0: {
                items: 1
            },
            768: {
                items: 2
            },
            1000: {
                items: 2
            }
        }


    });
    $("#menu-toggle").click(function (e) {
        e.preventDefault();
        $("#wrapper").toggleClass("toggled");
    });
    $("#menu-toggle-2").click(function (e) {
        e.preventDefault();
        $("#wrapper").toggleClass("toggled-2");
        $('#menu ul').hide();
    });

    function initMenu() {
        $('#menu ul').hide();
        $('#menu ul').children('.current').parent().show();
        //$('#menu ul:first').show();
        $('#menu li a').click(
            function () {
                var checkElement = $(this).next();
                if ((checkElement.is('ul')) && (checkElement.is(':visible'))) {
                    return false;
                }
                if ((checkElement.is('ul')) && (!checkElement.is(':visible'))) {
                    $('#menu ul:visible').slideUp('normal');
                    checkElement.slideDown('normal');
                    return false;
                }
            }
        );
    }
    $(document).ready(function () {
        initMenu();
    });  


});

