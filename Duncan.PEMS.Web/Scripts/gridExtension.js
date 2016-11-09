(function ($) {
    var defaultFrozenCol = 1; //number of column to freeze
    var scrollDelay = 50; //delay in milliseconds
    var scrollStep = 10; //how big scroll step should be before taking action

    var hiddenColCount = 0;
    var totalColWidth = 0;
    var currentScrollPos = 0;
    var defaultWidth = 300;

    var gridHeaderTable;
    var gridContentTable;
    var tableHeaderGroupCol;
    var tableHeaderCol;
    var tableContentGroupCol;
    var tableContentCol;
    var totalColCount;
    var resetPos = false;
    var resetColumnWidths = false;
    var sp = 0;
    var frozenColWidth = 0;
    var columnWidthArr = new Array();
    var fullWidthArr = new Array();
    var grid;
    var frozenContainer;
    var frozenInnerContainer;
    var frozenColCount;
    var browserVersion = parseInt($.browser.version);

    var totalVisibleWidth = 0;
    var totalGridWidth = 0;
    var currentGridName;
    //reset column widths, this is for multiple grids on the same page. so the widths dont get overriden by one another
    $.fn.clearColumnWidths = function (gridName) {
        if (currentGridName != gridName) {
            currentGridName = gridName;
            resetColumnWidths = true;
            grid = $(this);
            gridHeaderTable = null;
            gridContentTable = null;
            tableHeaderGroupCol = null;
            tableHeaderCol = null;
            tableContentGroupCol = null;
            tableContentCol = null;
            totalColCount = null;
            totalGridWidth = null;
            totalVisibleWidth = null;
            $('#gridfrozenContainer').remove();

            frozenContainer = null;
            frozenInnerContainer = null;

            columnWidthArr = new Array();
            fullWidthArr = new Array();
        }
    };



    //Hide columns
    //hide the last N columns. this has to be called before the freezegridcolumn or the updatewidths methods.
    $.fn.hideLastColumns = function (hideColumnsCount) {
        var currentGrid = $(this);
        var ghtable = currentGrid.find(".k-grid-header table");
        var gctable = currentGrid.find(".k-grid-content table");
        var thColGroup = $(ghtable).find("colgroup col");
        var tcColGroup = $(gctable).find("colgroup col");
        var totalCount = $(thColGroup).length;
        for (var i = 1; i <= hideColumnsCount; i++) {
            $(thColGroup).eq(totalCount -i).width(0);
            $(tcColGroup).eq(totalCount - i).width(0);
        }
    };
    //create seter grid freeze
    //Create freezeGridColumn function
    $.fn.freezeGridColumn = function (frozenColumnsCount) {
        initBrowsers();
        frozenColCount = (frozenColumnsCount != null) ? frozenColumnsCount : defaultFrozenCol;
        grid = $(this);
        initFreezeColumns();
    };

    $.fn.UpdateWidths = function(i, frzColCount, newWidth, oldWidth) {
        //update the arrays with the correct widths

        //do not allow them to resize locked columns
        if (i < frzColCount) {
            $(tableHeaderGroupCol).eq(i).width(oldWidth);
            $(tableContentGroupCol).eq(i).width(oldWidth);
        } else {
            
            //test to make sure there is something in the array before we make modifications
            var index = i - frzColCount;
            if (columnWidthArr.length > 0) {
                columnWidthArr[index] = newWidth;
            }
            if (fullWidthArr.length > 0) {
                fullWidthArr[i] = newWidth;
            }
        }

        //Kendo resizes the table widths as well, so we need to reove the Style width attribute as well
        gridHeaderTable.attr("style", "");
        gridContentTable.attr("style", "");
    };
    

    //Initialise all necessary grid info to begin frozen column implementation
    function initFreezeColumns() {

        getGridInfo();
        
        //only do this if you need to
        if (totalGridWidth > totalVisibleWidth) {
            createScrollContainer();
        //    getHiddenColCount();
            createColumnWidthList();
            unhideAllColumns();

            if (resetPos) {
                //determine scroll percent (frozen inner container * sp)
                var csp = sp * $(frozenInnerContainer).width();
                $(frozenContainer).scrollLeft(csp);
                performFreezeColumns();
                resetPos = false;
            } else {
                currentScrollPos = 0;
            }
            // currentScrollPos = 0;

            var hasScrolled = false;
            performFreezeColumns();
            $(frozenContainer).scroll(function(e) {
                e.preventDefault();
                var scrollDiff = Math.abs(currentScrollPos - $(frozenContainer).scrollLeft());
                if (scrollDiff >= scrollStep) {
                    hasScrolled = false;
                    $(frozenContainer).delay(scrollDelay).queue(function() {
                        if (!hasScrolled) {
                            performFreezeColumns();
                            hasScrolled = true;
                        }
                        $(this).dequeue();
                    });
                }
            });
        }
    }

    //get grid table details 

    function getGridInfo() {
        gridHeaderTable = grid.find(".k-grid-header table");
        gridContentTable = grid.find(".k-grid-content table");
        tableHeaderGroupCol = $(gridHeaderTable).find("colgroup col");
        tableHeaderGroup = $(gridHeaderTable).find("colgroup");
        tableHeaderCol = $(gridHeaderTable).find("thead tr th");
        tableContentGroupCol = $(gridContentTable).find("colgroup col");
        tableContentCol = $(gridContentTable).find("tbody tr td");
        totalColCount = $(tableHeaderGroupCol).length;
        totalGridWidth = $(gridContentTable).width();
        totalVisibleWidth  =      grid.find("div.k-grid-header").width();
    }

    //create frozen scrolling container
    function createScrollContainer() {
        //disable grid default scrolling
        resetPos = false;
        sp = 0;
        //if it already exist, scroll left and remove it
        if ($(frozenContainer).length) {
            var sl = $(frozenContainer).scrollLeft();
            var sw = $(frozenInnerContainer).width();
            sp = sl / sw;
            $(frozenContainer).scrollLeft(0);
            resetPos = true;
            //unfreeze everything
           performFreezeColumns();
            $('#gridfrozenContainer').remove();
        }
        $(grid).find(".k-grid-content").css("overflow-x", "hidden");
        //create controls to handle freeze column scrolling

        frozenContainer = $("<div id='gridfrozenContainer' />");
        frozenInnerContainer = $("<div id='gridfrozenInnerContainer' />");
        //in order for this to work in IE, outside container must be bigger, hence the 18px and 17px
        $(frozenContainer).css("height", "18px").css("overflow", "scroll").css("width", "100%");
        $(frozenInnerContainer).css("height", "17px").css("width", $(gridContentTable).width());

        $(frozenContainer).append($(frozenInnerContainer));
        $(frozenContainer).insertAfter('.k-grid-content');
    }

    ////get how many columns are not visible behind the content container
    //function getHiddenColCount() {
    //    hiddenColCount = 0;
    //    totalColWidth = 0;
    //    var visibleWidth = grid.find("div.k-grid-header").width();
    //    var visibleColCount = 0;
    //    if (!fullWidthArr.length) {
    //        for (var i = 0; i < totalColCount; i++) {
    //            fullWidthArr.push(getWidth($(tableHeaderGroupCol).eq(i)));
    //        }
    //    }

    //    $.each(tableHeaderGroupCol, function (idx, value) {
    //        totalColWidth += fullWidthArr[idx];

    //        if (visibleWidth >= totalColWidth) {
    //            visibleColCount = idx + 1;
    //        }
    //    });

    //    hiddenColCount = totalColCount - visibleColCount;
    //}

    //create column width array to detect which column to hide when scrolling

    function createColumnWidthList() {
        if (!columnWidthArr.length || resetColumnWidths) {
            resetColumnWidths = false;
            frozenColWidth = 0;
            columnWidthArr = new Array();
            fullWidthArr = new Array();
            for (var i = 0; i < totalColCount; i++) {
                //get total width for frozen column
                if (i < frozenColCount) {
                    fullWidthArr.push(getWidth($(tableHeaderGroupCol).eq(i)));
                    frozenColWidth += getWidth($(tableHeaderGroupCol).eq(i));
                }
                    //or add to the column array for adjacent columns
                else {
                    fullWidthArr.push(getWidth($(tableHeaderGroupCol).eq(i)));
                    columnWidthArr.push(getWidth($(tableHeaderGroupCol).eq(i)));
                }
            }
            var visibleWidth = grid.find("div.k-grid-header").width();
            var visibleColCount = 0;
            $.each(tableHeaderGroupCol, function (idx, value) {
                totalColWidth += fullWidthArr[idx];

                if (visibleWidth >= totalColWidth) {
                    visibleColCount = idx + 1;
                }
            });

            hiddenColCount = totalColCount - visibleColCount;


        } 
    }

    function unhideAllColumns() {
        for (var i = frozenColCount; i < totalColCount - 1; i++) {
            //reset to show all columns
            var index = i - frozenColCount;
            var widthAttribute = columnWidthArr[index];
            //and the colgroups so widths work
            $(tableHeaderGroupCol).eq(i).width(widthAttribute);
            $(tableContentGroupCol).eq(i).width(widthAttribute);
        }
    }

    //get width of a particular element
    function getWidth(element) {
        //chrome and safari detect width differently, getting width from style
        if ($.browser.chrome || $.browser.safari || ($.browser.msie && browserVersion > 7)) {
            try {
                var width = eval(parseInt($(element).css("width").replace("px", "")));
                return width;
            } catch (e) {
                return defaultWidth;
            }
        }
        else {
            return $(element).width();
        }
    }

    //initialise browser detection
    function initBrowsers() {
        $.browser.chrome = /chrome/.test(navigator.userAgent.toLowerCase());
        $.browser.safari = /safari/.test(navigator.userAgent.toLowerCase());
        $.browser.opera = /opera/.test(navigator.userAgent.toLowerCase());
    }

    //perform freeze columns implementation
    function performFreezeColumns() {

        var colOffset = 1;
        //dont need to do this if the visible width is greater than the total column width, so do that check first.
        var total = frozenColWidth;
        $.each(columnWidthArr, function () {
            total += this;
        });
        var visibleWidth = grid.find("div.k-grid-header").width();
    
            totalColWidth = frozenColWidth;

            for (var i = frozenColCount; i < totalColCount - 1; i++) {
                var index = i - frozenColCount;
                var adjacentColWidth = columnWidthArr[index];
                var scrollPos = $(frozenContainer).scrollLeft() + frozenColWidth + adjacentColWidth - colOffset;
                totalColWidth += columnWidthArr[index];
                var showCol = (scrollPos < totalColWidth);
                var widthAttribute = "0";
                if (showCol)
                    widthAttribute = columnWidthArr[index];

                //and the colgroups so widths work
                $(tableHeaderGroupCol).eq(i).width(widthAttribute);
                $(tableContentGroupCol).eq(i).width(widthAttribute);
            }

            currentScrollPos = $(frozenContainer).scrollLeft();
        
    }

})(jQuery);

