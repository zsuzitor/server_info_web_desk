$(function () {

    
    // Ссылка на автоматически-сгенерированный прокси хаба
    var chat = $.connection.chatHub;


    // Объявление функции, которая хаб вызывает при получении сообщений
    chat.client.NeedDownloadChangesMessages = function (id_dialog) {
        // Добавление сообщений на веб-страницу 
        //$('#chatroom').append('<p><b>' + htmlEncode(name)
        //+ '</b>: ' + htmlEncode(message) + '</p>');
    };

    

  

    // Открываем соединение
    $.connection.hub.start().done(function () {




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
