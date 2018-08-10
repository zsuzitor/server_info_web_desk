function OnComplete_follow_group(data) {

}



jQuery(document).ready(function () {
    $("#from_for_load_record_on_wall").submit(function () { return false; });

    $("#from_for_load_record_on_wall_submit").on("click", function () {


        //$(".submit").replaceWith("<div class='form_dscr'><p class='sending'>Отправка...</p></div>");
        ////получаем форму
        var form = document.getElementById("from_for_load_record_on_wall");

        var formData = new FormData(form);

        var xhr = new XMLHttpRequest();
        xhr.open("POST", "/SocialNetwork/AddMemeGroup");

        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4) {
                if (xhr.status == 200) {
                    //при возврате actionresult и тд, вернет -"<html>"
                    data = xhr.responseText;
                    var dv = document.getElementById("Wall_meme_block_update_id");
                    dv.innerHTML = data + dv.innerHTML;
                }
            }
        };

        xhr.send(formData);



    });
});