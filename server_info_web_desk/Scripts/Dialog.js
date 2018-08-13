

var Dialog_OBJECT = { get_dialog_id: 
    function(){
        if(Dialog_OBJECT.dialog_id==null)
            dialog_id=document.getElementById('dialog_id_input_id').value;
        return Dialog_OBJECT.dialog_id;
    }
    , dialog_id:document.getElementById('dialog_id_input_id').value,
    start: 11, count: 10, can_load: true, selected_messages: [],
    selected_friends_for_add: []
};

function load_more_messages() {
    //if (Dialog_OBJECT.get_dialog_id() == null) {
    //    Dialog_OBJECT.dialog = document.getElementById('dialog_id_input_id').value;

    //}

    if (!Dialog_OBJECT.can_load) {
        alert("попробуйте позже");
        return;
    }

    var dt = {
        'start': Dialog_OBJECT.start,
        'count': Dialog_OBJECT.count,
        'id': Dialog_OBJECT.get_dialog_id()
    };
    $.ajax({
        url: "/SocialNetwork/ListMessagesUser",
        data: dt,
        success: OnSuccessLoadDialogsMessages,
        error: function () {
            alert("ошибка загрузки");
            PreloaderAction(false);
            Dialog_OBJECT.can_load = true;
        },
        beforeSend: function () { PreloaderAction(true); Dialog_OBJECT.can_load = false; },
        complete: function () {
            PreloaderAction(false);
            Dialog_OBJECT.start += Dialog_OBJECT.count;
            Dialog_OBJECT.can_load = true;

        },
        type: 'POST', dataType: 'html'//html
    });


}


function OnSuccessLoadDialogsMessages(data) {
    var div = document.getElementById("Dialog_div_message_id");
    div.innerHTML = data + div.innerHTML;
}



function click_on_one_message(mes) {

    var id = mes.id.split('_')[3];
    //
    //var mes = document.getElementById();
    var index = 0;
    var start_length=Dialog_OBJECT.selected_messages.length;
    for (; index < start_length; ++index) {
        if (Dialog_OBJECT.selected_messages[index] == id) {
            mes.style.backgroundColor = '#FFFFFF';
            Dialog_OBJECT.selected_messages.splice(index,1)
            //break;
            return;
        }
    }

    if (index >= start_length) {
        mes.style.backgroundColor = '#d1d9e0';
        Dialog_OBJECT.selected_messages.push(id);
    }


}


function delete_select_messages(){

    //Dialog_OBJECT.selected_messages

    var dt = {
        'mass_id': Dialog_OBJECT.selected_messages
    };
    $.ajax({
        url: "/SocialNetwork/DeleteMessages",
        data: dt,
        success: OnSuccessDeleteMessages,
        error: function () {
            alert("ошибка загрузки");
            PreloaderAction(false);
           
        },
        beforeSend: function () { PreloaderAction(true); },
        complete: function () {
            PreloaderAction(false);
            

        },
        type: 'POST', dataType: 'json'//html
    });


}


function OnSuccessDeleteMessages(data) {

    for (var i = 0; i < data.length; ++i) {
        document.getElementById("OneMessageAllblock_div_id_"+data[i]).innerHTML='удалено';
    }
}


function leave_dialog() {
    var a = document.createElement('a');
    //href="/SocialNetwork/PersonalRecord/dc9defb2-f9a0-44cc-b7ee-2d08b1f2ada6"
    a.setAttribute('href', "/SocialNetwork/LeaveDialog/" + Dialog_OBJECT.get_dialog_id());
    a.setAttribute('id', "leave_dialog_link_id");
    var div = document.getElementById("div_for_rub");
    div.appendChild(a);
    document.getElementById("leave_dialog_link_id").click();

    //document.getElementById('body')[0].appendChild(a);
}

//function leave_dialog() {

//    var dt = {
//        'id_dialog':  Dialog_OBJECT.dialog
//    };
//    $.ajax({
//        url: "/SocialNetwork/LeaveDialog",
//        data: dt,
//        success: fun,
//        error: function () {
//            alert("ошибка загрузки");
//            PreloaderAction(false);

//        },
//        beforeSend: function () { PreloaderAction(true); },
//        complete: function () {
//            PreloaderAction(false);


//        },
//        type: 'POST', dataType: 'json'//html
//    });

   
//}






//добавление новых пользователей в диалог


function select_user_for_add_dialog(id) {

    //Dialog_OBJECT.selected_friends_for_add
    var mes=;
    var index = 0;
    var start_length = Dialog_OBJECT.selected_friends_for_add.length;
    for (; index < start_length; ++index) {
        if (Dialog_OBJECT.selected_friends_for_add[index] == id) {
            mes.style.backgroundColor = '#FFFFFF';
            Dialog_OBJECT.selected_friends_for_add.splice(index, 1)
            //break;
            return;
        }
    }

    if (index >= start_length) {
        mes.style.backgroundColor = '#d1d9e0';
        Dialog_OBJECT.selected_friends_for_add.push(id);
    }


}


