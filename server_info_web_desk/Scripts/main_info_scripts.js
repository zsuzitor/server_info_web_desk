// box-shadow: 0 0 10px rgba(0,0,0,0.5);




var client_width = 0;
var client_heigth = 0;
var x_centre = 0;//относительно всего экрана(от левого края БРАУЗЕРА)
var change_x_centre_object = {};
change_x_centre_object.click_change_x_centre = null;

var mass_section = [];
var mass_article = [];
var last_click_name = null;
var mass_section_open = [];



if (document.addEventListener) {
    document.addEventListener("DOMContentLoaded", page_first_start);
} else if (document.attachEvent) {
    document.attachEvent('DOMContentLoaded', page_first_start);
}


document.onmouseup = function (e) {
    click_on_centre_settings(false);
}
document.onmousedown = function (e) {
    if (event.which == 3) {

        click_on_centre_settings(false);
    }

}

document.onmousemove = function (e) {
    if (change_x_centre_object.click_change_x_centre == true) {

        var sett_block = document.getElementById("div_settings_block_id");
        var left_line_div = document.getElementById("div_left_column_id");
        var cord_mouse = event.clientX;
        var right_div = document.getElementById("main_block_right_id");
        if (cord_mouse < left_line_div.getBoundingClientRect().right) {
            cord_mouse = left_line_div.getBoundingClientRect().right;
        }
        else
            if ((cord_mouse + sett_block.offsetWidth / 2) > client_width)
                cord_mouse = client_width - sett_block.offsetWidth / 2;

        var centre = cord_mouse;//-src_div.offsetWidth
        x_centre = centre;

        var left_div = document.getElementById("main_block_left_id");
        change_centre(centre);
        if (sett_block.getBoundingClientRect().left < 310)
            move_search_div(450);
        else if (document.getElementById("div_search_id").getBoundingClientRect().right > 50)
            move_search_div(0);

        left_div.style.display = 'block';

        //
        // event.which == 1 – левая кнопка
        // event.which == 2 – средняя кнопка
        // event.which == 3 – правая кнопка
    }
}



function page_first_start() {
    client_width = document.documentElement.clientWidth;
    client_heigth = document.documentElement.clientHeight;
    var left_div = document.getElementById("main_block_left_id");
    var left_line_div = document.getElementById("div_left_column_id");
    var div_for_block = document.getElementById("div_for_block_id");
    div_for_block.style.width = ((client_width - left_line_div.offsetWidth) + 'px');
    x_centre = left_line_div.offsetWidth + left_div.offsetWidth;
    var setting_div = document.getElementById("div_settings_block_id");
    setting_div.style.left = x_centre - (setting_div.offsetWidth / 2) + 'px';
    left_line_div.style.height = client_heigth + 'px'
    var right_div = document.getElementById("main_block_right_id");
    right_div.style.left = left_div.offsetWidth + 5 + 'px';
    var left_top_button = document.getElementById("scroll_top_button_id");
    left_top_button.style.marginTop = document.getElementById("div_for_top_menu_id").getBoundingClientRect().bottom + 'px';

    if (setting_div.getBoundingClientRect().left < 310)
        move_search_div(450);
    else if (document.getElementById("div_search_id").getBoundingClientRect().right > 50)
        move_search_div(0);
    
    
    mass_section.push((function () {
        var tmp = {};
        tmp.Id = document.getElementById("id_first_load_section").value;
        tmp.Head = document.getElementById("head_first_load_section").value;
        return tmp;
    })());
    var left_div = document.getElementById("main_block_left_id");
    var str_res_for_left_ul = load_one_section(mass_section[0].Id);
    left_div.innerHTML = str_add_name_section(mass_section[0].Id) + str_res_for_left_ul;

}



