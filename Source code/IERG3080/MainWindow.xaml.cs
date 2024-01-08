using System;
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
using System.Windows.Threading;
using static IERG3080.MainWindow;

namespace IERG3080
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer GameTimer = new DispatcherTimer();
        DispatcherTimer EnemySpawnTimer = new DispatcherTimer();
        Random rnd = new Random();

        Ellipse POrbHitbox = new Ellipse();
        public MainWindow()
        {
            InitializeComponent();

            GameTimer.Interval = (TimeSpan.FromMilliseconds(10));
            GameTimer.Tick += GameLoop;
            GameTimer.Start();
            Universe();
            PlayerSpawn();
            SunSpawn();
            MakeOrbs();

            //ZoomViewbox.Width = 100;
            //ZoomViewbox.Height = 100;

        }
        private void GameLoop(object sender, EventArgs e)
        {
            double playerRadius = 0;
            //POrbHitbox = new Ellipse(Canvas.GetLeft(POrb), Canvas.GetTop(POrb), POrb.Width, POrb.Height);
            foreach (Orb orb in orbs)
            {
                if (orb.type == OrdType.Player)
                {
                    //orb.radius--; testing
                    playerRadius = orb.radius;

                    break;
                }
            }

            foreach (Orb orb in orbs)
            {
                if (orb.type == OrdType.Enemy)
                {
                    if (orb.radius < playerRadius)
                    {
                        SolidColorBrush blueBrush = new SolidColorBrush();
                        blueBrush.Color = Colors.Purple;
                        orb.ellipse.Fill = blueBrush;
                    }
                    else if (orb.radius > playerRadius)
                    {
                        SolidColorBrush RedBrush = new SolidColorBrush();
                        RedBrush.Color = Colors.Red;
                        orb.ellipse.Fill = RedBrush;
                    }
                }
            }
            Centripetal();
            CollisionDetect();
        }

        
        public enum OrdType
        {
            Player,
            Enemy,
            Sun
        }

        public struct Coordinate
        {
            public double x;
            public double y;
        };

        public class Orb
        {
            public double radius;

            public Coordinate coor;
            public OrdType type;
            public Ellipse ellipse { get; set; }
        }
        List<Orb> orbs = new List<Orb>();

   
        private double OrbDistance(Orb orb1, Orb orb2)
        {
            return Math.Sqrt(Math.Pow(orb1.coor.x - orb2.coor.x, 2) + Math.Pow(orb1.coor.y - orb2.coor.y, 2));
        }
        private void UpdatePosition(Orb orb)
        {
            Canvas.SetTop(orb.ellipse, orb.coor.x - orb.radius / 2);
            Canvas.SetLeft(orb.ellipse, orb.coor.y - orb.radius / 2);
        }

        private void Universe()
        {
            SolidColorBrush Black = new SolidColorBrush();
            Black.Color = Colors.Black;
            SolidColorBrush blueBrush = new SolidColorBrush();
            blueBrush.Color = Colors.Blue;
            Orb Universe = new Orb();
            Universe.type = OrdType.Player;
            Universe.radius = 2000;
            Universe.coor.x = 800;
            Universe.coor.y = 800;
            Universe.ellipse = new Ellipse()
            {
                Width = Universe.radius,
                Height = Universe.radius,
                Stroke = blueBrush,
                StrokeThickness = 4,
                Fill = Black,
            };
            Canvas.SetTop(Universe.ellipse, Universe.coor.x - Universe.radius /2);
            Canvas.SetLeft(Universe.ellipse, Universe.coor.y - Universe.radius /2);
            MyCanvas.Children.Add(Universe.ellipse);
        }
        private void CollisionDetect()
        {
            int i, j; Orb orb1, orb2;
            for (i = 0; i < orbs.Count - 1; i++)
            {
                orb1 = orbs[i];
                for (j = i; j < orbs.Count; j++)
                {
                    orb2 = orbs[j];
                    if (OrbDistance(orb1, orb2) < orb1.radius / 2 + orb2.radius / 2)
                    {
                        if (orb1.radius > orb2.radius)
                        {
                            orb1.radius += 0.4;
                            orb2.radius -= 0.4;
                            orb1.ellipse.Width = orb1.radius;
                            orb1.ellipse.Height = orb1.radius;
                            UpdatePosition(orb1 );
                            if (orb2.radius <= 0)
                            {
                                orbs.RemoveAt(j);
                                MyCanvas.Children.Remove(orb2.ellipse);
                                j -= 1;
                            }
                            else
                            {
                                orb2.ellipse.Width = orb2.radius;
                                orb2.ellipse.Height = orb2.radius;
                            }
                        }
                        else if (orb2.radius > orb1.radius)
                        {
                            orb2.radius += 0.4;
                            orb1.radius -= 0.4;
                            orb2.ellipse.Width = orb2.radius;
                            orb2.ellipse.Height = orb2.radius;
                            UpdatePosition(orb2);
                            if (orb1.radius <= 0)
                            {
                                orbs.RemoveAt(i);
                                MyCanvas.Children.Remove(orb1.ellipse);
                                i -= 1;
                                break;
                            }
                            orb1.ellipse.Width = orb1.radius;
                            orb1.ellipse.Height = orb1.radius;
                        }
                    }
                }
            }
        }
        private void PlayerSpawn()
        {
            SolidColorBrush blueBrush = new SolidColorBrush();
            blueBrush.Color = Colors.Blue;
            Orb player = new Orb();
            player.type = OrdType.Player;
            player.radius = 18.7;
            /*do
            {
                player.coor.x = rnd.Next(60, 1500);
                player.coor.y = rnd.Next(60, 1500);
            } while ((player.coor.x > 700 || player.coor.x < 1000) && (player.coor.y > 700 || player.coor.y < 1000));*/
            player.coor.x = rnd.Next(60, 1500);
            player.coor.y = rnd.Next(60, 1500);
            player.ellipse = new Ellipse()
            {
                Width = player.radius,
                Height = player.radius,
                Fill = blueBrush,
            };
            orbs.Add(player);
            Canvas.SetTop(player.ellipse, player.coor.x - player.radius / 2);
            Canvas.SetLeft(player.ellipse, player.coor.y - player.radius / 2);
            MyCanvas.Children.Add(player.ellipse);
        }
        private void SunSpawn()
        {

            Orb sun = new Orb();
            sun.type = OrdType.Sun;
            sun.radius = 120;
            sun.coor.x = 800;
            sun.coor.y = 800;
            ImageBrush sunImage = new ImageBrush();
            sunImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/sun.jpg"));
            sun.ellipse = new Ellipse()
            {
                Width = sun.radius,
                Height = sun.radius,
                Fill = sunImage,
            };
            orbs.Add(sun);
            Canvas.SetTop(sun.ellipse, sun.coor.x - sun.radius / 2);
            Canvas.SetLeft(sun.ellipse, sun.coor.y - sun.radius / 2);
            MyCanvas.Children.Add(sun.ellipse);
        }
        private void MakeOrbs()
        {
            SolidColorBrush redBrush = new SolidColorBrush();
            redBrush.Color = Colors.Red;

            int orbCounter = rnd.Next(50,100);

            for (int i = 0; i < orbCounter; i++)
            {
                Orb enemy = new Orb();
                enemy.type = OrdType.Enemy;
                if (i < orbCounter*2 / 3)
                {
                    enemy.radius = 16- rnd.NextDouble() * 12;// 4~16
                }
                else
                {
                    enemy.radius = 30 - rnd.NextDouble() * 16;//24~40
                }
                switch(i%4)
                {
                    case 0:
                        enemy.coor.x = rnd.Next(60, 740); //top-left
                        enemy.coor.y = rnd.Next(60, 740);
                        break;
                    case 1:
                        enemy.coor.x = rnd.Next(60, 740); //top-right
                        enemy.coor.y = rnd.Next(740, 1540);
                        break;
                    case 2:
                        enemy.coor.x = rnd.Next(740, 1540); //bottom-left
                        enemy.coor.y = rnd.Next(60, 740);
                        break;

                    case 3:
                        enemy.coor.x = rnd.Next(740, 1540); //bottom-right
                        enemy.coor.y = rnd.Next(740, 1540);
                        break;
                }
               
                enemy.ellipse = new Ellipse()
                {
                    Width = enemy.radius,
                    Height = enemy.radius,
                    Fill = redBrush,
                };
                orbs.Add(enemy);
                Canvas.SetTop(enemy.ellipse, enemy.coor.x - enemy.radius / 2);
                Canvas.SetLeft(enemy.ellipse, enemy.coor.y - enemy.radius / 2);
                MyCanvas.Children.Add(enemy.ellipse);
            }
        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var element = (UIElement)sender;
            var position = e.GetPosition(element);
            var transform = (MatrixTransform)element.RenderTransform;
            var matrix = transform.Matrix;
            var scale = (e.Delta >= 0 ? 1.1 : (1.0 / 1.1)); // choose appropriate scaling factor

            matrix.ScaleAtPrepend(scale, scale, position.X, position.Y);
            transform.Matrix = matrix;
        }

        private void Centripetal()
        {
            // Find the player and sun orbs
            Orb player = orbs.FirstOrDefault(orb => orb.type == OrdType.Player);
            Orb sun = orbs.FirstOrDefault(orb => orb.type == OrdType.Sun);

            if (player != null && sun != null)
            {
                // Calculate the distance between the player and sun
                double playerDistance = Math.Sqrt(Math.Pow(player.coor.x - sun.coor.x, 2) + Math.Pow(player.coor.y - sun.coor.y, 2));

                // Calculate the angle between the player and sun
                double playerAngle = Math.Atan2(player.coor.y - sun.coor.y, player.coor.x - sun.coor.x);

                foreach (Orb orb in orbs.Where(orb => orb.type == OrdType.Enemy))
                {
                    // Calculate the distance between the orb and sun
                    double orbDistance = Math.Sqrt(Math.Pow(orb.coor.x - sun.coor.x, 2) + Math.Pow(orb.coor.y - sun.coor.y, 2));

                    // Calculate the angle between the orb and sun
                    double orbAngle = Math.Atan2(orb.coor.y - sun.coor.y, orb.coor.x - sun.coor.x);

                    // Reduce the distance between the orb and sun by a small factor
                    double distanceReduction = 5 / (orbDistance); // Adjust the reduction factor as needed
                    orbDistance -= distanceReduction;

                    // Calculate the new angle for the enemy orb by incrementing the current angle
                    double angularSpeed = 5 / (orb.radius * orbDistance); // Adjust the angular speed as needed
                    orbAngle += angularSpeed;

                    // Calculate the new coordinates based on the new angle and distance
                    orb.coor.x = sun.coor.x + orbDistance * Math.Cos(orbAngle);
                    orb.coor.y = sun.coor.y + orbDistance * Math.Sin(orbAngle);

                    // Update the enemy orb's ellipse position on the canvas
                    Canvas.SetTop(orb.ellipse, orb.coor.x - orb.radius / 2);
                    Canvas.SetLeft(orb.ellipse, orb.coor.y - orb.radius / 2);
                }

                // Reduce the distance between the player and sun by a small factor
                double playerDistanceReduction = 5 / (playerDistance); // Adjust the reduction factor as needed
                playerDistance -= playerDistanceReduction;

                // Calculate the new angle for the player by incrementing the current angle
                playerAngle += 5 / (playerDistance * player.radius); // Adjust the angular speed as needed

                // Calculate the new coordinates for the player based on the new angle and distance
                player.coor.x = sun.coor.x + playerDistance * Math.Cos(playerAngle);
                player.coor.y = sun.coor.y + playerDistance * Math.Sin(playerAngle);

                // Update the player's ellipse position on the canvas
                Canvas.SetTop(player.ellipse, player.coor.x - player.radius / 2);
                Canvas.SetLeft(player.ellipse, player.coor.y - player.radius / 2);
            }
        }


    }
}
