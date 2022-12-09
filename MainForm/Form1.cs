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

        private void favB_Click(object sender, EventArgs e)//Раздел "Избранное"
        {

        }

        private void generalB_Click(object sender, EventArgs e)
        {

        }

        private void addRecB_Click(object sender, EventArgs e)//Раздел "Добавление рецепта"
        {
            
        }

        private void settingsB_Click(object sender, EventArgs e)//Раздел "Настройки"
        {
            
        }

        private void helpB_Click(object sender, EventArgs e)//Раздел "Помощь"
        {
            
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
            pictureBox1.Image = Image.FromFile(ImageFileNameFull);
            pictureBox2.Image = Image.FromFile(ImageFileNameFull);
            pictureBox3.Image = Image.FromFile(ImageFileNameOpacity);
            pictureBox4.Image = Image.FromFile(ImageFileNameOpacity);
            pictureBox5.Image = Image.FromFile(ImageFileNameOpacity);
        }

        private void pictureBox2_Click(object sender, EventArgs e)//Star2
        {
            
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void pictureBox3_Click(object sender, EventArgs e)//Star3
        {
            
        }

        private void pictureBox3_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void pictureBox4_Click(object sender, EventArgs e)//Star4
        {
            
        }

        private void pictureBox4_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void pictureBox5_Click(object sender, EventArgs e)//Star5
        {
            
        }

        private void pictureBox5_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void RecReadyB_Click(object sender, EventArgs e)//Добавление рецепта в таблицу "Мои рецепты"
        {
           
        }

        

        public void checkRecForm()
        {
            if (rec_name.Text == String.Empty)
            {

                return;
            }

            if (Instr_rec.Text == String.Empty)
            {

                return;
            }

            if (Ingr_rec.Text == String.Empty)
            {

                return;
            }

            time_rec.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;

            if (String.IsNullOrEmpty(time_rec.Text) || String.IsNullOrWhiteSpace(time_rec.Text) || time_rec.Text.Length != 6)
            {
                return;
            }

            if (int.Parse(time_rec.Text[2].ToString() + time_rec.Text[3].ToString()) >= 60 || int.Parse(time_rec.Text[4].ToString() + time_rec.Text[5].ToString()) >= 60 || int.Parse(time_rec.Text[0].ToString() + time_rec.Text[1].ToString()) >= 24)
            {
                return;
            }

            if (whatClicked == 0)
            {
                return;
            }

        }

        private void LangCB_SelectedIndexChanged(object sender, EventArgs e)//Смена языка в приложении
        {
            
        }

        public void languageChanges()//Смена языка в приложении
        {   
            
        }

        private void CancelB_Click(object sender, EventArgs e)//Очистка формы рецепта
        {
        }

        private void cleanAddRecForm()
        {
        }

        private void allStarsOpacityNull()//Сделать все звёзды прозрачными
        {
            
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
        
        
        public void checkButtonsColors(int num)//Функция для проверки активности кнопок
        {
            
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

        }

        public void fullRecipe(int id, int whatBu)//Заполнение рецепта при нажатии
        {
        }

        private void deleteRecB_Click(object sender, EventArgs e)
        {

        }

        private void updateRecB_Click(object sender, EventArgs e)//Редактировать рецепт
        {
            
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
