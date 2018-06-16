var time_for_page_up;


function page_up_function() {
    var top = Math.max(document.body.scrollTop, document.documentElement.scrollTop);
    if (top > 0) {
        window.scrollBy(0, -100);
        time_for_page_up = setTimeout('page_up_function()', 20);
    } else clearTimeout(time_for_page_up);
    return false;
}



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
            document.getElementById('Main_preloader_id').style.display = 'none;';
        },
        beforeSend: function () { document.getElementById('Main_preloader_id').style.display = 'block'; },
        complete: function () { document.getElementById('Main_preloader_id').style.display = 'none'; },
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
            document.getElementById('Main_preloader_id').style.display = 'none;';
            
        },
        beforeSend: function () { document.getElementById('Main_preloader_id').style.display = 'block'; },
        complete: function () {
            document.getElementById('Main_preloader_id').style.display = 'none';

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
            document.getElementById('Main_preloader_id').style.display = 'none;';
            Album_OBJECT.can_load = true;
        },
        beforeSend: function () { document.getElementById('Main_preloader_id').style.display = 'block'; Album_OBJECT.can_load = false; },
        complete: function () {
            document.getElementById('Main_preloader_id').style.display = 'none';
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