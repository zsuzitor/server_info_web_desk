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
        type: 'POST', dataType: 'json'
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








//-----------------------------------GROUP------------------------------------------------
function OnComplete_follow_group(data) {

}