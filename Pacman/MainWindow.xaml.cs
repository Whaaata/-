﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading; // для использования времени работы деспетчера

namespace Pacman
{
    public partial class MainWindow : Window
    {

        DispatcherTimer gameTimer = new DispatcherTimer(); // создаем новый диспетчерский таймер

        bool goLeft, goRight, goDown, goUp; // движение по 4 направлениям 
        bool noLeft, noRight, noDown, noUp; // остановка движения по 4 направлениям

        int speed = 5; // скорость пакмена

        Rect pacmanHitBox; // поле попадания игрока (для проверки столкновений с призраками, стенами и сбора монеток)

        int ghostSpeed = 10; // скорость призраков
        int ghostMoveStep = 160; // ограничение передвижения призраков
        int currentGhostStep; // текущее ограничение
        int score = 0; // счетчик собранных монет



        public MainWindow()
        {
            InitializeComponent();

            GameSetUp(); // запуск настройки игры
        }


        private void CanvasKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Left && noLeft == false)
            {
                // если нажата клавиша "влево" и нет ограничения на движение влево
                goRight = goUp = goDown = false; // все передвижения по остальным направлениям falsе
                noRight = noUp = noDown = false; // все ограничения по остальным направлениям falsе

                goLeft = true; // устанавливаеи движение влево

                pacman.RenderTransform = new RotateTransform(90, pacman.Width /2, pacman.Height / 2); // поворот картинки по направлению движения
            }

            if (e.Key == Key.Right && noRight == false)
            {
                // если нажата клавиша "вправо" и нет ограничения на движение вправо
                noLeft = noUp = noDown = false; // все передвижения по остальным направлениям falsе
                goLeft = goUp = goDown = false; // все ограничения по остальным направлениям falsе

                goRight = true; // устанавливаеи движение вправо

                pacman.RenderTransform = new RotateTransform(-90, pacman.Width / 2, pacman.Height / 2); // поворот картинки по направлению движения
            }

            if (e.Key == Key.Up && noUp == false)
            {
                // если нажата клавиша "вверх" и нет ограничения на движение вверх
                noRight = noDown = noLeft = false; // все передвижения по остальным направлениям falsе
                goRight = goDown = goLeft = false; // все ограничения по остальным направлениям falsе

                goUp = true; // устанавливаеи движение вверх

                pacman.RenderTransform = new RotateTransform(180, pacman.Width / 2, pacman.Height / 2); // поворот картинки по направлению движения
            }

