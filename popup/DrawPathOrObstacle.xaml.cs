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
using System.Windows.Shapes;

namespace Slam_MapEditor.popup
{
    /// <summary>
    /// Interaction logic for DrawPathOrObstacle.xaml
    /// </summary>
    public partial class DrawPathOrObstacle : Window
    {
        public DrawPathOrObstacle()
        {
            InitializeComponent();
        }

        private void Button_Input(object sender, RoutedEventArgs e)
        {
            if((bool)pathbutton.IsChecked)
            {
                View.VIewer3D.Instance.startDrawClickedPathOrObstacle(6);
                this.Close();
            }
            if((bool)obsbutton.IsChecked)
            {
                if(mylistbox.SelectedIndex == -1)
                {
                    MessageBox.Show("장애물의 종류를 선택해 주세요");
                    return;
                }
                View.VIewer3D.Instance.startDrawClickedPathOrObstacle(mylistbox.SelectedIndex);
                this.Close();
            }
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            View.VIewer3D.Instance.RemoveDots(View.VIewer3D.Instance.ClickedDot);
            View.VIewer3D.Instance.RemoveSomething();
            this.Close();
        }

        private void Path_Click(object sender, RoutedEventArgs e)
        {
            View.VIewer3D.Instance.startDrawClickedPathOrObstacle(7, false);
            mylistbox.Visibility = Visibility.Collapsed ;
        }

        private void Obstacle_Click(object sender, RoutedEventArgs e)
        {
            mylistbox.Visibility = Visibility.Visible;
            mylistbox.SelectedIndex = 1;
            mylistbox.SelectedIndex = 0;
        }

        private void pathbutton_Checked(object sender, RoutedEventArgs e)
        {
          
        }

        private void obsbutton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void mylistbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(mylistbox.SelectedItem !=null)
            {
                View.VIewer3D.Instance.startDrawClickedPathOrObstacle(mylistbox.SelectedIndex, false);
            }
            else
            {
                View.VIewer3D.Instance.startDrawClickedPathOrObstacle(7, false);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            View.VIewer3D.Instance.startDrawClickedPathOrObstacle(7, false);
        }
    }
}
