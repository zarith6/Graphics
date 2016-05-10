using Microsoft.Win32;
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
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace Graphics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const float POINT_HEIGHT = 5;
        const float POINT_WIDTH = 5;
        private Ellipse lastEllipseClicked;
        private Point clickOffSet;
        private bool isCaptured = false;
        Canvas canvasToDraw;
        private Ellipse permSelectedEllipse;
        private Ellipse permSelectedEllipse2;
        int amtOfPermSelectedEllipses = 0;
        List<double> xPointsFromFile;
        List<double> yPointsFromFile;
        List<Point> listOfPointsToPlot;
        
        public MainWindow()
        {
            InitializeComponent();
            canvasToDraw = gridCanvas;
            gridCanvas.MouseLeftButtonDown += leftClickOnCanvas;
            xPointsFromFile = new List<double>();
            yPointsFromFile = new List<double>();
            listOfPointsToPlot = new List<Point>();
        }

        public void middleMouseClickOnEllipse(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                Ellipse tempEllipse = sender as Ellipse;

                if (!tempEllipse.Equals(permSelectedEllipse) && amtOfPermSelectedEllipses == 0)
                {
                    permSelectedEllipse = tempEllipse;
                    tempEllipse.Fill = Brushes.Red;
                    amtOfPermSelectedEllipses++;
                }
                else if(!tempEllipse.Equals(permSelectedEllipse) && amtOfPermSelectedEllipses == 1)
                {
                    //permSelectedEllipse.Fill = Brushes.White;
                    //permSelectedEllipse = tempEllipse;
                    //permSelectedEllipse.Fill = Brushes.Red;

                    permSelectedEllipse2 = tempEllipse;
                    permSelectedEllipse2.Fill = Brushes.Red;
                    amtOfPermSelectedEllipses++;
                }
                else if(!tempEllipse.Equals(permSelectedEllipse) && amtOfPermSelectedEllipses == 2)
                {
                    permSelectedEllipse.Fill = Brushes.White;
                    permSelectedEllipse = tempEllipse;
                    permSelectedEllipse.Fill = Brushes.Red;
                }
                else
                {
                    tempEllipse.Fill = Brushes.White;
                    tempEllipse = null;
                    amtOfPermSelectedEllipses--;
                }
            }
        }


        
        public void leftClickOnCanvas(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.DirectlyOver == canvasToDraw)
            {
                Ellipse newEllipse = new Ellipse();
                //Point pointToPlace = new Point((e.GetPosition(this).X - (POINT_WIDTH * 2)), (e.GetPosition(this).Y - (POINT_HEIGHT * 2)));
                Point pointToPlace = new Point((Mouse.GetPosition(this).X - (POINT_WIDTH * 2)), ((Mouse.GetPosition(this).Y - (POINT_HEIGHT * 2))));
                newEllipse.Height = POINT_HEIGHT;
                newEllipse.Width = POINT_WIDTH;
                newEllipse.Fill = Brushes.White;

                newEllipse.MouseLeftButtonDown += OnEllipseMouseLeftButtonDown;
                newEllipse.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;
                newEllipse.MouseMove += Any_MouseMove;
                newEllipse.MouseRightButtonDown += OnEllipseMouseRightbuttonDown;
                newEllipse.MouseDown += middleMouseClickOnEllipse;
                lastEllipseClicked = newEllipse;
                placeEllipse(newEllipse, pointToPlace, true);
                //canvasToDraw.Children.Add(newEllipse);
            }
        }

    

        public void OnEllipseMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lastEllipseClicked = sender as Ellipse;
            Mouse.Capture(lastEllipseClicked);
            isCaptured = true;
            clickOffSet = e.GetPosition(lastEllipseClicked);
            //lastEllipseClicked.Fill = Brushes.Transparent;
            
            if (lastEllipseClicked.Stroke == Brushes.Red)
            {
                lastEllipseClicked.Stroke = Brushes.White;
                lastEllipseClicked.Fill = Brushes.White;
            }
            else
            {
                lastEllipseClicked.Stroke = Brushes.Yellow;
                lastEllipseClicked.Fill = Brushes.Yellow;
             }
        }

        public void OnEllipseMouseRightbuttonDown(object sender, MouseButtonEventArgs e)
        {
            lastEllipseClicked = sender as Ellipse;
            //Point offSet = e.GetPosition(lastEllipseClicked);
            Point mouseCoords = e.GetPosition(canvasToDraw);
            Vector offset = VisualTreeHelper.GetOffset(lastEllipseClicked);
            //double xCoordAdjusted = Canvas.GetLeft(lastEllipseClicked);
            double xCoordAdjusted = offset.X;
            double yCoordAdjusted = offset.Y;
            ////double yCoordAdjusted = Canvas.GetTop(lastEllipseClicked);
            //double yCoordAdjusted = Canvas.GetBottom(lastEllipseClicked);
            xCoordLabel.Content = xCoordAdjusted + (POINT_HEIGHT / 2) ;
            yCoordLabel.Content = yCoordAdjusted + (POINT_WIDTH / 2);
        }

        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Ellipse anEllipse = (Ellipse)sender;
            lastEllipseClicked.Stroke = Brushes.White;
            lastEllipseClicked.Fill = Brushes.White;
            
            Mouse.Capture(null);
            isCaptured = false;

           if(lastEllipseClicked.Equals(permSelectedEllipse))
            {
                lastEllipseClicked.Fill = Brushes.Red;
                lastEllipseClicked = null;
            }
            else
            {
                lastEllipseClicked = null;
            }
        }

        
        private void Any_MouseMove(object sender, MouseEventArgs e)
        {
            Ellipse anEllipse = (Ellipse)sender;
            //isCaptured = true;
            if (e.LeftButton == MouseButtonState.Pressed)
            {


                if (lockPointsCheckBox.IsChecked.Value)
                { MessageBox.Show("All points are locked!"); }

                else if (anEllipse.Fill == Brushes.White || anEllipse.Fill == Brushes.Yellow)
                {
                    ///lastEllipseClicked.SetValue(Canvas.LeftProperty, e.GetPosition(this).X - (POINT_WIDTH * 2));
                    ///lastEllipseClicked.SetValue(Canvas.BottomProperty, e.GetPosition(this).Y - (POINT_HEIGHT * 2));
                    //lastEllipseClicked.SetValue(Canvas.TopProperty, e.GetPosition(this).Y - (POINT_HEIGHT * 2));


                    ///Canvas.SetBottom(lastEllipseClicked, e.GetPosition(this).Y - (POINT_WIDTH * 2));
                    double yCoord = (300 - Mouse.GetPosition(this).Y) + (POINT_HEIGHT * 1.5);
                    Canvas.SetTop(lastEllipseClicked, yCoord);
                    Canvas.SetLeft(lastEllipseClicked, Mouse.GetPosition(this).X - (POINT_WIDTH * 2.5));

                }   
                
            }
        }


        private void placeEllipse(Ellipse ellipse, Point point, bool isGeneratedByMouse)
        {
            double xAdjusted = point.X - (POINT_WIDTH / 2);
            double yAdjusted = point.Y - (POINT_HEIGHT / 2);
            

            if (isGeneratedByMouse)
            {
                Canvas.SetLeft(ellipse, xAdjusted);
                Canvas.SetBottom(ellipse, yAdjusted);
                canvasToDraw.Children.Add(ellipse);
                
            }
            else
            {
                Canvas.SetLeft(ellipse, xAdjusted);
                Canvas.SetTop(ellipse, yAdjusted);
                canvasToDraw.Children.Add(ellipse);
            }
        }

        private void snapToGridButton_Click(object sender, RoutedEventArgs e)
        {
            if (xPositionTextBox.Text != "" && yPositionTextBox.Text != "" && amtOfPermSelectedEllipses == 1)
            {
                double x = Convert.ToDouble(xPositionTextBox.Text) - (POINT_WIDTH / 2);
                double y = Convert.ToDouble(yPositionTextBox.Text) - (POINT_HEIGHT / 2);
                Point point = new Point(x, y);

                if (permSelectedEllipse != null)
                {
                    //permSelectedEllipse.SetValue(Canvas.LeftProperty, x);
                    //permSelectedEllipse.SetValue(Canvas.TopProperty, y);
                    Canvas.SetLeft(permSelectedEllipse, x);
                    Canvas.SetTop(permSelectedEllipse, y);
                }
            }
            else
            {
                MessageBox.Show("You must enter coordinates and select a point before snapping to the grid.");
            }
        }

        private void loadPointsFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            xPointsFromFile.Clear();
            yPointsFromFile.Clear();
            OpenFileDialog openFiles = new OpenFileDialog();
            openFiles.Filter = "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
            
            if(openFiles.ShowDialog() == true)
            {
                string fullPath = openFiles.FileName;
                using (TextFieldParser parser = new TextFieldParser(fullPath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        

                        for(int i = 0; i < fields.Length; i++)
                        {
                            
                            if(i % 2 == 0)
                            {
                                xPointsFromFile.Add(Convert.ToDouble(fields[i]));
                                
                            }
                            else if(i % 2 == 1)
                            {
                                yPointsFromFile.Add(Convert.ToDouble(fields[i]));
                                
                            }
                        }
                    }
                }
                Point point = new Point();
                for(int i = 0; i < xPointsFromFile.Count; i++)
                {
                    point.X = xPointsFromFile.ElementAt(i);
                    point.Y = yPointsFromFile.ElementAt(i);
                    listOfPointsToPlot.Add(point);
                    pointsListBox.Items.Add(listOfPointsToPlot.ElementAt(i).ToString());
                }
            }
            
        }

        private void plotPointsButton_Click(object sender, RoutedEventArgs e)
        {
            Point tempPoint = new Point();
            for(int i = 0; i < listOfPointsToPlot.Count; i++)
            {
                Ellipse newEllipse = new Ellipse();
                newEllipse.Height = POINT_HEIGHT;
                newEllipse.Width = POINT_WIDTH;
                newEllipse.Fill = Brushes.White;

                newEllipse.MouseLeftButtonDown += OnEllipseMouseLeftButtonDown;
                newEllipse.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;
                newEllipse.MouseMove += Any_MouseMove;
                newEllipse.MouseRightButtonDown += OnEllipseMouseRightbuttonDown;
                newEllipse.MouseDown += middleMouseClickOnEllipse;

                tempPoint = listOfPointsToPlot.ElementAt(i);
                placeEllipse(newEllipse, tempPoint, false);
            }
        }

        private void pointsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pointsListBox.SelectedItem != null)
            {
                string coords = pointsListBox.SelectedItem.ToString();
                string[] splitUpCoords = coords.Split(',');
                double xCoord = Convert.ToDouble(splitUpCoords[0]);
                double yCoord = Convert.ToDouble(splitUpCoords[1]);

                foreach (UIElement ui in canvasToDraw.Children)
                {
                    double x = (double)ui.GetValue(Canvas.LeftProperty);
                    x += 2.5;
                    double y = (double)ui.GetValue(Canvas.TopProperty);
                    y += 2.5;

                    if (x == xCoord && y == yCoord)
                    {
                        Ellipse temp = (Ellipse)ui;

                        if (amtOfPermSelectedEllipses == 0)
                        {
                            permSelectedEllipse = temp;
                            permSelectedEllipse.Fill = Brushes.Red;
                            amtOfPermSelectedEllipses = 1;
                        }

                        else if (amtOfPermSelectedEllipses == 1)
                        {
                            permSelectedEllipse.Fill = Brushes.White;
                            permSelectedEllipse = temp;
                            permSelectedEllipse.Fill = Brushes.Red;
                        }
                    }
                }
            }
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("This will clear all points on the grid and in the box. Continue?");
            if (result == MessageBoxResult.OK)
            {
                pointsListBox.Items.Clear();

                for (int i = 0; i < canvasToDraw.Children.Count; i++)
                {
                    canvasToDraw.Children.Clear();
                }
            }
        }

        private void outputButton_Click(object sender, RoutedEventArgs e)
        {
            string output = null;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "Coordinates";
            sfd.DefaultExt = ".txt";
            sfd.Filter = "Text Documents (.txt)|*.txt|All Files (*.*)|*.*";

            Nullable<bool> result = sfd.ShowDialog();
            if (result == true)
            {
                
                for(int i = 0; i < pointsListBox.Items.Count; i++)
                {
                    output += pointsListBox.Items.GetItemAt(i).ToString() + ",";
                }
                output = output.Remove(output.Length - 1);
                File.WriteAllText(sfd.FileName, output);
            }
        }

        private void addPointsToListBoxButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("This will clear the existing points from the box. Continue?","Continue?",MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                pointsListBox.Items.Clear();
                foreach (UIElement ui in canvasToDraw.Children)
                {
                    double x = (double)ui.GetValue(Canvas.LeftProperty);
                    double y = (double)ui.GetValue(Canvas.TopProperty);
                    x += 2.5;
                    y += 2.5;
                    pointsListBox.Items.Add(x.ToString() + "," + y.ToString());
                }
            }
        }

        private void getDistanceButton_Click(object sender, RoutedEventArgs e)
        {
            if(amtOfPermSelectedEllipses != 2)
            {
                MessageBox.Show("You must have two points selected before performing this action.");
            }
            else
            {
                double x1 = (double)permSelectedEllipse.GetValue(Canvas.LeftProperty);
                double y1 = (double)permSelectedEllipse.GetValue(Canvas.TopProperty);
                x1 += 2.5;
                y1 += 2.5;

                warningLabel.Content = "Middle click the point you want to find the distance from.";

                
                    double x2 = (double)permSelectedEllipse2.GetValue(Canvas.LeftProperty);
                    double y2 = (double)permSelectedEllipse2.GetValue(Canvas.TopProperty);
                    x2 += 2.5;
                    y2 += 2.5;

                    double xResult = x1 - x2;
                    double yResult = y1 - y2;

                warningLabel.Content = xResult.ToString() + "," + yResult.ToString();
                   
            }
        }

        private void deselectEllipsesButton_Click(object sender, RoutedEventArgs e)
        {
            permSelectedEllipse.Fill = Brushes.White;
            permSelectedEllipse2.Fill = Brushes.White;
            amtOfPermSelectedEllipses = 0;
            permSelectedEllipse2 = null;
            permSelectedEllipse = null;
        }
    }
}