function str_add_name_section(id) {
    var res = "";
    //if (all != null && all == true)
        res += "<div  class='div_one_section_name' onclick='click_name_section(this)' id='div_one_section_name_" + id + "'>";
        res += "<div id='before_for_sect_name_" + id + "' class='before_for_sect_name div_inline_block'></div><div class='div_inline_block' id='div_one_section_name_text_" + id + "'>" + find_in_mass(id, 1).Head + "</div>";//convert_string(find_in_mass(id, 1).Head)
    //if (all != null && all == true)
        res += "</div>";
    return res;
}

//возвращает разметку для внутренней части div секции
function load_one_section(id) {
    var res = "<div style='display:none;' class='div_one_section_inside_cl' id='div_one_section_inside_" + id + "'>";
    res += "</div>";
    return res;
}


//ДАННЫЕ БЕРЕТ УЖЕ ЗАГРУЖЕННЫЕ
//загружает разметку в созданную секцию
//TODO проверять загружена ли она, если загружена то отобразить, иначе загрузить
function load_one_section_data(id) {
    //var block_res = document.getElementById("div_one_section_inside_"+id);
    var res = "<div class='div_one_section_inside_inside' id='div_one_section_inside_inside_" + id + "'>\
	<div class='div_inside_sections' id='div_inside_sections_"+ id + "'>";

    var mass_with_id = [];
    for (var i = 0; i < mass_section.length; ++i) {
        if (mass_section[i].Section_parrentId == id)
            mass_with_id.push(mass_section[i].Id);
    }

    for (var i = 0; i < mass_with_id.length; ++i) {
        res += str_add_name_section(mass_with_id[i]);
        res += load_one_section(mass_with_id[i]);
    }
    res += "</div><div class='div_inside_articles' id='div_inside_articles_" + id + "'>";

    for (var i = 0; i < mass_article.length; ++i) {
        if (mass_article[i].Section_parrentId == id) {//
            res += "<div class='div_one_article_name' id='div_one_article_name_" + mass_article[i].Id + "' onclick='load_article(" + mass_article[i].Id + ")'>" + mass_article[i].Head + "</div>";//convert_string(mass_article[i].Head)
        }
    }

    res += "</div></div>";
    return res;
}

function find_in_mass(id, type_mass) {//1--секция 2--артикл
    if (id == null || id == undefined)
        return null;
    if (type_mass == 1) {
        for (var i = 0; i < mass_section.length; ++i)
            if (mass_section[i].Id == id)
                return mass_section[i];
    }
    else if (type_mass == 2) {
        for (var i = 0; i < mass_article.length; ++i)
            if (mass_article[i].Id == id)
                return mass_article[i];
    }
    return null;
}

function convert_string(str) {
    var res = "";
    res = str.replace(/</g, '&lt;');
    res = res.replace(/>/g, '&gt;');
    //res = res.replace(/@/g, '#');

    return res;
}


function click_name_section(a) {
    //проверять если есть блоки с parrent id то загружать не надо
    select_view_line(a.id);
    var int_id = a.id.split("_")[4];
    var need_load = true;
    for (var i = 0; i < mass_section.length; ++i) {
        if (mass_section[i].Section_parrentId == int_id) {
            need_load = false;
            break;
        }
    }
    if (need_load)
    for (var i = 0; i < mass_article.length; ++i) {
        if (mass_article[i].Section_parrentId == int_id) {
            need_load = false;
            break;
        }
    }

    if (need_load) {
        var inp = document.getElementById("id_section_for_load_input");
        inp.value = int_id;
        document.getElementById("id_section_for_load_input_submit").click();

    }
    else {
        open_section(int_id);
    }
}

