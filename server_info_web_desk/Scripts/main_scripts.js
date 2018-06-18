﻿
//------------------------------------------------------------------------------------ALL---------------------


var time_for_page_up;


function page_up_function() {
    var top = Math.max(document.body.scrollTop, document.documentElement.scrollTop);
    if (top > 0) {
        window.scrollBy(0, -100);
        time_for_page_up = setTimeout('page_up_function()', 20);
    } else clearTimeout(time_for_page_up);
    return false;
}

function PreloaderAction(show) {
    if (show == true)
        document.getElementById('Main_preloader_id').style.display = 'block';
        else
        document.getElementById('Main_preloader_id').style.display = 'none;';
}


//-----------------------------------------------------SOCIAL-----------------------------------------------------------------------



function isVisible(tag) {
    var t = $(tag);
    var w = $(window);
    var top_window = w.scrollTop();
    var bot_window = top_window + document.documentElement.clientHeight;
    var top_tag = t.offset().top;
    var bot_tag = top_tag + t.height();

    return ((bot_tag >= top_window && bot_tag <= bot_window) || (top_tag >= top_window && top_tag <= bot_window) || (bot_tag >= bot_window && top_tag <= top_window));
}




$(function () {
    $(window).scroll(function () {
        ////var gg = $(window).scrollTop();
        //var left_menu = document.getElementById('LeftMenuPersonalMain_id');
        //if (left_menu.getBoundingClientRect().bottom < 0) {
        //    var height_left_menu = left_menu.offsetHeight;//left_menu.getBoundingClientRect().bottom - left_menu.getBoundingClientRect().top;
        //    left_menu.style.top = $(window).scrollTop() - 20 - height_left_menu + 'px';
        //}
        //else {
        //    var hhh = left_menu.getBoundingClientRect().top;
        //    if (left_menu.getBoundingClientRect().top > 80)
        //        left_menu.style.top = pageYOffset + 'px';
        //}

        
        //left menu

        var left_menu = document.getElementById("LeftMenuPersonalMain_id");
        //b.style.top = parseInt(100) + 20 + 'px';
        var left_menu_c = left_menu.getBoundingClientRect();
        //alert(document.body.scrollTop);
        if (left_menu_c.top > 80) {
            var str = pageYOffset;
            left_menu.style.top = str + 'px';
           
        }
        else
            if (left_menu_c.bottom < 0) {
            //alert("выше");
            //var g = document.body.scrollTop;

            //alert(c.bottom);
            var str = pageYOffset - left_menu.offsetHeight - 20;//- b.offsetHeight  offsetWidth
            //alert(str);
            left_menu.style.top = parseInt(str) + 'px';
            //b.top = str;
            //alert(b.style.top);
            
        }
        
       
        //alert(document.documentElement.clientHeight)
        var block_s_1 = document.getElementById("div_content_main_size_1_id");
        var block_s_2 = document.getElementById("div_content_main_size_2_id");
        var need_move_1 = null;
        var need_move_2 = null;

        var block_s_1_c = block_s_1.getBoundingClientRect();
        
        if (block_s_1_c.height < document.documentElement.clientHeight) {
            var str = pageYOffset;
            //block_s_1.style.marginTop = str + 'px';
            need_move_1 = str;
        }
        else {


            if (block_s_1_c.top > 80) {
                var str = pageYOffset;
                //block_s_1.style.marginTop = str + 'px';
                need_move_1 = str;

            }
            else
                if (block_s_1_c.bottom < (document.documentElement.clientHeight-100)) {//pageYOffset +

                    var str = parseInt(pageYOffset + document.documentElement.clientHeight - block_s_1.offsetHeight-100);//- b.offsetHeight  offsetWidth
                    if (str < 80)
                        // block_s_1.style.marginTop = 80 + 'px';
                        need_move_1 = 80;
                    else
                        //block_s_1.style.marginTop = str + 'px';
                        need_move_1 = str;


                }
        }

        var block_s_2_c = block_s_2.getBoundingClientRect();
        if (block_s_2_c.height < document.documentElement.clientHeight) {
            var str = pageYOffset;
            //block_s_2.style.marginTop = str + 'px';
            need_move_2 = str;
        }
        else {

       
        if (block_s_2_c.top > 80) {
            var str = pageYOffset;
            // block_s_2.style.marginTop = str + 'px';
            need_move_2 = str;
           
        }
        else
            if (block_s_2_c.bottom < (document.documentElement.clientHeight - 100)) {//pageYOffset +

                var str = parseInt(pageYOffset + document.documentElement.clientHeight - block_s_2.offsetHeight - 100);//- b.offsetHeight  offsetWidth
                if (str < 80)
                    //block_s_2.style.marginTop =  80 + 'px';
                    need_move_2 = 80;
                else
                    //block_s_2.style.marginTop = str + 'px';
                    need_move_2 = str;

                
            }
        }
        var hr_bot = document.getElementById('hr_bottom_footer_id');
        if (!isVisible(hr_bot)) {
            if (need_move_1 != null)
                block_s_1.style.marginTop = need_move_1 + 'px';
            if (need_move_2 != null)
                block_s_2.style.marginTop = need_move_2 + 'px';
        }
        //if (need_move_1 != null || need_move_2 != null) {
        //    var hr_bot=document.getElementById('hr_bottom_footer_id');
        //    if (!isVisible(hr_bot)) {

           
        //    if (need_move_1 != null)
        //        block_s_1.style.marginTop = need_move_1+'px';
        //    else
        //        block_s_2.style.marginTop = need_move_2 + 'px';
        //}
        //}
    });
});


















