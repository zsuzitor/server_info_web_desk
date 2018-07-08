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
    if (e.which == 3) {

        click_on_centre_settings(false);
    }

}

document.onmousemove = function (e) {
    if (change_x_centre_object.click_change_x_centre == true) {

        var sett_block = document.getElementById("div_settings_block_id");
        var left_line_div = document.getElementById("div_left_column_id");
        var cord_mouse = e.clientX;
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
        if (mass_section[i].SectionParrentId == id)
            mass_with_id.push(mass_section[i].Id);
    }

    for (var i = 0; i < mass_with_id.length; ++i) {
        res += str_add_name_section(mass_with_id[i]);
        res += load_one_section(mass_with_id[i]);
    }
    res += "</div><div class='div_inside_articles' id='div_inside_articles_" + id + "'>";

    for (var i = 0; i < mass_article.length; ++i) {
        if (mass_article[i].SectionParrentId == id) {//
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




function click_name_section(a) {//,open
    //проверять если есть блоки с parrent id то загружать не надо
    select_view_line(a.id);
    var int_id = a.id.split("_")[4];
    var need_load = true;
    for (var i = 0; i < mass_section.length; ++i) {
        if (mass_section[i].SectionParrentId == int_id) {
            need_load = false;
            break;
        }
    }
    if (need_load)
    for (var i = 0; i < mass_article.length; ++i) {
        if (mass_article[i].SectionParrentId == int_id) {
            need_load = false;
            break;
        }
    }

    if (need_load) {
        
        var dt = { 'id': int_id};
        $.ajax({
            url: "/Info/Load_inside_section",
            data: dt,
            success: OnComplete_load_inside_section,
            error: function () {
                alert("ошибка загрузки");
                PreloaderAction(false);
            },
            beforeSend: function () { PreloaderAction(true); },
            complete: function () { PreloaderAction(false); },
            type: 'POST', dataType: 'json'
        });


    }
    else {
        //if (!open)
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
    var open = false;
    for (var i = 0; i < mass_section_open.length; ++i) {
        if (mass_section_open[i] == id) {
            open = true;
            mass_section_open.splice(i, 1);
            break;
        }
    }
    if (open) {
        f1_();
    }
    else {
        mass_section_open.push(id);
        f2_();
    }




}
function OnComplete_load_inside_section(data) {
    
   // alert(dt);
    if (data == false) {
        alert("OnComplete_load_inside_section return false");
        return;
    }
       
    for (var i = 0; i < data.Sections.length; ++i) {
        var obg_tmp = { Id: data.Sections[i].Id, Head: data.Sections[i].Head, SectionParrentId: data.Sections[i].SectionParrentId };
        mass_section.push(obg_tmp)
    }
    for (var i = 0; i < data.Articles.length; ++i) {
        var obg_tmp = { Id: data.Articles[i].Id, Head: data.Articles[i].Head, Body: null, SectionParrentId: data.Articles[i].SectionParrentId };
        mass_article.push(obg_tmp)
    }

    var inside = document.getElementById("div_one_section_inside_" + data.Id);

    inside.innerHTML = load_one_section_data(data.Id);
    
    //= load_one_section(mass_section[0].Id);
    //left_div.innerHTML = str_add_name_section(mass_section[0].Id) + str_res_for_left_ul;

    open_section(data.Id);
}
function OnComplete_load_article_body(data) {
    //var dt = JSON.parse(data.responseText);
    if (data == false) {
        alert("OnComplete_load_inside_section return false");
        return;
    }
    var block = find_in_mass(data.Id, 2);
    block.Body = data.Body;
    show_article(data);
}


function OnComplete_Add_section(data) {
    
    if (data == false) {
        alert("OnComplete_Add_section return false");
        return;
    }
    var obj = { Id: data.Id, Head: data.Head, SectionParrentId: data.SectionParrentId };
    mass_section.push(obj);

    document.getElementById("main_block_right_id").innerHTML = "";

    var inside_sect = document.getElementById("div_inside_sections_" + obj.SectionParrentId);
    var str= str_add_name_section(obj.Id) + load_one_section(obj.Id);
    inside_sect.innerHTML += str;
        
}
function OnComplete_Add_article(data) {
    
    if (data == false) {
        alert("OnComplete_Add_article return false");
        return;
    }
    var obj = { Id: data.Id, Head: data.Head, Body: null, SectionParrentId: data.SectionParrentId };
    mass_article.push(obj)
    document.getElementById("main_block_right_id").innerHTML = "";
    var tmp = "";
    tmp += "<div class='div_one_article_name' id='div_one_article_name_" + obj.Id + "' onclick='load_article(" + obj.Id + ")'>" + obj.Head + "</div>";
    var inside_sect = document.getElementById("div_inside_articles_" + obj.SectionParrentId);
    inside_sect.innerHTML += tmp;
}

function OnComplete_edit_section(data) {
    
    if (data == false) {
        alert("OnComplete_edit_section return false");
        return;
    }
    
    for (var i = 0; i < mass_section.length; ++i) {
        if (mass_section[i].Id == data.Id) {
            mass_section[i].Head = data.Head;
            document.getElementById("div_one_section_name_text_" + data.Id).innerHTML = data.Head;
            
            break;
        }
    }
    document.getElementById("main_block_right_id").innerHTML="";
}
function OnComplete_edit_article(data) {
   
    if (data == false) {
        alert("OnComplete_edit_article return false");
        return;
    }
    for (var i = 0; i < mass_article.length; ++i) {
        if (mass_article[i].Id == data.Id) {
            mass_article[i].Head = data.Head;
            mass_article[i].Body = data.Body;
            document.getElementById("div_one_article_name_" + data.Id).innerHTML = data.Head;
            load_article(data.Id);
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
    //var right_div = document.getElementById("main_block_right_id");
    
   // var res = '';
    if (last_click_name.indexOf("div_one_section_name") >= 0) {
        //res += add_form_for_add_or_edit(id, 1);
        add_form_for_add_or_edit(id, 1);
    }
    else if (last_click_name.indexOf("div_one_article_name") >= 0) {
        //res += add_form_for_add_or_edit(id, 2);
        add_form_for_add_or_edit(id, 2);
    }
    //right_div.innerHTML = res;
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
    
    var dt = { 'id': id };
    $.ajax({
        url: "/Info/Delete_section",
        data: dt,
        success: OnComplete_delete_section,
        error: function () {
            alert("ошибка загрузки");
            PreloaderAction(false);
        },
        beforeSend: function () { PreloaderAction(true); },
        complete: function () { PreloaderAction(false); },
        type: 'POST', dataType: 'json'
    });


}

function delete_article_f(id) {

    var dt = { 'id': id };
    $.ajax({
        url: "/Info/Delete_article",
        data: dt,
        success: OnComplete_delete_article,
        error: function () {
            alert("ошибка загрузки");
            PreloaderAction(false);
        },
        beforeSend: function () { PreloaderAction(true); },
        complete: function () { PreloaderAction(false); },
        type: 'POST', dataType: 'json'
    });

}
function OnComplete_delete_section(data) {
    //var dt = JSON.parse(data.responseText);
    if (data == false) {
        alert("OnComplete_Add_article return false");
        return;
    }
    if (data.parrent_id_main == null)
        for (var i2 = 0; i2 < mass_article.length; ++i2)
            if (mass_article[i2].SectionParrentId == data.main_id)
                mass_article.splice(i2--, 1);
            
        

    for (var i = 0; i < data.sec_list.length; ++i) {
        for (var i2 = 0; i2 < mass_article.length; ++i2) {
            if (mass_article[i2].SectionParrentId == data.sec_list[i]) {
                mass_article.splice(i2--,1);
            }
        }
        go:
            for (var i2 = 0; i2 < mass_section.length; ++i2) 
                if (mass_section[i2].Id == data.sec_list[i]) {
                    mass_section.splice(i2--, 1);
                    break go;
                }
                   
            
    }
    if (data.parrent_id_main == null) {
        var inside = document.getElementById("div_one_section_inside_" + data.main_id);
        inside.innerHTML = load_one_section_data(data.main_id);
    }
    else {
        document.getElementById("div_one_section_name_" + data.main_id).remove();
        document.getElementById("div_one_section_inside_" + data.main_id).remove();
        
    }
    
   
   
}
function OnComplete_delete_article(data) {
   // var dt = JSON.parse(data.responseText);
    if (data == false) {
        alert("OnComplete_Add_article return false");
        return;
    }
    for (var i2 = 0; i2 < mass_article.length; ++i2) {
        if (mass_article[i2].Id == data) {
            mass_article.splice(i2--, 1);
            break;
        }
    }

    document.getElementById('div_one_article_name_' + data).remove();
}



function send_form(type) {

    switch (type) {
        case 1:
         
  //          $.post("/Info/Add_section",
  //              {
  //                  Head: convert_string(document.getElementById("input_for_section_head").value),
  //                  parrent_sec_id: document.getElementById("input_for_parrent_sec_id").value
  //              })
            //.done(OnComplete_Add_section);
            var dt={'Head':convert_string(document.getElementById("input_for_section_head").value),'parrent_sec_id':document.getElementById("input_for_parrent_sec_id").value};
            $.ajax({
                url: "/Info/Add_section",
                data:dt,
                success: OnComplete_Add_section,
                error: function () {
                    alert("ошибка загрузки");
                    PreloaderAction(false);
                },
                beforeSend: function () { PreloaderAction(true); },
                complete: function () { PreloaderAction(false); },
                type: 'POST', dataType: 'json'
            });
            break;
        case 2:

            var dt = { 'Head': convert_string(document.getElementById("input_for_section_head").value), 'Id': document.getElementById("input_for_sec_id").value };
            $.ajax({
                url: "/Info/Edit_section",
                data: dt,
                success: OnComplete_edit_section,
                error: function () {
                    alert("ошибка загрузки");
                    PreloaderAction(false);
                },
                beforeSend: function () { PreloaderAction(true); },
                complete: function () { PreloaderAction(false); },
                type: 'POST', dataType: 'json'
            });

            break;
        case 3:
        
            var dt = {
                'Head': convert_string(document.getElementById("input_for_article_head").value),
                'Body': convert_string(document.getElementById("input_for_article_body").value),
                'parrent_sec_id': document.getElementById("input_for_parrent_sec_id").value
            };
            $.ajax({
                url: "/Info/Add_article",
                data: dt,
                success: OnComplete_Add_article,
                error: function () {
                    alert("ошибка загрузки");
                    PreloaderAction(false);
                },
                beforeSend: function () { PreloaderAction(true); },
                complete: function () { PreloaderAction(false); },
                type: 'POST', dataType: 'json'
            });

            break;
        case 4:

            var dt = {
                'Head': convert_string(document.getElementById("input_for_article_head").value),
                'Body': convert_string(document.getElementById("input_for_article_body").value),
                'Id': document.getElementById("input_for_art_id").value
            };
            $.ajax({
                url: "/Info/Edit_article",
                data: dt,
                success: OnComplete_edit_article,
                error: function () {
                    alert("ошибка загрузки");
                    PreloaderAction(false);
                },
                beforeSend: function () { PreloaderAction(true); },
                complete: function () { PreloaderAction(false); },
                type: 'POST', dataType: 'json'
            });

            break;
        default:
            alert("что то пошло не так");
            break;
    }

}




function add_form_for_add_or_edit(id, type) {//1 секция 2 статья
    var res = '';
    switch (type) {
        case 1:
            var block = find_in_mass(id, type);
            res += '<div><div><label>Заголовок</label></div>';
            
            res += '<textarea name="Head" class="text_area_add_edit" id="input_for_section_head">' + (block == null ? '' : block.Head) + '</textarea>';
            if (block == null){
                res += "<input name='parrent_sec_id' id='input_for_parrent_sec_id' type='hidden' value=" + click_sect_id() + " />";
                res += "<button   onclick='send_form(1)'>Добавить секцию</button>";
            }
                
            else {
                res += "<input name='Id' id='input_for_sec_id' type='hidden' value=" + id + " />";
                res += "<button  onclick='send_form(2)' >Сохранить</button>";
            }
                

            res += '</div>';
            break;
        case 2:
            var block = find_in_mass(id, type);

            res += '<div><div><label>Заголовок</label></div>';
            res += '<textarea name="Head" class="text_area_add_edit" id="input_for_article_head">' + (block == null ? '' : block.Head) + '</textarea>';
            res += '<div><label>Содержание</label></div>';
            res += '<textarea name="Body" class="text_area_add_edit" id="input_for_article_body">' + (block == null ? ("\n \n"+GetHashtagForNew()) : block.Body) + '</textarea>';
            if (block == null) {
                res += "<input name='parrent_sec_id' id='input_for_parrent_sec_id' type='hidden' value=" + click_sect_id() + " />";
                res += "<button onclick='send_form(3)' >Добавить статью</button>";
                
                
            }
               
            else {
                res += "<input name='Id' id='input_for_art_id' type='hidden' value=" + id + " />";
                res += "<button onclick='send_form(4)' >Сохранить</button>";
            }
                
                
            res += '</div>';
            break;
        default:
            alert("error");
            break;
    }
    document.getElementById("main_block_right_id").innerHTML = res;
    //return res;
}


function GetHashtagForNew() {
    var str = ' ';
    var p_id = click_sect_id();
    var mass_id = (p_id + " " + GetParrentSectionId(p_id)).split(' ');
    var str_hash = '';

    for (var i = 0; i < mass_id.length; ++i) {
        for (var i2 = 0; i2 < mass_section.length; ++i2) {
            if (mass_section[i2].Id == mass_id[i]) {
                str+=mass_section[i2].Head+" ";

            }
        }
    }
    var gg = str.replace(/\s+/gi," #");
    return gg;
}
function GetParrentSectionId(id) {
    var res = '';
    for (var i = 0; i < mass_section.length; ++i) {
        if (mass_section[i].Id == id) {
            if (mass_section[i].SectionParrentId != null && mass_section[i].SectionParrentId != 0) {
                res += ' ' + mass_section[i].SectionParrentId;
                res += GetParrentSectionId(mass_section[i].SectionParrentId);
            }
            break;
        }
           
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

            var dt = { 'id': id_ar };
            $.ajax({
                url: "/Info/Load_article_body",
                data: dt,
                success: OnComplete_load_article_body,
                error: function () {
                    alert("ошибка загрузки");
                    PreloaderAction(false);
                },
                beforeSend: function () { PreloaderAction(true); },
                complete: function () { PreloaderAction(false); },
                type: 'POST', dataType: 'json'
            });


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
        return find_in_mass(last_click_name.split('_')[4], 2).SectionParrentId;
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

    location.href = '/Info/Index';
    //alert("home_button_return_left currently not implemented");
}

function OnComplete_load_all_data_for_save_file(data) {
    if (data == false) {
        alert("OnComplete_load_all_data_for_save_file return false");
        return;
    }
    var pom = document.createElement('a');
    pom.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(JSON.stringify(data)));
    pom.setAttribute('download', 'data.json');
    pom.click();
}
function save_server_db() {
    $.ajax({
        url: "/Info/Download_data_file",
        data: null,
        success: OnComplete_load_all_data_for_save_file,
        error: function () {
            alert("ошибка загрузки");
            PreloaderAction(false);
        },
        beforeSend: function () { PreloaderAction(true); },
        complete: function () { PreloaderAction(false); },
        type: 'POST', dataType: 'json'
    });
}

function OnComplete_load_all_data_for_save_server(data) {

    if (data == false) {
        alert("OnComplete_load_all_data_for_save_server return false");
        return;
    }
    location.href = '/Info/Info_page';

}


function OnComplete_search(data) {
    var funct_sort = function (a, b) {
        if (a.Mark < b.Mark) return 1;
        if (a.Mark > b.Mark) return -1;
    };


    if (data == false) {
        alert("OnComplete_search return false");
        return;
    }


    data.sort(funct_sort);

    //var mass_inside = [];
    //for (var i = 0; i < data.length; ++i) {
    //    if (data[i].inside) 
    //        mass_inside.push(data[i]);
        
    //}
    var div = document.getElementById("main_block_left_id");
    div.innerHTML = "";
    var res1 = "<div class='div_inside_articles'>";
    var res2 = "<div class='div_inside_articles'>";
    
    //res += "<div class='div_one_article_name' id='div_one_article_name_" + mass_article[i].Id + "' onclick='load_article(" + mass_article[i].Id + ")'>" + mass_article[i].Head + "</div>";//convert_string(mass_article[i].Head)
    for (var i = 0; i < data.length; ++i) {
        var tmp_art = "<div class='div_one_article_name' id='div_one_article_name_"
            + data[i].Id + "' onclick='load_article(" + data[i].Id + ")'>"
            + data[i].Head + "</div>";
        if (data[i].inside)
            res1 += tmp_art;
        else
            res2 += tmp_art;
    }
    res1 += "</div>";
    res2 += "</div>";
    //
    //res += "<div>";

    //for (var i = 0; i < data.length; ++i) {
    //    res += "<div class='div_one_article_name' id='div_one_article_name_"
    //        + data[i].Id + "' onclick='load_article(" + data[i].Id + ")'>"
    //        + data[i].Head + "</div>";
    //}

    //res += "</div>";
    div.innerHTML = res1 +"<hr/>"+ res2;
   
}

function load_left_block() {
    //var funct_sort = function (a, b) {
    //    if (a.Id < b.Id) return 1;
    //    if (a.Id > b.Id) return -1;
    //};
    function get_all_parrent(id,mass) {

        //var res = [];
        for (var i = 0; i < mass_section.length; ++i) 
            if (mass_section[i].Id == id) {
                if (mass_section[i].SectionParrentId != null) {
                    var break_=false;
                    for (var i2 = 0; i2 < mass; ++i2) {
                        // ТУТ не надо добавлять в массив + и родителей тоже если уже есть в последовательности
                        if (mass[i2] == mass_section[i].SectionParrentId) {
                            break_ = true;
                        }

                    }
                    if (!break_) {
                        mass.push(mass_section[i].SectionParrentId);
                        get_all_parrent(mass_section[i].SectionParrentId, mass);
                    }
                   
                }
                break;
            }
        
        return ;
    }
    function load_sec(id, mass) {

        for (var i = 0; i < mass_section.length; ++i) {
            if (mass_section[i].SectionParrentId == id) {
                for (var i2 = 0; i2 < mass.length; ++i2) {
                    if (mass[i2] == mass_section[i].Id) {
                        mass.splice(i2, 1);
                        //click_name_section(document.getElementById("div_one_section_name_" + mass_section[i].Id), true);

                        //var div;
                        //div.innerHTML = str_add_name_section(main_sec_id) + load_one_section(main_sec_id);
                        var inside = document.getElementById("div_one_section_inside_" + mass_section[i].Id);

                        inside.innerHTML = load_one_section_data(mass_section[i].Id);


                        load_sec(mass_section[i].Id,mass);
                    }
                }
                

            }
        }

    }
    var div = document.getElementById("main_block_left_id");

    var mass_parrent = mass_section_open.slice();

    for(var i=0;i<mass_section_open.length;++i){
        get_all_parrent(mass_section_open[i], mass_parrent);
    }
    
    var main_sec_id = null;
    for (var i = 0; i < mass_section.length; ++i) {

        if(main_sec_id==null||main_sec_id>mass_section[i].Id)
            main_sec_id = mass_section[i].Id;
    }

    //var res = "";

    
    div.innerHTML = str_add_name_section(main_sec_id) + load_one_section(main_sec_id);
    var inside = document.getElementById("div_one_section_inside_" + main_sec_id);

    inside.innerHTML = load_one_section_data(main_sec_id);
    //click_name_section(document.getElementById("div_one_section_name_" + main_sec_id),true);
    load_sec(main_sec_id, mass_parrent);
    //div.innerHTML = res;
    var mass_sec_open_ = mass_section_open.slice();
    mass_section_open = [];
    for (var i = 0; i < mass_sec_open_.length;++i) {
        //click_name_section(document.getElementById("div_one_section_name_" + mass_section_open[i]));
        //mass_sec_open_.splice(i, 1);
        open_section(mass_sec_open_[i]);
    }
}



function upload_text_all_data() {
    var div=document.getElementById("main_block_right_id");
    var res = "<div>";
    //res += '<textarea name="upload_text" class="text_area_add_edit" id="input_for_text_all_data"></textarea>';
    //res+="<button onclick='before_load_all_data_text()'>Загрузить</button>";
    res += '<input type="file" onchange="before_load_all_data_text(this.files)" id="file">';
    res += "</div>";
    div.innerHTML = res;
}

function before_load_all_data_text(files) {
    if (!confirm("ВНИМАНИЕ!!! ВСЕ данные на сервере(список секций, статей, картинок из статей) будут перезаписаны на указанные в поле! Продолжить??")) {
        var div = document.getElementById("main_block_right_id");
        div.innerHTML = "";
        return;
    }
        
    var string = '';
    var file = files[0];
    if (file) {
        var reader = new FileReader();
        reader.onload = function (e) {
            string = e.target.result;

            string = convert_string(string);
            var dt = { upload_text: string };
            $.ajax({
                url: "/Info/Upload_data_file_text",
                data: dt,
                success: OnComplete_load_all_data_for_save_server,
                error: function () {
                    alert("ошибка загрузки");
                    PreloaderAction(false);
                },
                beforeSend: function () { PreloaderAction(true); },
                complete: function () { PreloaderAction(false); },
                type: 'POST', dataType: 'json'
            });
        }
    }
    reader.readAsText(file);

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


function start_search() {
    var string = document.getElementById("search_string_client").value;
   
    var dt = { src: str_to_str_search(string), id_for_search: click_sect_id() };
    $.ajax({
        url: "/Info/Start_search",
        data: dt,
        success: OnComplete_search,
        error: function () {
            alert("ошибка загрузки");
            PreloaderAction(false);
        },
        beforeSend: function () { PreloaderAction(true); },
        complete: function () { PreloaderAction(false); },
        type: 'POST', dataType: 'json'
    });

    
    


    function str_to_str_search(str) {
        //var res=[];
        //var reg1 = new RegExp("(\w+)(\s*\+\s*|\s*#\s*|\s*(\(.*\))?\s*)", "gi");
        var reg1 = new RegExp("(\\w+)(\\s*\\+\\s*)", "gi");
        str = str.replace(reg1, '$1 + ');
        reg1 = new RegExp("(\\w+)(\\s*#\\s*)", "gi");
        str = str.replace(reg1, '$1 # ');
        reg1 = new RegExp("\\s*\\((.*?)\\)\\s*", "gi");//"(\\w+)(\\s*\(.*?\)\\s*)", "gi"
        str = str.replace(reg1, '( $1 )');
        return str;
    }

}