function open_section(id) {
    var before_d = document.getElementById("before_for_sect_name_" + id);
    var div = document.getElementById("div_one_section_inside_" + id);
    var f1_ = function () {
        before_d.style.borderLeft = '30px solid black';
        before_d.style.borderBottom = '15px solid transparent';
        before_d.style.borderTop = '15px solid transparent';
        div.style.display = 'none';
    };

    var f2_ = function () {
        var div_in_sec = document.getElementById("div_inside_sections_" + id);
        var div_in_art = document.getElementById("div_inside_articles_" + id);
        before_d.style.borderTop = '30px solid black';
        before_d.style.borderRight = '15px solid transparent';
        before_d.style.borderLeft = '15px solid transparent';
        if (div_in_sec.innerHTML != '' || div_in_art.innerHTML != '') {
            div.style.display = 'inline-block';
            
        }
    };

    if (div.style.display == '' || div.style.display == 'inline-block') {
        f1_();
    }
    else {
        f2_();
    }




}
function OnComplete_load_inside_section(data) {
    var dt = JSON.parse(data.responseText);
   // alert(dt);
    if (dt == false) {
        alert("OnComplete_load_inside_section return false");
        return;
    }
       
    for (var i = 0; i < dt.Sections.length; ++i) {
        var obg_tmp = { Id: dt.Sections[i].Id, Head: dt.Sections[i].Head, Section_parrentId: dt.Sections[i].Section_parrentId };
        mass_section.push(obg_tmp)
    }
    for (var i = 0; i < dt.Articles.length; ++i) {
        var obg_tmp = { Id: dt.Articles[i].Id, Head: dt.Articles[i].Head, Body: null, Section_parrentId: dt.Articles[i].Section_parrentId };
        mass_article.push(obg_tmp)
    }

    var inside = document.getElementById("div_one_section_inside_" + dt.Id);

    inside.innerHTML = load_one_section_data(dt.Id);
    
    //= load_one_section(mass_section[0].Id);
    //left_div.innerHTML = str_add_name_section(mass_section[0].Id) + str_res_for_left_ul;

    open_section(dt.Id);
}
function OnComplete_load_article_body(data) {
    var dt = JSON.parse(data.responseText);
    if (dt == false) {
        alert("OnComplete_load_inside_section return false");
        return;
    }
    var block = find_in_mass(dt.Id, 2);
    block.Body = dt.Body;
    show_article(dt);
}


function OnComplete_Add_section(data) {
    var dt = JSON.parse(data.responseText);
    if (dt == false) {
        alert("OnComplete_Add_section return false");
        return;
    }
    var obj = { Id: dt.Id, Head: dt.Head, Section_parrentId: dt.Section_parrentId };
    mass_section.push(obj);

    document.getElementById("main_block_right_id").innerHTML = "";

    var inside_sect = document.getElementById("div_inside_sections_" + obj.Section_parrentId);
    var str= str_add_name_section(obj.Id) + load_one_section(obj.Id);
    inside_sect.innerHTML += str;
        
}
function OnComplete_Add_article(data) {
    var dt = JSON.parse(data.responseText);
    if (dt == false) {
        alert("OnComplete_Add_article return false");
        return;
    }
    var obj = { Id: dt.Id, Head: dt.Head, Body: null, Section_parrentId: dt.Section_parrentId };
    mass_article.push(obj)
    document.getElementById("main_block_right_id").innerHTML = "";
    var tmp = "";
    tmp += "<div class='div_one_article_name' id='div_one_article_name_" + obj.Id + "' onclick='load_article(" + obj.Id + ")'>" + obj.Head + "</div>";
    var inside_sect = document.getElementById("div_inside_articles_" + obj.Section_parrentId);
    inside_sect.innerHTML += tmp;
}

function OnComplete_edit_section(data) {
    var dt = JSON.parse(data.responseText);
    if (dt == false) {
        alert("OnComplete_edit_section return false");
        return;
    }
    
    for (var i = 0; i < mass_section.length; ++i) {
        if (mass_section[i].Id == dt.Id) {
            mass_section[i].Head = dt.Head;
            document.getElementById("div_one_section_name_text_" + dt.Id).innerHTML = dt.Head;
            
            break;
        }
    }
    document.getElementById("main_block_right_id").innerHTML="";
}
function OnComplete_edit_article(data) {
    var dt = JSON.parse(data.responseText);
    if (dt == false) {
        alert("OnComplete_edit_article return false");
        return;
    }
    for (var i = 0; i < mass_article.length; ++i) {
        if (mass_article[i].Id == dt.Id) {
            mass_article[i].Head = dt.Head;
            mass_article[i].Body = dt.Body;
            document.getElementById("div_one_article_name_" + dt.Id).innerHTML = dt.Head;
            load_article(dt.Id);
            break;
        }
    }

}