            if (e.Key == Key.Down && noDown == false)
            {
                // если нажата клавиша "вниз" и нет ограничения на движение вниз
                noUp = noLeft = noRight = false; // все передвижения по остальным направлениям falsе
                goUp = goLeft = goRight = false; // все ограничения по остальным направлениям falsе

                goDown = true; // устанавливаеи движение вниз

                pacman.RenderTransform = new RotateTransform(0, pacman.Width / 2, pacman.Height / 2); // поворот картинки по направлению движения
            }


        }

        private void GameSetUp()
        {
            // запуск функции при загрузке программы

            FirstLevel.Focus(); // устанавливаем фокус программы на окно

            gameTimer.Tick += GameLoop; // связь игры со временем
            gameTimer.Interval = TimeSpan.FromMilliseconds(20); // устанавливаем метку на каждые 20 миллисекунд
            gameTimer.Start(); // начало отсчета времени
            currentGhostStep = ghostMoveStep; // устанавливаем текущий шаг призрака на шаг перемещения

            // импорт изображений
            ImageBrush pacmanImage = new ImageBrush();
            pacmanImage.ImageSource = new BitmapImage(new Uri("C:/Users/Pavel/Desktop/УЧЕБА/ЭСКиТП/Pacman/Pacman/images/mouse.jpg"));
            pacman.Fill = pacmanImage;

            ImageBrush redGhost = new ImageBrush();
            redGhost.ImageSource = new BitmapImage(new Uri("C:/Users/Pavel/Desktop/УЧЕБА/ЭСКиТП/Pacman/Pacman/images/cat1.jpg"));
            redghost.Fill = redGhost;

            ImageBrush orangeGhost = new ImageBrush();
            orangeGhost.ImageSource = new BitmapImage(new Uri("C:/Users/Pavel/Desktop/УЧЕБА/ЭСКиТП/Pacman/Pacman/images/cat2.jpg"));
            orangeghost.Fill = orangeGhost;

            ImageBrush pinkGhost = new ImageBrush();
            pinkGhost.ImageSource = new BitmapImage(new Uri("C:/Users/Pavel/Desktop/УЧЕБА/ЭСКиТП/Pacman/Pacman/images/cat3.jpg"));
            pinkghost.Fill = pinkGhost;
        }

        private void GameLoop(object sender, EventArgs e)
        {

            // тут начинается контроль всех движений, столкновений, счета в игре и ее итога

            Score.Content = "Score: " + score; // вывод счета на экране 

            // пакмен начал движение

            if (goRight)
            {
                // перемещаем пакмена вправо на его скорость 
                Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) + speed);
            }
            if (goLeft)
            {
                // перемещаем пакмена влево на его скорость
                Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - speed);
            }
            if (goUp)
            {
                // перемещаем пакмена вверх на его скорость
                Canvas.SetTop(pacman, Canvas.GetTop(pacman) - speed);
            }
            if (goDown)
            {
                // перемещаем пакмена вниз на его скорость
                Canvas.SetTop(pacman, Canvas.GetTop(pacman) + speed);
            }
            // заканчиваем движение 


            // ограничения передвижения
            if (goDown && Canvas.GetTop(pacman) + 80 > Application.Current.MainWindow.Height)
            {
                // если пакмен выходит за нижнюю границу окра 
                noDown = true;
                goDown = false;
            }
            if (goUp && Canvas.GetTop(pacman) < 1)
            {
                //  если пакмен выходит за верхнюю границу окра 
                noUp = true;
                goUp = false;
            }
            if (goLeft && Canvas.GetLeft(pacman) - 10 < 1)
            {
                //  если пакмен выходит за левую границу окра 
                noLeft = true;
                goLeft = false;
            }
            if (goRight && Canvas.GetLeft(pacman) + 70 > Application.Current.MainWindow.Width)
            {
                //  если пакмен выходит за правую границу окра 
                goRight = false;
            }

            pacmanHitBox = new Rect(Canvas.GetLeft(pacman), Canvas.GetTop(pacman), pacman.Width, pacman.Height); // вписываем пакмена в прямоугольник для учета столкновений

            // отслеживание столкновений
            foreach (var x in FirstLevel.Children.OfType<Rectangle>())
            {
                // все прямоугольники идентифицируем как х


                Rect hitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height); // создаем hitbox для всех прямоугольников

                // ищем стены
                if ((string)x.Tag == "wall")
                {
                    // проверка столкновения пакмена со стеной при движении влево
                    if (goLeft == true && pacmanHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) + 5);
                        noLeft = true;
                        goLeft = false;
                    }
                    // проверка столкновения пакмена со стеной при движении вправо
                    if (goRight == true && pacmanHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - 5);
                        noRight = true;
                        goRight = false;
                    }
                    // проверка столкновения пакмена со стеной при движении вниз
                    if (goDown == true && pacmanHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetTop(pacman, Canvas.GetTop(pacman) - 5);
                        noDown = true;
                        goDown = false;
                    }
                    // проверка столкновения пакмена со стеной при движении вверх
                    if (goUp == true && pacmanHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetTop(pacman, Canvas.GetTop(pacman) + 5);
                        noUp = true;
                        goUp = false;
                    }

                }

                // ищем монетки
                if ((string) x.Tag == "coin")
                {
                    if (pacmanHitBox.IntersectsWith(hitBox) && x.Visibility == Visibility.Visible)
                    {
                        // при столкновении пакмена с монеткой делаем ее невидимой
                        x.Visibility = Visibility.Hidden;
                        // повышаем счет
                        score++;
                    }

                }

                // ищем призраков
                if ((string) x.Tag == "ghost")
                {
                    if (pacmanHitBox.IntersectsWith(hitBox))
                    {
                       // если пакмен столкнулся с призраком, выводим сообщение
                       GameOver("Вы проиграли - призраки поймали вас! Хотите начать новую игру?");
                    }

                    if (x.Name.ToString() == "orangeghost") // если есть прямоугольник оранжевый призрак
                    {
                        // описание движения оранжевого призрака
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - ghostSpeed / 5);
                        Canvas.SetTop(x, Canvas.GetTop(x) - ghostSpeed / 2);

                    }
                    else if (x.Name.ToString() == "redghost") // если есть прямоугольник красный призрак
                    {
                        // описание движения красного призрака
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - ghostSpeed / 5);
                        Canvas.SetTop(x, Canvas.GetTop(x) + ghostSpeed / 2);

                    }
                    else
                    {
                        // описание движения розового призрака
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + ghostSpeed);

                    }

                    // уменьшение текущего призрачного шага
                    currentGhostStep--;

                    // если текущий призрачный шаг становится меньше 1
                    if (currentGhostStep < 1)
                    {
                        // сброс текущего шага перемещения призрака до значения шага перемещения призрака
                        currentGhostStep = ghostMoveStep;
                        // обратная скорость движения
                        ghostSpeed = -ghostSpeed;

                    }
                }
            }


            // если все монетки собраны
            if (score == 26)
            {
                // выводим сообщение о победе
                GameOver("Вы победили и собрали все монетки! Хотите начать новую игру?");
            }
        }

        private void GameOver(string message)
        {
            gameTimer.Stop(); // остановка игрового таймера
            // вывод окна с сообщением

            if (MessageBox.Show(message, "Pacman Game", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            // когда игрок нажимает кнопку "ок", игра перезапускается
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            }
            Application.Current.Shutdown();
        }
    }
}