function LikeRecordClick(id) {
    

    var dt = {
        'id': id
    };
    $.ajax({
        url: "/SocialNetwork/LikeRecord",
        data: dt,
        success: OnComplete_LikeRecordClick,
        error: function () {
            alert("ошибка загрузки");
            PreloaderAction(false);
        },
        beforeSend: function () { PreloaderAction(true); },
        complete: function () { PreloaderAction(false); },
        type: 'POST', dataType: 'json'//html
    });

}
function OnComplete_LikeRecordClick(data){
    ////Record_like_block_img_id_234
    if (data == null) {
        alert("Ошибка OnComplete_LikeRecordClick вернул null");
        return;
    }
    var img=document.getElementById("Record_like_block_img_id_" + data.id);
    if(data.red_heart){
        img.innerHTML = '<div class="Record_like_block_img_active"></div>';
    }
    else{

        img.innerHTML = '<div class="Record_like_block_img_not_active"></div>';

    }
    var num = document.getElementById("Record_like_block_count_id_" + data.id);
    num.innerHTML=data.count;
}


//-------------------------------------------------NOT NEED NEW FILE

function ShowImageRecordAJAX(a) {
    //var id = a.id.split('_')[1];
    var dt = {
        'id': a
    };
    $.ajax({
        url: "/SocialNetwork/LoadShowImageRecord",
        data: dt,
        success: OnComplete_LoadShowImageRecord,
        error: function () {
            alert("ошибка загрузки");
            PreloaderAction(false);
            
        },
        beforeSend: function () { PreloaderAction(true); },
        complete: function () {
            PreloaderAction(false);

        },
        type: 'POST', dataType: 'html'//html
    });


}

function OnComplete_LoadShowImageRecord(data) {

    var div = document.getElementById("div_for_replace_ajax_id_2");
    var div2 = document.getElementById("div_for_replace_ajax_id");
    div2.style.width = '100%';//document.documentElement.clientWidth + 'px';
    div2.style.height = '100%';//document.documentElement.clientHeight + 'px';
    div.innerHTML = data;
    div.style.left = '0px';
    div.style.top = '100px';
    div2.style.left = '0px';
    div2.style.top = '0px';
    div2.style.display = 'block';
}


function ShowImageRecordClose() {
    var div = document.getElementById("div_for_replace_ajax_id_2");
    var div2 = document.getElementById("div_for_replace_ajax_id");

    div.innerHTML = '';

    div2.style.width =  '0px';
    div2.style.height = '0px';
   
    div2.style.left = '-100px';
    div2.style.top = '-100px';
    div2.style.display = 'none';
    
}

