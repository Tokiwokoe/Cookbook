﻿using Npgsql;
using MainForm.BD;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace bd
{
    internal static class ControllerForBD
    {
        private static string configConnection;

        public static List<Recipe> myRecipes;
        public static List<Recipe> inetRecipes;
        public static List<Recipe> starRecipes;
        public static List<Recipe> searchRecipes;
        public static Boolean isStartMy;
        public static Boolean isStartInet;
        public static Boolean isStartStar;
        public static Boolean isStartSearch;
        public static Boolean isDone;
        public static Boolean isDoneMy;
        public static Boolean isDoneInet;
        public static Boolean isDoneStar;
        public static Boolean isDoneSearch;
        public static Boolean isDoneFile;
        public static Boolean isStartFile;
        public static List<Recipe> fileRecipes;
        /*
        Метод подключения:
        параметром является строка с информацией о сервере,
        порте, пользователе, пароле и базой данных
        в данной программе конфигурация у всех одинакова, кроме пароля(у каждого свой)
        метод возвращает тру - если подключен, фолс - если нет.
        Пример конфига: "Server = localhost; Port = 5432;UserId = postgres; Password = пароль; Database = MyDatabase; "
        */
        public static bool Сonnect(string config_connect)
        {

            try
            {

                configConnection = config_connect;
                NpgsqlConnection test = new NpgsqlConnection(config_connect);
                test.Open();
                test.Close();
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine($"Connection error: \n {e} ");
                return false;
            }
        }

        /*
        Метод добавления данных для пользователя:
        параметрами являются следующие данные в виде строк: 
        название, категория, ингридиенты, инструкция, оценка, цена , время(особенность см. ниже)
        оценку и цену перед переводом в строку ОКРУГЛИТЬ ДО ДВУХ ЗНАКОВ ПОСЛЕ ЗАПЯТОЙ!!!
        Временно добавление не включает в себя картинки
        Использовать только после подключения!
        Время требуется передавать в виде строки в след. формате: "12:00:00"
        Возвращает тру - если добавил, фолс - если нет.
        */
        public static bool InsertToMyRecipes(string name, string category, string ingredients, string guide, string marklike, string markdif, string time, byte[] image)
        {
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(configConnection);
                connection.Open();
                string textCommand = "Insert into recipes(name, category, ingredients, guide, time, marklike, markdif, type )";
                textCommand += $"values ('{name}','{category}','{ingredients}','{guide}','{time}',{marklike},{markdif}, 0)";
                textCommand += " returning id;";
                NpgsqlCommand npgsqlCommand = new NpgsqlCommand(textCommand, connection);
                int id = (int)npgsqlCommand.ExecuteScalar();
                npgsqlCommand = new NpgsqlCommand("Insert into images(id, pic) values (" + id + ", @Image )", connection);
                NpgsqlParameter parameter = npgsqlCommand.CreateParameter();
                parameter.ParameterName = "@Image";
                parameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bytea;
                parameter.Value = image;


                npgsqlCommand.Parameters.Add(parameter);


                try
                {
                    npgsqlCommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {

                }
                Console.WriteLine($"Recipe is insert with id = {id}");
                connection.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Insert error: \n {e} ");

                return false;
            }
        }

        public static bool InsertToInetRecipes(string name, string category, string ingredients, string guide, string marklike, string markdif, string time, byte[] image)
        {
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(configConnection);
                connection.Open();
                string textCommand = "Insert into recipes(name, category, ingredients, guide, time, marklike, markdif, type )";
                textCommand += $"values ('{name}','{category}','{ingredients}','{guide}','{time}',{marklike},{markdif}, 1)";
                textCommand += " returning id;";
                NpgsqlCommand npgsqlCommand = new NpgsqlCommand(textCommand, connection);

                int id = (int)npgsqlCommand.ExecuteScalar();
                npgsqlCommand = new NpgsqlCommand("Insert into images(id, pic) values (" + id + ", @Image )", connection);
                NpgsqlParameter parameter = npgsqlCommand.CreateParameter();
                parameter.ParameterName = "@Image";
                parameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bytea;
                parameter.Value = image;
                npgsqlCommand.Parameters.Add(parameter);



                npgsqlCommand.ExecuteNonQuery();

                Console.WriteLine($"Recipe is insert with id = {id}");
                connection.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Insert error: \n {e} ");
                return false;
            }
        }



        /*
        Метод получения по айди:
        При выводе списков рецептов дается не вся информация о рецептах,
        поэтому при выборе пользователем определенного рецепта, через айди дается полная информация

        параметр TABLE - из какой таблицы беруться данные: избранные = starrecipes, пользовательские = myrecipes, с интернета = inetrecipes, все рецепты = recipes
         */


        public static Recipe SelectById(int id, string table)
        {
            try
            {

                // !!!!!!!!!!!!!!!!!!
                NpgsqlConnection connection = new NpgsqlConnection(configConnection);
                connection.Open();
                // !!!!1!!!!!!!!!!!!!!!!!!
                string textCommand;
                if (table.Equals("starrecipes"))
                {
                    textCommand = $"Select recipes.id, category, ingredients, guide, marklike, name, markdif,star, time, images.pic from recipes left join Images on recipes.id = images.id  where ((recipes.id = {id}) and (star = true))";
                }
                else if (table.Equals("myrecipes"))
                {
                    textCommand = $"Select  recipes.id, category, ingredients, guide, marklike, name, markdif,star, time, images.pic from recipes left join Images on recipes.id = images.id where ((recipes.id = {id}) and (type = 0))";
                }
                else if (table.Equals("inetrecipes"))
                {
                    textCommand = $"Select  recipes.id, category, ingredients, guide, marklike, name, markdif,star, time, images.pic from recipes left join Images on recipes.id = images.id where ((recipes.id = {id}) and (type = 1))";
                }
                else
                {
                    textCommand = $"Select  recipes.id, category, ingredients, guide, marklike, name, markdif,star, time, images.pic from recipes left join Images on recipes.id = images.id where ((recipes.id = {id}))";
                }
                Recipe r = null;

                var command = new NpgsqlCommand(textCommand, connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    byte[] picture = null;
                    r = new Recipe(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetTimeSpan(8).ToString(), reader.GetString(3), reader.GetDouble(4).ToString(), reader.GetString(5), reader.GetDouble(6).ToString(), reader.GetBoolean(7));
                    picture = reader[9] as byte[];
                    r.Pic = picture;




                }
                reader.Close();
                connection.Close();
                return r;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error of select by id : \n" + e);
                return null;
            }
        }
        /*
        Метод получения рецептов из таблицы Мои рецепты:  StartSelectAllMyRecipes

        myRecipes - массив, в котором хранятся полученные из БД рецепты
        isDoneMy - булевая, которая сигнализирует, что вся инфа из БД загруженны
        метод запускает параллельный поток с загрузкой
        ПРИМЕР ИСПОЛЬЗОВАНИЯ (на примере просто вывод в консоль, !!! - то, что нужно изменить, остальное - оставить):


          ControllerForBD.StartSelectAllMyRecipes();
          bool isAll = false;
                while (!isAll)
                {
                    if (ControllerForBD.myRecipes != null)
                    {
                        if (ControllerForBD.myRecipes.Count != 0)
                        {
                            Recipe r = ControllerForBD.myRecipes.ElementAt(0);
                          !!!  Console.WriteLine(r.Name); !!!
                            ControllerForBD.myRecipes.Remove(r);
                        }
                        if ((ControllerForBD.myRecipes.Count == 0) && (ControllerForBD.isDoneMy))
                        {
                            isAll = true;
                        }
                    }
                    
                }




        Пример перевода из байт в картинку

        if (picture != null)
                    {
                        using (MemoryStream productImageStream = new System.IO.MemoryStream(picture))
                        {
                            ImageConverter imageConverter = new System.Drawing.ImageConverter();
                            pictureBox1.Image = imageConverter.ConvertFrom(productImageByte) as System.Drawing.Image;
                        }
                    }

        // ------------------------------------------------------------------------------------------------------------------------------------------------

        
        // Объект ImageConverter, используемый для преобразования массивов байтов, содержащих изображения файлов JPEG или PNG, в объекты Bitmap. 

        private static readonly ImageConverter _imageConverter = new ImageConverter();

        // Image to byte array:

        // Метод для «преобразования» объекта изображения в массив байтов, отформатированный в формате файла PNG, который
        // обеспечивает сжатие без потерь. Можно использовать вместе с GetImageFromByteArray().
  

          public static byte[] CopyImageToByteArray(Image theImage)
                  {
                   using (MemoryStream memoryStream = new MemoryStream())
                       {
                          theImage.Save(memoryStream, ImageFormat.Png);
                          return memoryStream.ToArray();
                       }
                  } 
         
       // Byte to image:

       


         // Чтобы получить изображение из файла jpg или png, вы должны прочитать файл в массив байтов, используя File.ReadAllBytes():

          Bitmap newBitmap = GetImageFromByteArray(File.ReadAllBytes(fileName));

         //Это позволяет избежать проблем, связанных с тем, что Bitmap хочет, чтобы его исходный поток оставался открытым, и
         //некоторых предлагаемых обходных путей для этой проблемы, которые приводят к тому, что исходный файл остается заблокированным.

        // ------------------------------------------------------------------------------------------------------------------------------------------------

         */
        private static void SelectAllMyRecipes()
        {
            myRecipes = new List<Recipe>();
            isDoneMy = false;
            isStartMy = false;
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(configConnection);
                connection.Open();
                myRecipes = new List<Recipe>();
                Recipe r = null;

                string textCommand = "Select recipes.id, name ,  category, time, marklike, markdif, star, pic from recipes left join Images on recipes.id = images.id  where type = 0;";

                var command = new NpgsqlCommand(textCommand, connection);
                var reader = command.ExecuteReader();
                isDoneMy = false;
                isStartMy = true;
                while (reader.Read())
                {
                    byte[] picture = null;
                    r = new Recipe(reader.GetInt32(0), reader.GetString(2), null, reader.GetTimeSpan(3).ToString(), null, reader.GetDouble(4).ToString(), reader.GetString(1), reader.GetDouble(5).ToString(), reader.GetBoolean(6));

                    picture = reader[7] as byte[];
                    r.Pic = picture;

                    myRecipes.Add(r);
                }
                isDoneMy = true;
                reader.Close();
                connection.Close();
            }
            catch (Exception e)
            {
                isStartMy = false;
                Console.WriteLine("Error of select all from MyRecipes : \n" + e);
                myRecipes = null;
                isDoneMy = true;
            }
        }
        public static void StartSelectAllMyRecipes()
        {
            try
            {

                Thread th = new Thread(SelectAllMyRecipes);
                th.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        private static void SelectAllInetRecipes()
        {
            inetRecipes = new List<Recipe>();
            isDoneInet = false;
            isStartInet = false;
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(configConnection);
                connection.Open();
                inetRecipes = new List<Recipe>();
                Recipe r = null;


                //ВОЗМОЖНА ОШИБКА!


                string textCommand = "Select recipes.id, name ,  category, time, marklike, markdif, star, pic from recipes left join Images on recipes.id = images.id  where type = 1;";





                var command = new NpgsqlCommand(textCommand, connection);
                var reader = command.ExecuteReader();
                isDoneInet = false;
                isStartInet = true;
                while (reader.Read())
                {
                    byte[] picture = null;
                    r = new Recipe(reader.GetInt32(0), reader.GetString(2), null, reader.GetTimeSpan(3).ToString(), null, reader.GetDouble(4).ToString(), reader.GetString(1), reader.GetDouble(5).ToString(), reader.GetBoolean(6));
                    picture = reader[7] as byte[];
                    r.Pic = picture;
                    inetRecipes.Add(r);
                }
                isDoneInet = true;
                reader.Close();
                connection.Close();
            }
            catch (Exception e)
            {
                isStartInet = false;
                Console.WriteLine("Error of select all from InetRecipes : \n" + e);
                inetRecipes = null;
                isDoneInet = true;
            }
        }
        public static void StartSelectAllInetRecipes()
        {
            try
            {

                Thread th = new Thread(SelectAllInetRecipes);
                th.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private static void SelectAllStarRecipes()
        {
            starRecipes = new List<Recipe>();
            isDoneStar = false;
            isStartStar = false;
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(configConnection);
                connection.Open();
                starRecipes = new List<Recipe>();
                Recipe r = null;


                //ВОЗМОЖНА ОШИБКА!


                string textCommand = "Select recipes.id, name ,  category, time, marklike, markdif, star, pic from recipes left join Images on recipes.id = images.id  where star = true;";





                var command = new NpgsqlCommand(textCommand, connection);
                var reader = command.ExecuteReader();
                isDoneStar = false;
                isStartStar = true;
                while (reader.Read())
                {
                    byte[] picture = null;
                    r = new Recipe(reader.GetInt32(0), reader.GetString(2), null, reader.GetTimeSpan(3).ToString(), null, reader.GetDouble(4).ToString(), reader.GetString(1), reader.GetDouble(5).ToString(), reader.GetBoolean(6));
                    picture = reader[7] as byte[];
                    r.Pic = picture;
                    starRecipes.Add(r);
                }
                isDoneStar = true;
                reader.Close();
                connection.Close();
            }
            catch (Exception e)
            {
                isStartStar = false;
                Console.WriteLine("Error of select all from MyRecipes : \n" + e);
                starRecipes = null;
                isDoneStar = true;
            }
        }
        public static void StartSelectAllStarRecipes()
        {
            try
            {

                Thread th = new Thread(SelectAllStarRecipes);
                th.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void selectAllFileRecipes()
        {

            fileRecipes = new List<Recipe>();
            isDoneFile = false;
            isStartFile = false;
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(configConnection);

                connection.Open();

                fileRecipes = new List<Recipe>();

                Recipe r = null;

                string textCommand = "Select id, name ,  category, time, marklike, markdif,guide,ingredients from recipes  where type = 0;";

                var command = new NpgsqlCommand(textCommand, connection);
                var reader = command.ExecuteReader();
                isDoneFile = false;
                isStartFile = true;
                while (reader.Read())
                {
                    r = new Recipe(reader.GetInt32(0), reader.GetString(2), reader.GetString(7), reader.GetTimeSpan(3).ToString(), reader.GetString(6), reader.GetDouble(4).ToString(), reader.GetString(1), reader.GetDouble(5).ToString(), false);

                    fileRecipes.Add(r);
                }
                isDoneFile = true;
                reader.Close();
                connection.Close();
            }
            catch (Exception e)
            {
                isStartFile = false;
                Console.WriteLine("Error of select all from File : \n" + e);
                fileRecipes = null;
                isDoneFile = true;
            }
        }

        public static void StartSelectAllFileRecipes()
        {
            Thread thread = new Thread(selectAllFileRecipes);
            thread.Start();
        }
        public static string createFilter(int type, List<string> categories, List<string> marklikes, List<string> markdifs, bool star)
        {
            string filter = "";

            if (type == 0)
            {
                if (!filter.Equals(""))
                {
                    filter += " and ";
                }
                filter += "type = 0";

            }
            else
                if (type == 1)
            {
                if (!filter.Equals(""))
                {
                    filter += " and ";
                }
                filter += "type = 1";

            }
            string f = "";
            foreach (string cat in categories)
            {

                if (!f.Equals(""))
                {
                    f += " or ";
                }
                if (f.Equals(""))
                {
                    f += "(";
                }
                f += ("category = '" + cat + "'");




            }
            if (!f.Equals(""))
            {
                f += ")";
            }
            if ((!filter.Equals("")) && (!f.Equals("")))
            {
                filter += " and ";
            }
            filter += f;
            f = "";

            foreach (string like in marklikes)
            {

                if (!f.Equals(""))
                {
                    f += " or ";
                }
                if (f.Equals(""))
                {
                    f += "(";
                }
                f += ("marklike = " + like);
            }
            if (!f.Equals(""))
            {
                f += ")";
            }
            if ((!filter.Equals("")) && (!f.Equals("")))
            {
                filter += " and ";
            }
            filter += f;
            f = "";
            foreach (string dif in markdifs)
            {

                if (!f.Equals(""))
                {
                    f += " or ";
                }
                if (f.Equals(""))
                {
                    f += "(";
                }
                f += ("markdif = " + dif);
            }
            if (!f.Equals(""))
            {
                f += ")";
            }
            if ((!filter.Equals("")) && (!f.Equals("")))
            {
                filter += " and ";
            }
            filter += f;
            return filter;
        }
        public static void deleteById(int id)
        {
            string textCommand = $"delete from images where(id = {id})";
            NpgsqlConnection connection = new NpgsqlConnection(configConnection);
            connection.Open();
            var command = new NpgsqlCommand(textCommand, connection);
            command.ExecuteNonQuery();
            textCommand = $"delete from recipes where(id = {id})";
            connection = new NpgsqlConnection(configConnection);
            connection.Open();
            command = new NpgsqlCommand(textCommand, connection);
            command.ExecuteNonQuery();
        }

        public static void StartSelectSearchRecipes(object pair)
        {
            try
            {
                Thread th = new Thread(SelectForSearch);
                th.Start(pair);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void SelectForSearch(object pair)
        {
            searchRecipes = new List<Recipe>();
            isStartSearch = false;
            isDoneSearch = false;
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(configConnection);
                connection.Open();
                searchRecipes = new List<Recipe>();

                string f, text = "";
                string textCommand = "";
                PairSearch p = (PairSearch)pair;
                if (p is PairSearch)
                {

                    textCommand = "Select id, name  from recipes";
                    f = p.filter;
                    text = p.textSearch;
                    if (!f.Equals(""))
                    {
                        textCommand += "where";
                        textCommand += f;
                    }

                    var command = new NpgsqlCommand(textCommand, connection);
                    var reader = command.ExecuteReader();
                    isDoneSearch = false;
                    isStartSearch = true;
                    while (reader.Read())
                    {
                        Recipe r;
                        double index;
                        int id;
                        string name;
                        name = reader.GetString(1);
                        id = reader.GetInt32(0);

                        if (text.Equals(""))
                        {
                            r = SelectById(id, "recipes");
                            searchRecipes.Add(r);
                        }
                        else
                        {
                            if (Search.indexEquals(text, name) >= 49) // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                            {
                                r = SelectById(id, "recipes");
                                searchRecipes.Add(r);

                            }
                        }

                    }
                    isDoneSearch = true;
                    reader.Close();
                    connection.Close();
                }
                else
                {
                    isDoneSearch = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                searchRecipes = null;
                isDoneSearch = true;
                isStartSearch = false;

            }



        }

        public static void alterSearch(PairSearch p)
        {
            searchRecipes = new List<Recipe>();
            isStartSearch = false;
            isDoneSearch = false;
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(configConnection);
                connection.Open();
                searchRecipes = new List<Recipe>();

                string f, text = "";
                string textCommand = "";

                if (p is PairSearch)
                {


                    textCommand = "Select id, name  from recipes";
                    f = p.filter;
                    text = p.textSearch;
                    if (!f.Equals(""))
                    {
                        textCommand += " where ";
                        textCommand += f;
                    }

                    var command = new NpgsqlCommand(textCommand, connection);
                    var reader = command.ExecuteReader();
                    isDoneSearch = false;
                    isStartSearch = true;
                    List<PairForList> pairlist = new List<PairForList>();
                    while (reader.Read())
                    {
                        Recipe r;
                        double index;
                        int id;
                        string name;
                        name = reader.GetString(1);
                        id = reader.GetInt32(0);

                        if (text.Equals(""))
                        {
                            r = SelectById(id, "recipes");
                            searchRecipes.Add(r);
                        }
                        else
                        {

                            double ind = Search.indexEquals(text, name);
                            if (ind >= 49) // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                            {
                                PairForList pair = new PairForList(id, ind);
                                // r = SelectById(id, "recipes");
                                // searchRecipes.Add(r);
                                pairlist.Add(pair);
                            }
                        }

                    }
                    isDoneSearch = true;

                    pairlist.Sort((x, y) => (
                        y.index.CompareTo(x.index)
                    ));
                    if (!text.Equals(""))
                    {
                        foreach (PairForList pair in pairlist)
                        {
                            Recipe r;
                            r = SelectById((int)pair.id, "recipes");
                            searchRecipes.Add(r);
                        }
                    }

                    reader.Close();
                    connection.Close();
                }
                else
                {
                    isDoneSearch = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                searchRecipes = null;
                isDoneSearch = true;
                isStartSearch = false;

            }


        }

        /*
          Метод изменения поля Избранное в Рецепте:
            Параметры: id - айди рецепта
                        star - булевая
         */

        public static void setStar(int id, bool star)
        {
            string textCommand = $"update recipes set star= {star} where(id = {id})";
            NpgsqlConnection connection = new NpgsqlConnection(configConnection);
            connection.Open();
            var command = new NpgsqlCommand(textCommand, connection);
            command.ExecuteNonQuery();


        }


        public static void editRecipe(long id, string name, string category, string ingredients, string guide, string marklike, string markdif, string time, byte[] image)
        {

        }
    }

}
