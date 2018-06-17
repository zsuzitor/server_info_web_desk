$(function () {

    
    // Ссылка на автоматически-сгенерированный прокси хаба
    var chat = $.connection.chatHub;


    // Объявление функции, которая хаб вызывает при получении сообщений
    chat.client.NeedDownloadChangesMessages = function (id_dialog) {
        //Dialog_div_message_id
        if (isNaN(id_dialog) || id_dialog == undefined || id_dialog == null) {
            alert("NeedDownloadChangesMessages error");
            return;
        }
        var dt = {
            'dialog': id_dialog
        };
        $.ajax({
            url: "/SocialNetwork/LoadNewMessages",
            data: dt,
            success: OnComplete_Load_new_messages,
            error: function () {
                alert("ошибка загрузки");
                document.getElementById('Main_preloader_id').style.display = 'none;';
               
            },
            beforeSend: function () { document.getElementById('Main_preloader_id').style.display = 'block';  },
            complete: function () {
                document.getElementById('Main_preloader_id').style.display = 'none';
                

            },
            type: 'POST', dataType: 'html'//html
        });




    };

    check_load_new_message_dialog = function (id_dialog) {
        chat.server.Send(id_dialog);
    };

  

    // Открываем соединение
    $.connection.hub.start().done(function () {

        chat.server.JoinToHub();


        //устанавливаем событие отправки на кнопку
        //$('#sendmessage').click(function () {
        //    // Вызываем у хаба метод Send
        //    chat.server.send($('#username').val(), $('#message').val());
        //    $('#message').val('');
        //});





    });
});
// Кодирование тегов
function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}
