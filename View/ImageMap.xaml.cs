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
using System.Windows.Threading;
using Microsoft.Win32;
using OpenCvSharp;
using System.IO;
using Slam_MapEditor.Model;
using Telerik.Windows.Controls;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Media.Media3D;
using WPFCustomMessageBox;

namespace Slam_MapEditor.View
{
    /// <summary>
    /// Interaction logic for VideoExplorer.xaml
    /// </summary>


    public partial class ImageMap : UserControl
    {
        //public string[] pFileNames;
        protected int pCurrentImage { get; set; }
        protected int currentimage;
        public List<string> fileNames = new List<string>();
        public List<string> ListViewfileNames = new List<string>();
        protected int temp;
        System.Timers.Timer timer;

        public System.Windows.Media.Media3D.Point3D pt;
        public static ImageMap instance { get; set; }
       
        //DispatcherTimer Playtimer = new DispatcherTimer();
        //System.Windows.Window message = new System.Windows.Window();

        public ImageMap()
        {
            InitializeComponent();
            instance = this;

        }
        #region Timer
        private void PlayTimer(bool play = true)
        {

            timer.Interval = 100;
           
            if (play)
            {
              
                timer.Elapsed += Timer_Play;

            }
            else
            {
          
                timer.Elapsed += Timer_Rewind;
                
            }
           

            timer.Start();

        }

        private void Timer_Rewind(object sender, System.Timers.ElapsedEventArgs e)
        {
         
            --pCurrentImage;
            ShowCurrentImage();
            if (pCurrentImage >= fileNames.Count)
            {
                timer.Stop();
                timer = null;
            }

        }