function edit_select() {

    if (last_click_name == null) {
        alert("выберите что-то для редактирования");
        return;
    }
    var id = last_click_name.split('_')[4];
    var right_div = document.getElementById("main_block_right_id");

    var res = '';
    if (last_click_name.indexOf("div_one_section_name") >= 0) {
        res += add_form_for_edit(id, 1);
    }
    else if (last_click_name.indexOf("div_one_article_name") >= 0) {
        res += add_form_for_edit(id, 2);
    }
    right_div.innerHTML = res;
}




//TODO //add_article add_section
function add_form_right(type) {//1-секция 2 -статья
    if (type != 1 && type != 2) {
        alert("Ошибка?")
        return;
    }
    if (last_click_name != null)
        if (last_click_name.indexOf('div_one_section_name_') < 0)
            alert('выберите секцию');
        else {
            var right_div = document.getElementById("main_block_right_id");
            var res = add_form_for_add(null, type);
            right_div.innerHTML = res;
        }
    else
        alert('выберите секцию');
}







function dell_select() {
    if (!confirm("Удалить выбранное со всеми вложениями?"))
        return;

    if (last_click_name == null) {
        alert("выберите что-то для удаления");
        return;
    }
    var id = last_click_name.split('_')[4];
    
    if (last_click_name.indexOf("div_one_section_name") >= 0) {
        delete_section_f(id);
    }
    else if (last_click_name.indexOf("div_one_article_name") >= 0) {
        delete_article_f(id);
    }
    last_click_name = null;
    document.getElementById("main_block_right_id").innerHTML = "";
}
function delete_section_f(id) {
    var inp = document.getElementById("id_section_for_delete_input");
    inp.value = id;
    document.getElementById("id_section_for_delete_input_submit").click();
}

function delete_article_f(id) {
    var inp = document.getElementById("id_article_for_delete_input");
    inp.value = id;
    document.getElementById("id_article_for_delete_input_submit").click();
}
function OnComplete_delete_section(data) {
    var dt = JSON.parse(data.responseText);
    if (dt == false) {
        alert("OnComplete_Add_article return false");
        return;
    }
    if (dt.parrent_id_main == null)
        for (var i2 = 0; i2 < mass_article.length; ++i2)
            if (mass_article[i2].Section_parrentId == dt.main_id)
                mass_article.splice(i2--, 1);
            
        

    for (var i = 0; i < dt.sec_list.length; ++i) {
        for (var i2 = 0; i2 < mass_article.length; ++i2) {
            if (mass_article[i2].Section_parrentId == dt.sec_list[i]) {
                mass_article.splice(i2--,1);
            }
        }
        go:
            for (var i2 = 0; i2 < mass_section.length; ++i2) 
                if (mass_section[i2].Id == dt.sec_list[i]) {
                    mass_section.splice(i2--, 1);
                    break go;
                }
                   
            
    }

    var inside = document.getElementById("div_one_section_inside_" + dt.main_id);

    inside.innerHTML = load_one_section_data(dt.main_id);
   
   
}
function OnComplete_delete_article(data) {
    var dt = JSON.parse(data.responseText);
    if (dt == false) {
        alert("OnComplete_Add_article return false");
        return;
    }
    for (var i2 = 0; i2 < mass_article.length; ++i2) {
        if (mass_article[i2].Id == dt) {
            mass_article.splice(i2--, 1);
            break;
        }
    }

    document.getElementById('div_one_article_name_' + dt).remove();
}

