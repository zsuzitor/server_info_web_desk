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