//-------------------------------------PERSON------------------------------------------------
function PersonalRecord_show_more_imfo() {
    var div = document.getElementById("PersonalRecord_div_more_imfo");
    var button = document.getElementById("PersonalRecord_button_more_imfo");
    
    if (div.style.display == 'none') {
        div.style.display = 'block';
        button.innerHTML = "Скрыть полную информацию";
    }
    else {
        div.style.display = 'none';
        button.innerHTML = "  Показать полную информацию";
      
    }


}



//-----------------------------------GROUP------------------------------------------------
function OnComplete_follow_group(data) {

}





//---------------------------------------------------ALBUMS-------------------------------------------------------
var Album_OBJECT = { album_id: null, start: 11, count: 10, can_load: true };

function load_more_img_album() {
    if (Album_OBJECT.album_id == null ) {
        alert("Не выбран альбом");
        return;
    }
    if (!Album_OBJECT.can_load) {
        alert("попробуйте позже");
        return;
    }
   
    var dt = {
        'id': Album_OBJECT.album_id,
        'start': Album_OBJECT.start,
        'count': Album_OBJECT.count
    };
    $.ajax({
        url: "/SocialNetwork/LoadImagesAlbum",
        data: dt,
        success: OnComplete_Load_images_album,
        error: function () {
            alert("ошибка загрузки");
            PreloaderAction(false);
            Album_OBJECT.can_load = true;
        },
        beforeSend: function () { PreloaderAction(true); Album_OBJECT.can_load = false; },
        complete: function () {
            PreloaderAction(false);
            Album_OBJECT.start += Album_OBJECT.count;
            Album_OBJECT.can_load = true;
           
        },
        type: 'POST', dataType: 'html'//html
    });


}

function OnComplete_Load_images_album(data) {
    //alert(data);
    //Albums_block_for_img
    var div = document.getElementById("Albums_block_for_img");
    div.innerHTML = data;
}

function Album_one_block_click_al(a) {
    var id = a.id.split('_')[4];//    Album_one_block_id_
    Album_OBJECT.album_id = id;
    Album_OBJECT.start = 0;
    Album_OBJECT.can_load = true;
    document.getElementById("album_id").value = id;
    document.getElementById("Albums_block_for_img").innerHTML="";

    load_more_img_album();

}




//---------------------------------------------------------------DIALOG--------------------------------------------------
var check_load_new_message_dialog = null;//сюда сохраняется функция отправки к хабу



function SendMessage() {
    

    var dt = {
        'dialog': document.getElementById('dialog').value,
        'text': document.getElementById('text').value
    };

    $.ajax({
        url: "/SocialNetwork/SendNewMessageForm",
        data: dt,
        success: OnComplete_SendMessage,
        error: function () {
            alert("ошибка загрузки");
            PreloaderAction(false);
           
        },
        beforeSend: function () { PreloaderAction(true); },
        complete: function () {
            PreloaderAction(false);
            //OnComplete_SendMessage();

        },
        type: 'POST', dataType: 'json'//html
    });


}






function OnComplete_SendMessage(data){

    if (data == null){
        alert("OnComplete_SendMessage error");
        return;
    }
        
    var id = +data.dialog;
    if (isNaN(id) || id == undefined || id == null) {
        alert("OnComplete_SendMessage error");
        return;
    }
    check_load_new_message_dialog(id);

}


function OnComplete_Load_new_messages(data) {
    var div = document.getElementById("Dialog_div_message_id");
    if (div != undefined || div != null) {
        div.innerHTML += data;
    }
    else {
        //TODO тут надо порядок менять , сначала Dialog_div_message_id->страница диалогов->num_CountNewMessage_id
        div = document.getElementById("num_CountNewMessage_id");
        if (div != undefined || div != null) {
            div.innerHTML += data;
        }
        else {
            //TODO обновлять диалог на странице диалогов
        }
    }
    
}