//--------------------------------------------------------------
function add_form_for_edit(id, type) {//1 секция 2 статья

    //var block = find_in_mass(id, type);
    var res = "<form action=\"/Info/";
    if (type == 1)
        res += 'Edit_section"';
    else
        res += 'Edit_article"';
    res += 'data-ajax="true" data-ajax-complete="';
    if (type == 1)
        res += 'OnComplete_edit_section"';
    else
        res += 'OnComplete_edit_article"';
    res += '" data-ajax-loading="#Main_preloader_id" data-ajax-loading-duration="200"  method="post">';

    
    res += add_form_for_add_or_edit(id, type);
    res += '</form>';
    return res;
}
function add_form_for_add(id, type) {//1 секция 2 статья

    //var block = find_in_mass(id, type);
    var res = "<form action=\"/Info/";
    if(type==1)
        res+='Add_section"';
    else
        res+='Add_article"';
    res+='data-ajax="true" data-ajax-complete="';
    if (type == 1)
        res += 'OnComplete_Add_section"';
    else
        res += 'OnComplete_Add_article"';
    res += '" data-ajax-loading="#Main_preloader_id" data-ajax-loading-duration="200"  method="post">';

    //СЮДА переписать add_form_for_add_or_edit
    res+=add_form_for_add_or_edit(id, type);
    res += '</form>';
    return res;
}
//function add_form_for_edit(id, type) {//1 секция 2 статья


//}
function add_form_for_add_or_edit(id, type) {//1 секция 2 статья
    var res = '';
    switch (type) {
        case 1:
            var block = find_in_mass(id, type);
            res += '<div><div><label>Заголовок</label></div>';
            
            res += '<textarea name="Head" class="text_area_add_edit" id="input_for_section_head">' + (block == null ? '' : block.Head) + '</textarea>';
            if (block == null){
                res += "<input name='parrent_sec_id' type='hidden' value=" + click_sect_id() + " />";
                res += "<input value='Добавить секцию'  type='submit'/>";
            }
                
            else {
                res += "<input name='Id' type='hidden' value=" + id + " />";
                res += "<input value='Сохранить'  type='submit'/>";
            }
                

            res += '</div>';
            break;
        case 2:
            var block = find_in_mass(id, type);

            res += '<div><div><label>Заголовок</label></div>';
            res += '<textarea name="Head" class="text_area_add_edit" id="input_for_article_head">' + (block == null ? '' : block.Head) + '</textarea>';
            res += '<div><label>Содержание</label></div>';
            res += '<textarea name="Body" class="text_area_add_edit" id="input_for_article_body">' + (block == null ? '' : block.Body) + '</textarea>';
            if (block == null){
                res += "<input value='Добавить статью'  type='submit'/>";
                res += "<input name='parrent_sec_id' type='hidden' value=" + click_sect_id() + " />";
                
            }
               
            else {
                res += "<input name='Id' type='hidden' value=" + id + " />";
                res += "<input value='Сохранить'  type='submit'/>";
            }
                
                
            res += '</div>';
            break;
        default:
            alert("error");
            break;
    }
    return res;
}

function select_view_line(new_click_id) {
    if (new_click_id != null && new_click_id != undefined) {
        var block_prev = document.getElementById(last_click_name);
        var block_current = document.getElementById(new_click_id);
        if (block_prev != null)
            block_prev.style.backgroundColor = '';
        if (block_current != null) {
            block_current.style.backgroundColor = '#F08080';
            last_click_name = new_click_id;
        }
    }
}


function load_article(id_ar) {
    var article = null;
    select_view_line('div_one_article_name_' + id_ar);
    article = find_in_mass(id_ar, 2);

    
        if (article == null||article.Body == null) {
            //TODO загрузить статью
            document.getElementById("id_article_for_load_input").value = id_ar;
            document.getElementById("id_article_for_load_input_submit").click();
        }
        else
            show_article(article);
    
   
}
function show_article(acticle) {
    var res = "";
    var right_div = document.getElementById("main_block_right_id");
    res = "<div>";
    res += "<h1><pre>";
    res += acticle.Head;//convert_string()
    res += "</pre><h1>";
    res += "<div><pre>";
    res += acticle.Body;//convert_string()
    res += "</pre></div>";
    res += "</div>";
    right_div.innerHTML = res;
}


