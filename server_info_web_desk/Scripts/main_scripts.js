﻿var time_for_page_up;


function page_up_function() {
    var top = Math.max(document.body.scrollTop, document.documentElement.scrollTop);
    if (top > 0) {
        window.scrollBy(0, -100);
        time_for_page_up = setTimeout('page_up_function()', 20);
    } else clearTimeout(time_for_page_up);
    return false;
}