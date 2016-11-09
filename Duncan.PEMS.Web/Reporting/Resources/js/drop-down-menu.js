// Copyright (c) 2005-2013 Izenda, L.L.C. - ALL RIGHTS RESERVED    

function mainmenu(){
$(" .nav ul ul ").css({display: "none"}); // Opera Fix
$(" .nav ul li").hover(function(){
		$(this).find('ul:first').css({visibility: "visible",display: "none"}).slideDown(400);
		},function(){
		$(this).find('ul:first').css({visibility: "hidden"});
		});
}
$(document).ready(function(){					
	mainmenu();
});