function click_sect_id() {
    if (last_click_name == null)
        return null;
    if (last_click_name.indexOf('div_one_section_name_') == 0)
        return last_click_name.split('_')[4];
    else {
        return find_in_mass(last_click_name.split('_')[4], 2).Section_parrentId;
    }
}
//-------------------------------------------------------------

function close_reload_list() {

    alert("close_reload_list currently not implemented");
}

function save_server_db() {

    alert("save_server_db currently not implemented");
}
function home_button_return_left() {

    alert("home_button_return_left currently not implemented");
}

function show_top_menu() {
    var top_menu = document.getElementById("div_for_top_menu_id");
    top_menu.style.left = "0";
    var ch = document.getElementById("div_settings_bottom_b_ex");
    ch.innerHTML = "<div class='div_settings_top_b' onclick='hidden_top_menu()'></div>";
}


function hidden_top_menu() {
    var top_menu = document.getElementById("div_for_top_menu_id");
    top_menu.style.left = "-70px";
    var ch = document.getElementById("div_settings_bottom_b_ex");
    ch.innerHTML = "<div class='div_settings_bottom_b' onclick='show_top_menu()'></div>";
}


function show_left_menu() {
    var left_div = document.getElementById("main_block_left_id");
    var setting_div = document.getElementById("div_settings_block_id");
    if (left_div.style.display == 'none') {
        left_div.style.display = 'block';
        change_centre(x_centre);
        move_search_div(0);
    }
    else {
        change_centre(client_width - setting_div.offsetWidth / 2);
    }
}


function change_centre(coord) {//координата от левого края  //change_var   
    var left_div = document.getElementById("main_block_left_id");
    var setting_div = document.getElementById("div_settings_block_id");
    var right_div = document.getElementById("main_block_right_id");
    var left_line_div = document.getElementById("div_left_column_id");

    setting_div.style.left = coord - setting_div.offsetWidth / 2 + 'px';
    left_div.style.width = coord - left_line_div.offsetWidth - 3 + 'px';
    right_div.style.width = client_width - coord - 6 + 'px';
    right_div.style.left = coord - left_line_div.offsetWidth + 3 + 'px';
}

function hidden_left_menu() {
    var left_div = document.getElementById("main_block_left_id");
    var right_div = document.getElementById("main_block_right_id");
    var left_line_div = document.getElementById("div_left_column_id");
    var setting_div = document.getElementById("div_settings_block_id");
    if (left_div.style.display == 'block' || left_div.style.display == '') {

        if (setting_div.getBoundingClientRect().left < x_centre) {
            change_centre(left_line_div.offsetWidth);
            left_div.style.display = 'none';
            move_search_div(300);
        }
        else {
            change_centre(x_centre);
            if (setting_div < 310)
                move_search_div(450);
        }
    }
}








function show_search_block() {
    var div = document.getElementById("div_search_id");
    div.style.width = '300px';
    var div_img = document.getElementById("div_search_img_id");
    div_img.style.transform = 'rotateY(180deg)';
}


function hidden_search_block() {
    var div = document.getElementById("div_search_id");
    div.style.width = '55px';
    var div_img = document.getElementById("div_search_img_id");
    div_img.style.transform = 'rotateY(0deg)';
}


function click_on_centre_settings(flag) {
    change_x_centre_object.click_change_x_centre = flag;
    var sett_block = document.getElementById("div_settings_block_id");
    var left_div = document.getElementById("main_block_left_id");
    var right_div = document.getElementById("main_block_right_id");
    if (flag == true) {
        sett_block.style.transition = '0s';
        left_div.style.transition = '0s';
        right_div.style.transition = '0s';
    }
    else {
        sett_block.style.transition = '1s';
        left_div.style.transition = '1s';
        right_div.style.transition = '1s';
    }
}



function move_search_div(marginleft) {
    var search_div = document.getElementById("div_search_id");
    search_div.style.marginLeft = marginleft + 'px';
}