using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Slam_MapEditor.Model;
using System.Collections;

namespace Slam_MapEditor.View
{
    /// <summary>
    /// Interaction logic for ToolBox.xaml
    /// </summary>
    public partial class ToolBox : UserControl
    {
        public ToolBox()
        {
            InitializeComponent();
        }


        private void list_path_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Tile selectData = list_path.SelectedItem as Tile;
            //var prop = mainWindow.FindView(typeof(PropertiesPane)) as PropertiesPane;
            //if (prop != null)
            //{
            //    prop.property.Item = selectData;
            //}
            MainWindow mainWindow = ((MainWindow)System.Windows.Application.Current.MainWindow);
            NaviPoint selectededdata = list_path.SelectedItem as NaviPoint;
            var propa = mainWindow.FindView(typeof(PropertiesPane)) as PropertiesPane;
            if (propa != null)
            {
               // propa.property.Item = selectededdata;
            }


        }

        private void list_obs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainWindow mainWindow = ((MainWindow)System.Windows.Application.Current.MainWindow);
            ObsPoint selectededdata = list_obs.SelectedItem as ObsPoint;
            var propa = mainWindow.FindView(typeof(PropertiesPane)) as PropertiesPane;
            if (propa != null)
            {
                //propa.property.Item = selectededdata;
            }
        }
        //public static void MoveUp(ListBox listBox, IList items)
        //{
        //    if (listBox.SelectedIndex < 0)
        //        return;

        //    if (listBox.SelectedIndex == 0 || listBox.Items.Count < 2)
        //        return;

        //    var tempItem = items[listBox.SelectedIndex - 1];
        //    items[listBox.SelectedIndex - 1] = items[listBox.SelectedIndex];
        //    items[listBox.SelectedIndex] = tempItem;

        //    show(listBox, items);
        //}

        //public static void MoveDown(ListBox listBox, IList items)
        //{
        //    if (listBox.SelectedIndex < 0)
        //        return;

        //    if (listBox.SelectedIndex == listBox.Items.Count - 1 ||
        //        listBox.Items.Count < 2)
        //        return;

        //    var tempItem = items[listBox.SelectedIndex + 1];
        //    items[listBox.SelectedIndex + 1] = items[listBox.SelectedIndex];
        //    items[listBox.SelectedIndex] = tempItem;

        //    show(listBox, items);
        //}

        //public static void Delete(ListBox listBox, IList items)
        //{
        //    if (listBox.SelectedIndex > -1)
        //        items.RemoveAt(listBox.SelectedIndex);
        //    else
        //        return;

        //    show(listBox, items);
        //}

        //private static void show(ListBox listBox, IList items)
        //{
        //    listBox.Items.Clear();
        //    foreach (var item in items)
        //        listBox.Items.Add(item);
        //}
    }

}