using Slam_MapEditor.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Telerik.Windows.Controls;

namespace Slam_MapEditor.View
{
    /// <summary>
    /// Interaction logic for Properties.xaml
    /// </summary>
    public partial class PropertiesPane : UserControl
    {
        public static PropertiesPane Instance { get; set; }

        public PropertiesPane()
        {
            InitializeComponent();
            Instance = this;
        }

        private void Cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// 경로, 장애물 프로퍼티창 숨김
        /// </summary>
        public void hidePropertypane()
        {
            property_navi.Visibility = System.Windows.Visibility.Collapsed;
            property_obs.Visibility = System.Windows.Visibility.Collapsed;
            NavinObsDiagram.Instance.setShapeDefultColor();
        }

        /// <summary>
        /// 네비, 장애물 프로퍼티창 출력
        /// </summary>
        /// <param name="type"></param>
        public void settingProperty(string type)
        {
            if (type == "navi")
            {
                property_navi.Visibility = System.Windows.Visibility.Visible;
                property_obs.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                property_navi.Visibility = System.Windows.Visibility.Collapsed;
                property_obs.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}