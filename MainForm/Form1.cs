using bd;
using MainForm.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
// ТЕСТ
namespace MainForm
{
    public partial class MainForm : Form
    {
        public enum Buttons : int//Номера кнопок
        {
            My_Rec = 0,
            Fav_Rec = 1,
            General_Rec = 2,
            Add_Rec = 3,
            Settings = 4,
            Help = 5,
            Start_Page = 6,
            SearchResultPage = 7
        }

        public enum Star_Marks : int//Оценки
        {
            NoMark = 0,
            Mark1 = 1,
            Mark2 = 2,
            Mark3 = 3,
            Mark4 = 4,
            Mark5 = 5
        }

        public int whatClicked = (int)Star_Marks.NoMark;//Какая оценка рецепта выбрана

        bool isPhoto = false;//Загружено ли фото для рецепта

        public int whatButtonClicked = -1;//Какой раздел выбран

        public string ImageFileNameStartPage = Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().Length - 27) + "images\\Main_start_page.png";

        public string ImageFileNameMyRecB = Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().Length - 27) + "images\\my_rec.png";

        public string ImageFileNameFavB = Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().Length - 27) + "images\\fav_rec.png";

        public string ImageFileNameAddRecB = Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().Length - 27) + "images\\add_rec.png";

        public string ImageFileNameSettingsB = Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().Length - 27) + "images\\settings.png";

        public string ImageFileNameHelpB = Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().Length - 27) + "images\\help.png";

        public string ImageFileNameOpacity = Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().Length - 27) + "images\\opacity_star.png";//Пустая звезда

        public string ImageFileNameFull = Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().Length - 27) + "images\\full_star.png";//Заполненная звезда

        public string HeartFileNameOpacity = Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().Length - 27) + "images\\opacity_heart.png";

        public string HeartFileNameFull = Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().Length - 27) + "images\\full_heart.png";
        
        public string ImageAddRec = Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().Length - 27) + "images\\add.png";//Добавление рецепта

        public string StandartPhotoImage = Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().Length - 27) + "images\\standart_photo.jpg";

        //public Instruments Instruments;

        public Thread thread;

        Recipe main_recipe;//Для показа инф-ии о выбранном рецепте

        bool isCollapsed = true;//Переменная отрисовки панели

        bool isRecipe;

        int i = 0;//Считает интервалы для добавления рецептов

        int counter = 0;//Считает количество отображенных рецептов

        int partsForPanel = 18;

        bool isMy;

        public MainForm()
        {
            InitializeComponent();

            //ControllerForBD.Сonnect("Server = localhost; Port = 5432;UserId = postgres; Password =postgres; Database = test2; "); //Подключение БД

            formChanges(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height - 50);

            setColors();

            languageChanges();

            LangCB.SelectedIndex = 0;//Начальный язык - русский

            markDif.SelectedIndex = 0;//Начальная оценка сложности  - 1

            tabContr.SelectedIndex = (int)Buttons.Start_Page;//Стартовая страница

            btnChangePos();

        }

        private void closeB_Click(object sender, EventArgs e)//Кнопка закрытия
        {
            Close();
        }

        //Обработка нажатия кнопок меню

        private void myRecB_Click(object sender, EventArgs e)//Раздел "Мои рецепты"
        {

        }

        public static void loadAllRecipesIntoFile()
        {
           
        }

        public void showAllMyRecipes()//Вывести все "Мои рецепты"
        {
            Action action = () => my_recipes_list.Controls.Clear();

            if (InvokeRequired) { Invoke(action); }

            else { my_recipes_list.Controls.Clear(); }

            i = counter = 0;

            bool isAll = false;

            isRecipe = false;

            my_recipes_list.Invoke((MethodInvoker)delegate {

                my_recipes_list.Enabled = false;
            });

            while (!isAll)
            {
                if (ControllerForBD.isStartMy)
                {
                    if (ControllerForBD.myRecipes.Count != 0)
                    {
                        isRecipe = true;

                        Recipe r = ControllerForBD.myRecipes.ElementAt(0);

                        var t = createTableForRecipes(r);

                        my_recipes_list.BeginInvoke((MethodInvoker)(() => my_recipes_list.Controls.Add(t)));

                        ControllerForBD.myRecipes.Remove(r);

                    }
                    if ((ControllerForBD.myRecipes.Count == 0) && (ControllerForBD.isDoneMy))
                    {
                        isAll = true;


                        if (!isRecipe)
                        {
                            my_recipes_list.BeginInvoke((MethodInvoker)(() => my_recipes_list.Controls.Add(pbForNoRec())));

                            my_recipes_list.BeginInvoke((MethodInvoker)(() => my_recipes_list.Controls.Add(labelForNoRec())));
                        }
                    }
                }
                else
                {
                    if ((ControllerForBD.isDoneMy))
                    {
                        isAll = true;
                    }
                }
            }

            my_recipes_list.Invoke((MethodInvoker)delegate {

                my_recipes_list.Enabled = true;
            });

        }

        private void favB_Click(object sender, EventArgs e)//Раздел "Избранное"
        {
            checkButtonsColors((int)Buttons.Fav_Rec);

            tabContr.SelectedIndex = (int)Buttons.Fav_Rec;

            if (whatButtonClicked != (int)Buttons.Fav_Rec)
            {
                ControllerForBD.StartSelectAllStarRecipes();

                thread = new Thread(showAllFavRecipes);

                thread.Start();
            }

            whatButtonClicked = (int)Buttons.Fav_Rec;
        }

        public void showAllFavRecipes()//Вывести "Избранные"
        {
            Action action = () => fav_recipes_list.Controls.Clear();

            if (InvokeRequired) { Invoke(action); }

            else { fav_recipes_list.Controls.Clear(); }

            i = counter = 0;

            bool isAll = false;

            isRecipe = false;

            fav_recipes_list.Invoke((MethodInvoker)delegate {

                fav_recipes_list.Enabled = false;
            });

            while (!isAll)
            {
                if (ControllerForBD.isStartStar)
                {
                    if (ControllerForBD.starRecipes.Count != 0)
                    {
                        isRecipe = true;

                        Recipe r = ControllerForBD.starRecipes.ElementAt(0);

                        var t = createTableForRecipes(r);

                        fav_recipes_list.BeginInvoke((MethodInvoker)(() => fav_recipes_list.Controls.Add(t)));

                        ControllerForBD.starRecipes.Remove(r);

                    }
                    if ((ControllerForBD.starRecipes.Count == 0) && (ControllerForBD.isDoneStar))
                    {
                        isAll = true;

                        if (!isRecipe)
                        {
                            fav_recipes_list.BeginInvoke((MethodInvoker)(() => fav_recipes_list.Controls.Add(pbForNoRec())));

                            fav_recipes_list.BeginInvoke((MethodInvoker)(() => fav_recipes_list.Controls.Add(labelForNoRec())));
                        }
                    }
                }
                else
                {
                    if ((ControllerForBD.isDoneStar))
                    {
                        isAll = true;
                    }
                }
            }
            fav_recipes_list.Invoke((MethodInvoker)delegate {

                fav_recipes_list.Enabled = true;
            });
        }

        private void generalB_Click(object sender, EventArgs e)
        {

        }

        private void addRecB_Click(object sender, EventArgs e)//Раздел "Добавление рецепта"
        {
            isPhoto = false;

            cleanAddRecForm();

            if (!RecReadyB.Visible)
            {
                RecReadyB.Show();
            }

            if (!CancelB.Visible)
            {
                CancelB.Show();
            }

            if (updateRecB.Visible)
            {
                updateRecB.Hide();
            }

            if (deleteRecB.Visible)
            {
                deleteRecB.Hide();
            }

            checkButtonsColors((int)Buttons.Add_Rec);

            tabContr.SelectedIndex = (int)Buttons.Add_Rec;

            whatButtonClicked = (int)Buttons.Add_Rec;
        }

        private void settingsB_Click(object sender, EventArgs e)//Раздел "Настройки"
        {
            checkButtonsColors((int)Buttons.Settings);

            tabContr.SelectedIndex = (int)Buttons.Settings;

            whatButtonClicked = (int)Buttons.Settings;
        }

        private void helpB_Click(object sender, EventArgs e)//Раздел "Помощь"
        {
            checkButtonsColors((int)Buttons.Help);

            help_label.Font = new Font(rec_name.Font.FontFamily, rec_name.Font.Size, rec_name.Font.Style);

            help_label.SetBounds(10, helpL.Height, helpL.Width, 500);

            help_label.Text = readHelpFile();

            tabContr.SelectedIndex = (int)Buttons.Help;

            whatButtonClicked = (int)Buttons.Help;
        }

        public string readHelpFile()
        {
            string text;

            if (LanguagesForAddingRecipe.isRu)
            {
                using (StreamReader reader = new StreamReader(Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().Length - 27) + "help_ru.txt"))
                {
                    text = reader.ReadToEnd();
                }
            }
            else
            {
                using (StreamReader reader = new StreamReader(Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().Length - 27) + "help_en.txt"))
                {
                    text = reader.ReadToEnd();
                }
            }
            return text;
        }
        //Обработка событий с оценкой рецепта

        private void pictureBox_MouseLeave(object sender, EventArgs e)
        {
            allStarsOpacityNull();
        }

        private void pictureBox1_Click(object sender, EventArgs e)//Star1
        {
            whatClicked = (int)Star_Marks.Mark1;
            pictureBox1.Image = Image.FromFile(ImageFileNameFull);
            pictureBox2.Image = Image.FromFile(ImageFileNameOpacity);
            pictureBox3.Image = Image.FromFile(ImageFileNameOpacity);
            pictureBox4.Image = Image.FromFile(ImageFileNameOpacity);
            pictureBox5.Image = Image.FromFile(ImageFileNameOpacity);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (whatClicked == (int)Star_Marks.NoMark)
            {
                pictureBox1.Image = Image.FromFile(ImageFileNameFull);
                pictureBox2.Image = Image.FromFile(ImageFileNameFull);
                pictureBox3.Image = Image.FromFile(ImageFileNameOpacity);
                pictureBox4.Image = Image.FromFile(ImageFileNameOpacity);
                pictureBox5.Image = Image.FromFile(ImageFileNameOpacity);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)//Star2
        {
            whatClicked = (int)Star_Marks.Mark2;
            pictureBox1.Image = Image.FromFile(ImageFileNameFull);
            pictureBox2.Image = Image.FromFile(ImageFileNameFull);
            pictureBox3.Image = Image.FromFile(ImageFileNameOpacity);
            pictureBox4.Image = Image.FromFile(ImageFileNameOpacity);
            pictureBox5.Image = Image.FromFile(ImageFileNameOpacity);
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (whatClicked == (int)Star_Marks.NoMark)
            {
                pictureBox1.Image = Image.FromFile(ImageFileNameFull);
                pictureBox2.Image = Image.FromFile(ImageFileNameFull);
                pictureBox3.Image = Image.FromFile(ImageFileNameOpacity);
                pictureBox4.Image = Image.FromFile(ImageFileNameOpacity);
                pictureBox5.Image = Image.FromFile(ImageFileNameOpacity);
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)//Star3
        {
            whatClicked = (int)Star_Marks.Mark3;
            pictureBox1.Image = Image.FromFile(ImageFileNameFull);
            pictureBox2.Image = Image.FromFile(ImageFileNameFull);
            pictureBox3.Image = Image.FromFile(ImageFileNameFull);
            pictureBox4.Image = Image.FromFile(ImageFileNameOpacity);
            pictureBox5.Image = Image.FromFile(ImageFileNameOpacity);
        }

        private void pictureBox3_MouseMove(object sender, MouseEventArgs e)
        {
            if (whatClicked == (int)Star_Marks.NoMark)
            {
                pictureBox1.Image = Image.FromFile(ImageFileNameFull);
                pictureBox2.Image = Image.FromFile(ImageFileNameFull);
                pictureBox3.Image = Image.FromFile(ImageFileNameFull);
                pictureBox4.Image = Image.FromFile(ImageFileNameOpacity);
                pictureBox5.Image = Image.FromFile(ImageFileNameOpacity);
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)//Star4
        {
            whatClicked = (int)Star_Marks.Mark4;
            pictureBox1.Image = Image.FromFile(ImageFileNameFull);
            pictureBox2.Image = Image.FromFile(ImageFileNameFull);
            pictureBox3.Image = Image.FromFile(ImageFileNameFull);
            pictureBox4.Image = Image.FromFile(ImageFileNameFull);
            pictureBox5.Image = Image.FromFile(ImageFileNameOpacity);
        }

        private void pictureBox4_MouseMove(object sender, MouseEventArgs e)
        {
            if (whatClicked == (int)Star_Marks.NoMark)
            {
                pictureBox1.Image = Image.FromFile(ImageFileNameFull);
                pictureBox2.Image = Image.FromFile(ImageFileNameFull);
                pictureBox3.Image = Image.FromFile(ImageFileNameFull);
                pictureBox4.Image = Image.FromFile(ImageFileNameFull);
                pictureBox5.Image = Image.FromFile(ImageFileNameOpacity);
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)//Star5
        {
            whatClicked = (int)Star_Marks.Mark5;
            pictureBox1.Image = Image.FromFile(ImageFileNameFull);
            pictureBox2.Image = Image.FromFile(ImageFileNameFull);
            pictureBox3.Image = Image.FromFile(ImageFileNameFull);
            pictureBox4.Image = Image.FromFile(ImageFileNameFull);
            pictureBox5.Image = Image.FromFile(ImageFileNameFull);
        }

        private void pictureBox5_MouseMove(object sender, MouseEventArgs e)
        {
            if (whatClicked == (int)Star_Marks.NoMark)
            {
                pictureBox1.Image = Image.FromFile(ImageFileNameFull);
                pictureBox2.Image = Image.FromFile(ImageFileNameFull);
                pictureBox3.Image = Image.FromFile(ImageFileNameFull);
                pictureBox4.Image = Image.FromFile(ImageFileNameFull);
                pictureBox5.Image = Image.FromFile(ImageFileNameFull);
            }
        }

        private void RecReadyB_Click(object sender, EventArgs e)//Добавление рецепта в таблицу "Мои рецепты"
        {
           checkRecForm();

            if (ControllerForBD.InsertToMyRecipes(rec_name.Text, CategoryCB.Text, Ingr_rec.Text, Instr_rec.Text, whatClicked.ToString(), markDif.Text, time_rec.Text, isPhoto ? Instruments.convertImageIntoB(this.RecPhoto.Image) : null))
            {
                MessageBox.Show("Рецепт успешно добавлен.", "Добавление рецепта", MessageBoxButtons.OK, MessageBoxIcon.Information);

               // cleanAddRecForm();
            }
            else
            {
                MessageBox.Show("Что-то пошло не так.", "Добавление рецепта");
            }
        }

        

        public void checkRecForm()
        {
            if (rec_name.Text == String.Empty)
            {
                MessageBox.Show(LanguagesForAddingRecipe.isRu ? "Вы не ввели название рецепта." : "You have not entered a name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            if (Instr_rec.Text == String.Empty)
            {
                MessageBox.Show(LanguagesForAddingRecipe.isRu ? "Вы не ввели инструкцию к рецепту." : "You have not entered an instruction.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            if (Ingr_rec.Text == String.Empty)
            {
                MessageBox.Show(LanguagesForAddingRecipe.isRu ? "Вы не ввели ингредиенты для рецепта." : "You have not entered ingredients.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            time_rec.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;

            if (String.IsNullOrEmpty(time_rec.Text) || String.IsNullOrWhiteSpace(time_rec.Text) || time_rec.Text.Length != 6)
            {
                MessageBox.Show(LanguagesForAddingRecipe.isRu ? "Вы не ввели время или ввели некорректно." : "You have not entered time or entered incorrectly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            if (int.Parse(time_rec.Text[2].ToString() + time_rec.Text[3].ToString()) >= 60 || int.Parse(time_rec.Text[4].ToString() + time_rec.Text[5].ToString()) >= 60 || int.Parse(time_rec.Text[0].ToString() + time_rec.Text[1].ToString()) >= 24)
            {
                MessageBox.Show(LanguagesForAddingRecipe.isRu ? "Вы ввели время некорректно." : "You have  entered time incorrectly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            if (whatClicked == 0)
            {
                MessageBox.Show(LanguagesForAddingRecipe.isRu ? "Оценка рецепта не задана." : "Recipe's rating is not defined.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
        }

        private void LangCB_SelectedIndexChanged(object sender, EventArgs e)//Смена языка в приложении
        {
            
        }

        public void languageChanges()//Смена языка в приложении
        {   
            CategoryAndFilterInit();

            AddLabel.Text = LanguagesForAddingRecipe.isRu ? "Добавить рецепт" : "Add a recipe";

            startLabel.Text = LanguagesForAddingRecipe.isRu ? "Храните все рецепты в одном месте" : "Store all recipes in one place";

            myRecB.Text = LanguagesForAddingRecipe.isRu ? "Мои рецепты" : "My recipes";

            favB.Text = LanguagesForAddingRecipe.isRu ? "Избранные" : "Favourite";

            generalB.Text = LanguagesForAddingRecipe.isRu ? "Общие рецепты" : "General recipes";

            addRecB.Text = LanguagesForAddingRecipe.isRu ? "Добавить рецепт" : "Add a recipe";

            settingsB.Text = LanguagesForAddingRecipe.isRu ? "Настройки" : "Settings";

            helpB.Text = LanguagesForAddingRecipe.isRu ? "Помощь" : "Help";

            TitleL.Text = LanguagesForAddingRecipe.isRu ? "Название" : "Title";

            RateLable.Text = difl1.Text = LanguagesForAddingRecipe.isRu ? "Оценка рецепта" : "Prescription evaluation";

            PhotoLab.Text = LanguagesForAddingRecipe.isRu ? "Фото блюда" : "Dish photo";

            CategoryL.Text = catl.Text = LanguagesForAddingRecipe.isRu ? "Категория" : "Category";

            IngredL.Text = LanguagesForAddingRecipe.isRu ? "Ингредиенты" : "Ingredients";

            TimeL.Text = LanguagesForAddingRecipe.isRu ? "Время приготовления(ч:м:с)" : "Cooking time(h:m:s)";

            genL.Text = LanguagesForAddingRecipe.isRu ? "Общие рецепты" : "General recipes";

            myL.Text = LanguagesForAddingRecipe.isRu ? "Мои рецепты" : "My recipes";

            favL.Text = LanguagesForAddingRecipe.isRu ? "Избранные" : "Favourite";

            helpL.Text = LanguagesForAddingRecipe.isRu ? "Помощь" : "Help";

            ChangeLLabel.Text = LanguagesForAddingRecipe.isRu ? "Язык" : "Language";

            SettingsL.Text = LanguagesForAddingRecipe.isRu ? "Настройки" : "Settings";

            CancelB.Text = LanguagesForAddingRecipe.isRu ? "Очистить" : "Clean";

            RecReadyB.Text = LanguagesForAddingRecipe.isRu ? "Добавить" : "Add";

            button1.Text = LanguagesForAddingRecipe.isRu ? "Выгрузить \"Мои рецепты\" в файл" : "Load \"My recipes\" into file";

            searchB.Text = LanguagesForAddingRecipe.isRu ? "Поиск" : "Search";

            deleteRecB.Text = LanguagesForAddingRecipe.isRu ? "Удалить" : "Delete";

            updateRecB.Text = LanguagesForAddingRecipe.isRu ? "Редактировать" :"Update";

            InstrL.Text = LanguagesForAddingRecipe.isRu ? "Инструкция" : "Instruction";

            DiffL.Text = difl.Text = LanguagesForAddingRecipe.isRu ? "Оценка сложности рецепта" : "Recipe Difficulty Score";

            searchL.Text = LanguagesForAddingRecipe.isRu ? "Результат поиска" : "Result of search";
        }

        private void CancelB_Click(object sender, EventArgs e)//Очистка формы рецепта
        {
            cleanAddRecForm();
        }

        private void cleanAddRecForm()
        {
            rec_name.Clear();

            markDif.SelectedIndex = 0;

            time_rec.Clear();

            CategoryCB.SelectedIndex = 0;

            Ingr_rec.Clear();

            Instr_rec.Clear();

            whatClicked = (int)Star_Marks.NoMark;

            allStarsOpacityNull();

            RecPhoto.Image = Image.FromFile(ImageAddRec);//SizeMode??????
        }

        private void allStarsOpacityNull()//Сделать все звёзды прозрачными
        {
            if (whatClicked == (int)Star_Marks.NoMark)
            {
                pictureBox1.Image = Image.FromFile(ImageFileNameOpacity);
                pictureBox2.Image = Image.FromFile(ImageFileNameOpacity);
                pictureBox3.Image = Image.FromFile(ImageFileNameOpacity);
                pictureBox4.Image = Image.FromFile(ImageFileNameOpacity);
                pictureBox5.Image = Image.FromFile(ImageFileNameOpacity);
            }
        }

        private void RecPhoto_Click(object sender, EventArgs e)//Добавление фото в рецепт
        {

        }

        private void MainForm_SizeChanged(object sender, EventArgs e)//Изменение размеров элементов при изменении размеров формы
        {

        }


        public void formChanges(int x, int y)//Изменения размеров элементов формы
        {
            
        }

        public void setColors()
        {
           
        }

        public void CategoryAndFilterInit()//Инициализация категорий и панели фильтра в соответствии с языком
        {
            if (CategoryCB.Items.Count != 0)
            {
                CategoryCB.Items.Clear();
            }

            if (rateCheckB.Items.Count != 0)
            {
                rateCheckB.Items.Clear();
            }

            if (diffCheckB.Items.Count != 0)
            {
                diffCheckB.Items.Clear();
            }

            if (categoryCheckB.Items.Count != 0)
            {
                categoryCheckB.Items.Clear();
            }

            if (LanguagesForAddingRecipe.isRu)
            {
                foreach (var item in LanguagesForAddingRecipe.categoriesRu)
                {
                    CategoryCB.Items.Add(item);

                    categoryCheckB.Items.Add(item);
                }
            }
            else
            {
                foreach (var item in LanguagesForAddingRecipe.categoriesEn)
                {
                    CategoryCB.Items.Add(item);

                    categoryCheckB.Items.Add(item);
                }
            }
            for (int i = 1; i < 6; i++)
            {
                diffCheckB.Items.Add(i);

                if (i == 1)
                {
                    rateCheckB.Items.Add(i + (LanguagesForAddingRecipe.isRu ? " звезда" : " star"));

                    continue;
                }

                if (i == 5)
                {
                    rateCheckB.Items.Add(i + (LanguagesForAddingRecipe.isRu ? " звезда" : " stars"));

                    continue;
                }

                rateCheckB.Items.Add(i + (LanguagesForAddingRecipe.isRu ? " звезда" : " stars"));
            }
        }
        
        public void checkButtonsColors(int num)//Функция для проверки активности кнопок
        {
            if (num == (int)Buttons.My_Rec)
            {
                if (myRecB.BackColor != Instruments.myButtonHighlightColor) { myRecB.BackColor = Instruments.myButtonHighlightColor; }
            }
            else
            {
                myRecB.BackColor = Color.Transparent;
            }

            if (num == (int)Buttons.Fav_Rec)
            {
                if (favB.BackColor != Instruments.myButtonHighlightColor) { favB.BackColor = Instruments.myButtonHighlightColor; }
            }
            else
            {
                favB.BackColor = Color.Transparent;
            }

            if (num == (int)Buttons.General_Rec)
            {
                if (generalB.BackColor != Instruments.myButtonHighlightColor) { generalB.BackColor = Instruments.myButtonHighlightColor; }
            }
            else
            {
                generalB.BackColor = Color.Transparent;
            }

            if (num == (int)Buttons.Add_Rec)
            {
                if (addRecB.BackColor != Instruments.myButtonHighlightColor) { addRecB.BackColor = Instruments.myButtonHighlightColor; }
            }
            else
            {
                addRecB.BackColor = Color.Transparent;
            }

            if (num == (int)Buttons.Settings)
            {
                if (settingsB.BackColor != Instruments.myButtonHighlightColor) { settingsB.BackColor = Instruments.myButtonHighlightColor; }
            }
            else
            {
                settingsB.BackColor = Color.Transparent;
            }

            if (num == (int)(int)Buttons.Help)
            {
                if (helpB.BackColor != Instruments.myButtonHighlightColor) { helpB.BackColor = Instruments.myButtonHighlightColor; }
            }
            else
            {
                helpB.BackColor = Color.Transparent;
            }
        }

        Label labelForNoRec()
        {
            Label l = new Label();

            l.Font = new Font(startLabel.Font.FontFamily, 30, startLabel.Font.Style);

            //l.Text = LanguagesForAddingRecipe.isRu ? LanguagesForAddingRecipe.haveSomeRecRu : LanguagesForAddingRecipe.haveSomeRecEn;

            l.TextAlign = ContentAlignment.MiddleCenter;

            l.SetBounds(0, general_recipes_list.Height - 400, general_recipes_list.Width - 50, 300);

            return l;
        }

        PictureBox pbForNoRec()
        {
            PictureBox pb = new PictureBox();

            pb.SizeMode = PictureBoxSizeMode.Zoom;//??????????????????

            pb.SetBounds(general_recipes_list.Width / 2 - 160, 100, 256, 256);

            pb.BackgroundImage = Image.FromFile(Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().Length - 27) + "images\\em.png");

            return pb;
        }


        public static Image image= Image.FromFile(Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().Length - 27) + "images\\standart_photo.jpg");

        public TableLayoutPanel createTableForRecipes(Recipe r)//Создание рецепта для отображения
        {
            TableLayoutPanel t = new TableLayoutPanel();         
            return t;
        }
        
        public void changeFavourite(Recipe r)
        {
            if (r.Star)
            {
                ControllerForBD.setStar(r.Id, false);
            }
            else
            {
                ControllerForBD.setStar(r.Id, true);
            }
        }

        public void fullRecipe(int id, int whatBu)//Заполнение рецепта при нажатии
        {
        }

        private void deleteRecB_Click(object sender, EventArgs e)
        {
            if (main_recipe != null)
            {
                ControllerForBD.deleteById(main_recipe.Id);//ДЛЯ РЕЦЕПТОВ ИЗ ИНТЕРНЕТА ТАКОЕ НЕ ДЕЛАЕМ
                tabContr.SelectedIndex = (int)Buttons.Start_Page;
                whatButtonClicked = -1;
                checkButtonsColors(-1);
            }
        }

        private void updateRecB_Click(object sender, EventArgs e)//Редактировать рецепт
        {
            if (whatClicked == 0)
            {
                whatClicked = int.Parse(main_recipe.Marklike);
            }

            checkRecForm();

            ControllerForBD.editRecipe(main_recipe.Id,rec_name.Text, CategoryCB.Text, Ingr_rec.Text, Instr_rec.Text, whatClicked.ToString(), markDif.Text, time_rec.Text, isPhoto ? Instruments.convertImageIntoB(this.RecPhoto.Image) : null);

            MessageBox.Show("Рецепт успешно отредактирован.","Оповещение");

            if (isMy)
            {
                tabContr.SelectedIndex = (int)Buttons.My_Rec;

                ControllerForBD.StartSelectAllMyRecipes();

                thread = new Thread(showAllMyRecipes);

                thread.Start();
            }
            if (!isMy)
            {
                tabContr.SelectedIndex = (int)Buttons.Fav_Rec;

                ControllerForBD.StartSelectAllStarRecipes();

                thread = new Thread(showAllFavRecipes);

                thread.Start();
            }
        }

        private void searchB_Click(object sender, EventArgs e)
        {

        }

        private void FilterB_Click(object sender, EventArgs e)//Повторное нажатие?
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isCollapsed)
            {
                FilterB.Image = Image.FromFile(Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().Length - 27) + "images\\collapse.png");
                filterPanel.Height += 30;
                if (filterPanel.Size == filterPanel.MaximumSize)
                {
                    timer1.Stop();
                    isCollapsed = false;
                }
            }
            else
            {
                FilterB.Image = Image.FromFile(Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().Length - 27) + "images\\expand.png");
                filterPanel.Height -= 30;
                if (filterPanel.Size == filterPanel.MinimumSize)
                {
                    timer1.Stop();
                    isCollapsed = true;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Загрузка начата.");

            ControllerForBD.StartSelectAllFileRecipes();

            thread = new Thread(loadAllRecipesIntoFile);

            thread.Start();
        }

        private void btnChangePos()
        {
            int x = 0;
            int y = 0;

            helpB.Location = settingsB.Location;
            settingsB.Location = addRecB.Location;
            addRecB.Location = generalB.Location;
            
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }
    }
    
}
