﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace server_info_web_desk.Models.functions
{
    public static class FunctionsProject
    {
        //1 надо ли вычитать если get_start==true то не надо 2 с конца списка ли надо брать
        public static List<T> GetPartialList<T>(ICollection<T> list,int start,int? count,bool get_start=false,bool end=true)
        {
            //res.AddRange((List<ApplicationUser>)GetPartialList<ApplicationUser>(this.Friends, start, count));
            if (count == null)
                count = list.Count;
            int skipT=start;
            int countT = (int)count;
            if (!get_start)
                skipT = skipT > 0 ? skipT - 1 : 0;
            if (end)
                skipT = list.Count - skipT - (int)countT;
            if (skipT < 0)
            {
                countT = countT + skipT;
                skipT = 0;
            }
            return list.Skip(skipT).Take((int)countT).ToList();

        }
        public static List<byte[]> Get_photo_post(HttpPostedFileBase[] uploadImage)
        {

            /* сохранение картинок как файл ...
              HttpPostedFileBase image = Request.Files["fileInput"];
            
            if (image != null && image.ContentLength > 0 && !string.IsNullOrEmpty(image.FileName))
            {
                string fileName = image.FileName;
                image.SaveAs(Path.Combine(Server.MapPath("Images"), fileName));
            }
             
             * */
            List<byte[]> res = new List<byte[]>();
            if (uploadImage != null)
            {
                foreach (var i in uploadImage)
                {
                    try
                    {
                        byte[] imageData = null;
                        // считываем переданный файл в массив байтов
                        using (var binaryReader = new BinaryReader(i.InputStream))
                        {
                            imageData = binaryReader.ReadBytes(i.ContentLength);
                        }
                        // установка массива байтов
                        res.Add(imageData);
                    }
                    catch
                    {}
                }

            }

            return res;
        }


    }
}