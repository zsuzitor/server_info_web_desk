using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace server_info_web_desk.Models.functions
{
    public static class FunctionsProject
    {

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