        private void Timer_Play(object sender, System.Timers.ElapsedEventArgs e)
        {
            ++pCurrentImage;
            ShowCurrentImage();
           
        }

      
        #endregion
        private void ShowCurrentImage()
        {
            
            if (pCurrentImage >= 0 && pCurrentImage < fileNames.Count)
            {
                var uri = new Uri(fileNames[pCurrentImage]);
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    //   slider.Value = pCurrentImage;
                    slider.Value = pCurrentImage;

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.Default;
                    bitmap.UriSource = uri;
                    bitmap.EndInit();
                    imageshow.Source = bitmap;

                    textBox.Text = pCurrentImage.ToString();


                    VIewer3D.Instance.startDrawExistingPath(pCurrentImage);
                    if(pCurrentImage == fileNames.Count)
                    {
                        timer.Stop();
                        timer = null;
                    }
                }));
            }
        }
        #region ButtonEvents

        private void ShowSliderCurrentImage()
        {
            if (pCurrentImage >= 0 && pCurrentImage < fileNames.Count)
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(fileNames[pCurrentImage]);
                bitmap.EndInit();
                imageshow.Source = bitmap;

                //imageshow.Source = new BitmapImage(new Uri(fileNames[pCurrentImage]));

                textBox.Text = pCurrentImage.ToString();
            }

        }

        bool playflag = true;
        private void RadButton_Click_Play(object sender, RoutedEventArgs e)
        {
            try
            {
                if (playflag)
                {
                    if (timer != null)
                    {
                        timer.Stop();
                        timer = null;
                    }
                        timer = new System.Timers.Timer();

                 
                    PlayTimer();
                   
                
                   
                   
                    
                }

                playflag = false;
                rewindflag = true;
            }
            catch { MessageBox.Show("파일이 선택되지 않았습니다."); }
        }


        private void RadButton_Click_Stop(object sender, RoutedEventArgs e)
        {
            try
            {
                playflag = true;
                rewindflag = true;
                //buttonrewind.IsEnabled = true;
                //buttonstop.IsEnabled = false;
                //buttonplay.IsEnabled = true;
               // timer.Dispose();
                //timer.AutoReset = false;
                timer.Stop();
               // timer.Interval = 0;

            }
            catch { MessageBox.Show("파일이 선택되지 않았습니다."); }
        }

        bool rewindflag = true;
        private void RadButton_Click_Preview(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rewindflag)
                {
                    if (timer != null)
                    {
                        timer.Stop();
                        timer = null;
                    }
                    timer = new System.Timers.Timer();
                    //check if this was first image in list
                    PlayTimer(false);
                    if (pCurrentImage < 0)
                    {
                        timer.Stop();
                        timer = null;
                    }
                   
                }
                rewindflag = false;
                playflag = true;
            }
            catch { MessageBox.Show("파일이 선택되지 않았습니다."); }

        }

        private void RadButton_Click_Skip(object sender, RoutedEventArgs e)
        {
            try
            {
                pCurrentImage = pCurrentImage + 5;
                //check if this was last image in list
                if (pCurrentImage >= fileNames.Count)
                    pCurrentImage = fileNames.Count == 0 ? -1 : 0;//if this was last image, go to first image
                ShowCurrentImage();
            }
            catch { MessageBox.Show("파일이 선택되지 않았습니다."); }
        }

        #endregion

        //private void slider_ValueChanged(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {

        //        slider.Value = pCurrentImage;
        //        pCurrentImage = (int)slider.Value;
        //        slider.Maximum = fileNames.Count - 1;
        //        ++pCurrentImage;
        //        if (pCurrentImage >= fileNames.Count)
        //            pCurrentImage = fileNames.Count == 0 ? -1 : fileNames.Count;//if this exceeds last image, go to last image
        //        ShowCurrentImage();

        //    }
        //    catch { MessageBox.Show("파일이 선택되지 않았습니다."); }
        //}
        //public void getimagefile()
        //{
        //    pCurrentImage = 0;


        //}


        //private void radButton_Open_Folder(object sender, RoutedEventArgs e)
        //{

        //    {

        //        OpenFileDialog ofd = new OpenFileDialog(){ Multiselect = true, ValidateNames = true, Filter = "PNG|*.png" };

        //      //  RadOpenFolderDialog rofd = new RadOpenFolderDialog();

        //        if (ofd.ShowDialog() == true)
        //        {




        //            //VIewer3D.Instance.startDrawExsitingPath();




        //        }
        //    }
        //}

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pCurrentImage = listView.SelectedIndex;

            ShowCurrentImage();


        }

        private void imageshow_MouseMove(object sender, MouseEventArgs e)
        {


        }

        private void slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {

                // slider.Value = pCurrentImage;
                pCurrentImage = (int)slider.Value;
                slider.Maximum = fileNames.Count - 1;

                if (pCurrentImage >= fileNames.Count)
                    pCurrentImage = fileNames.Count == 0 ? -1 : fileNames.Count;//if this exceeds last image, go to last image
                ShowSliderCurrentImage();


            }
            catch { MessageBox.Show("파일이 선택되지 않았습니다."); }
        }
        public void getimagefile()
        {
            pCurrentImage = 0;


        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int data = 0;
            if (int.TryParse(textBox.Text, out data))
            {
                pCurrentImage = data;

                ShowCurrentImage();
            }
        }

        private void imageshow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CoordinateTransformation dd = new CoordinateTransformation();
           
            Mat rgb = null;
            string rgbpath = System.IO.Path.Combine(Global.ImageDataPath, "rgb_" + pCurrentImage.ToString("D5") + ".jpg");
            rgb = Cv2.ImRead(rgbpath);

            OpenCvSharp.Size imagesize = rgb.Size();



            int inputx = ((int)e.GetPosition(imageshow).X * imagesize.Width) / ((int)imageshow.ActualWidth);
            int inputy = ((int)e.GetPosition(imageshow).Y * imagesize.Height) / ((int)imageshow.ActualHeight);



            pt = dd.getvalue(pCurrentImage, inputx, inputy);
         //   Console.WriteLine(pt);
            if (pt != new Point3D(0, 0, 0))
            {
                VIewer3D.Instance.ClickedCoordinate();
            }
          
        }

        private void btn_hideList_Click(object sender, RoutedEventArgs e)
        {
            if (listView.Visibility == Visibility.Visible)
            {
                listView.Visibility = Visibility.Collapsed;
                maingrid.RowDefinitions[3].Height = new GridLength(0);

                btn_hideList.Content = "▲";
            }
            else if (listView.Visibility == Visibility.Collapsed)
            {
                listView.Visibility = Visibility.Visible;
                maingrid.RowDefinitions[3].Height = new GridLength(150, GridUnitType.Star);

                btn_hideList.Content = "▼";
            }
        }

        private void pushpin_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("현재 경로를 추가하시겠습니까?", "Question", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                
                VIewer3D.Instance.startDrawExistingPath(pCurrentImage , false);
                NavinObsDiagram.Instance.Navidiagram.AutoFit();
            }
            else {

            }
        }
     

        private void imageshow_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (pt == new Point3D(0, 0, 0))
            {
                MessageBox.Show("선택된 좌표가 올바르지 않습니다.");
            }
            else
            {

                Slam_MapEditor.popup.DrawPathOrObstacle dpo= new popup.DrawPathOrObstacle();
                dpo.ShowDialog();
            
            //{
            //    MessageBoxResult result = CustomMessageBox.ShowYesNoCancel("입력 할 물체를 선택하세요.", "Question", "경로", "장애물", "취소");
            //    if (result == MessageBoxResult.Yes)
            //    {
                    
            //        VIewer3D.Instance.startDrawClickedPathOrObstacle(6);

            //        NavinObsDiagram.Instance.Navidiagram.AutoFit();
            //    }
            //    else if (result == MessageBoxResult.No)
            //    {
            //        MessageBoxResult results = CustomMessageBox.ShowYesNo("장애물의 종류를 선택하세요", "Question", "입력", "취소");


            //        NavinObsDiagram.Instance.Navidiagram.AutoFit();
            //        NavinObsDiagram.Instance.ObsDiagram.AutoFit();
            //    }
            //    else
            //    {

            //    }

            }
        }

      
    }
}
