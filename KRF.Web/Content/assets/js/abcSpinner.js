(function ( $ ) {
$.fn.abcSpinner = function( options ) {
var settings = $.extend({
// These are the defaults.
}, options );
// Greenify the collection based on the settings variable.
//return this.css({color: settings.color,backgroundColor: settings.backgroundColor});
var currentVal = 0;
if ($(this).attr("data-val") != undefined) {
    currentVal = $(this).attr("data-val");
    $(this).attr("data-val", currentVal);
}
$("<div class='spinner'><input type='text' class='spinner_inputText' value='"+ currentVal + "'/><div class='spinner_btnHolder'><a class='spinner_btnUp' href='#'></a><a class='spinner_btnDown' href='#'></a></div></div>").appendTo(this);


 $(".spinner_btnUp").click( function(){	
	var spinVal=$(this).parent().prev().val();
	if(spinVal>=0){
	spinVal++
	 $(this).parent().prev().val(spinVal);
	 $(this).parent().parent().parent().attr( "data-val", spinVal );
	}
 });
 
$(".spinner_btnDown").click( function(){	
	var spinVal=$(this).parent().prev().val();
	if(spinVal>0){
	spinVal--
	 $(this).parent().prev().val(spinVal);
	  $(this).parent().parent().parent().attr( "data-val", spinVal );
	}
 });
 
 $(".spinner_inputText").change( function(){	
 $(this).parent().parent().attr( "data-val",  $(this).val() );
 });
 
};
}( jQuery ));