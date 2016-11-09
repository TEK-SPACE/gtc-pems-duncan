
//NOTE: do not override this file with an updated version, since we have modified it for out uses.

jQuery(document).ready(function ($) {
  // ('Starting');
  // Check if the sidebar is collapsed
  if ($.cookie('collapsed') === 'true') {
    $('body').addClass('collapsed');
    // console.log('Found collapsed. Ending.');
  }

  $('#left-navigation').children().each(function() {
      //console.log($(this));
    //if($.cookie($(this).attr('id')) === 'open' ) {
    //  $(this).addClass('open');
    //}
  });

  $('.collapse').click(function() {
    // console.log('Click fired');
    if ($('body').hasClass('collapsed')) {
      uncollapseMenu();
    }else {
      collapseMenu();
    }
  });

  function collapseMenu() {
    // console.log('Menu collapsed');
    $('body').addClass('collapsed');
    $.cookie('collapsed', 'true', { expires: 365, path: '/' });
  }

  function uncollapseMenu() {
    // console.log('Menu uncollapsed');
    $('body').removeClass('collapsed');
    // $.cookie('collapsed', null);
    $.removeCookie('collapsed', { path: '/' });
  }

  // Drop down
  $('#left-navigation').click(function(event) {

      //TO prevent the menu from collapsing on click for the main nav items, uncomment this section
      //we dont want cause a collapse if it is a main menuitem
    if ($(event.target).hasClass('stopClickEvent')) {
        return;
    }

    if ($(event.target).hasClass('top-level') || $(event.target).hasClass('top-level-header') || $(event.target).hasClass('menuExpandor')) {
      var $parent = $(event.target).parent();
      while($parent.attr('id') === undefined || $parent.attr('id') === false) {
        $parent = $parent.parent();
      }

      if($parent.hasClass('open')) {
        $parent.removeClass('open');
       // $.cookie($parent.attr('id'), null);
      }else {
        $parent.addClass('open');
       // $.cookie($parent.attr('id'), 'open', { expires: 365, path: '/' });
      }
    }
  });
});