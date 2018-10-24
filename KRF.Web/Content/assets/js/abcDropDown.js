(function ($) {
    $.fn.abcDropDown = function (options) {
        var identfy = $(this).attr('id');
        // alert(identfy);
        var settings = $.extend({
            // These are the defaults.
            ddwidth: "100%",
            backgroundColor: "white"
        }, options);
        $('#' + identfy).css("width", settings.ddwidth)
        function DropDown(el) {
            this.dd = el;
            this.initEvents();
        }
        DropDown.prototype = {
            initEvents: function () {
                var obj = this;
                obj.dd.on('click', function (event) {
                    $(this).toggleClass('active');
                    event.stopPropagation();
                });
            }
        }

        $(function () {
            var dd = new DropDown($('#' + identfy));
            $(document).click(function () {
                $('.abcDropDown').removeClass('active');
            });

            $('#' + identfy + ' ul li a').on("click", function () {
                // alert($(this).text());
                $('#' + identfy).find(".selected").removeClass("selected");
                $(this).addClass("selected");
                $('#' + identfy).find('span').text($(this).text());
            });
        });
    };

    $.fn.abcDropDownRefresh = function (options) {
        var identfy = $(this).attr('id');
        $('#' + identfy + ' ul li a').on("click", function () {
            // alert($(this).text());
            $(this).addClass("selected").siblings().removeClass("selected");
            $('#' + identfy).find('span').text($(this).text());
        });
    }
}(jQuery));


















