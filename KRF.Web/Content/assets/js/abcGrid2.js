(function ($) {
    $.fn.abcGrid2 = function (options) {
        // This is default options.
		//alert(this.attr("id"));
        var settings = $.extend({
            // These are the defaults value.
            numberOfColumn: 6,
            columnWidth: []
        }, options);
        //alert(settings.dataSource[0].split(','));
        var numberOfKeys = settings.columnName.length;
        var toatalColWidth = settings.columnWidth;
        var totalRecord = settings.dataSource.length;
        var myRecord = [settings.dataSource];
        this.addClass("abcGrid");
       

        $("<div id='abcGrid_Row2' class='tHead'></div>").appendTo(this);
        $("<div id='tbody2' class='tbody scrollbox'></div>").appendTo(this);
        $('.tbody').css("height", settings.gridHeight);
        //this loop is for heading only	
        for (i = 0; i < numberOfKeys; i++) {
            $("<div id='headCol2_" + i + "' class='tcol' style='width:calc(" + settings.columnWidth[i] + " - 8px)'><span class='dataCol_DataHead'>" + settings.columnName[i] + "</span><a id='" + settings.columnName[i].replace(/\s/g, '') + "' class='short_arrow' href='#'></a></div>").appendTo("#abcGrid_Row2");
            
        }

        var myD = [];
        //--------------------------------
        for (var i = 0; i < totalRecord; i++) {
           
            myD.push([settings.dataSource[i].JobName, settings.dataSource[i].CustomerName, settings.dataSource[i].StartDate, settings.dataSource[i].Time, settings.dataSource[i].vdate, settings.dataSource[i].Time]);
        }

        var theTable = "";
        for (var j = 0; j < totalRecord; j++) {
            theTable += "<div id='dataRow2_" + j + "' class='dataRow'>";
            for (var k = 0; k < numberOfKeys; k++) {
                
                theTable += "<div class='tcol " + settings.columnName[k] + "' style='width:calc(" + settings.columnWidth[k] + " - 8px)'><a class='dataCol_DataHead' href='#' onclick='showInmateData();'>" + myD[j][k] + "</a></div>";
              
            }
            theTable += '</div>';
        }
        //Finally appended the whole string to the table
        
        $('#tbody2').append(theTable);
		//working fine till here -----
		
		
   
  
  
		//write above it--------
    };
}(jQuery));