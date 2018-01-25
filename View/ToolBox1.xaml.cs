using Slam_MapEditor.Model;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using Slam_MapEditor.View;
using System.Windows.Media.Media3D;

namespace Slam_MapEditor.View
{
    /// <summary>
    /// Interaction logic for ToolBox1.xaml
    /// </summary>


    public partial class ToolBox1 : UserControl
    {

        double obsrectloc;
        double navirectloc;
        double obsrectloctemp;

        MainWindow mainWindow = ((MainWindow)System.Windows.Application.Current.MainWindow);
        // ToolBox1 instance = new ToolBox1();



        public static ToolBox1 Instance { get; set; }


        public ToolBox1()
        {
            InitializeComponent();
            //MouseDown += Grid_MouseDown;
            //MouseMove += Grid_MouseMove;
            //ObsRectangle.SetValue(Grid.RowProperty, 1);
            //ObsRectangle.SetValue(Grid.ColumnProperty, 2);
            //MainGrid.Children.Add(ObsRectangle);

            Instance = this;
        }

        Rectangle GetNewRect(Brush brush)
        {
            return new Rectangle
            {
                Height = 23,
                Width = 15,
                Fill = brush,
                IsHitTestVisible = true,
                HorizontalAlignment = HorizontalAlignment.Left,
                AllowDrop = true

            };
        }



        //private void Grid_MouseMove(object sender, MouseEventArgs e)
        //{
        //    //var pos = e.GetPosition(MainGrid);
        //    //ObsRectangle.SetValue(Grid.ColumnProperty, pos);
        //    //Console.WriteLine(pos);
        //}

        //private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    //if (DataManager.NaviDis != null)
        //    //{
        //    //    for (int i = DataManager.NaviDis.Count - 1; i < DataManager.NaviDis.Count; i++)
        //    //    {
        //    //        NaviRectLocation(new NaviPoint((float)DataManager.NaviDis[i]));
        //    //    }
        //    //}


        //}

        public void NaviRectLocation()
        {
            //if (DataManager.NaviDis != null)
            //{
            //    for (int i = DataManager.NaviDis.Count - 1; i < DataManager.NaviDis.Count; i++)
            //    {

            //        //Console.WriteLine(DataManager.NaviDis[i]);
            //       // rectloc += 30 + p.NaviDistance * 0.3;
            //       NaviRectangle.Margin = new Thickness(rectloc, 0, 0, 0);

            //    }
            //}
            //NaviRectangle.SetValue(Grid.RowProperty, 0);
            Rectangle rect = GetNewRect(Brushes.DarkBlue);
            rect.SetValue(Grid.RowProperty, 0);
            rect.SetValue(Grid.ColumnProperty, 0);

            //네비 정보 만들어서

            NaviInfo info = new NaviInfo();

            //info.Navi_Index = DataManager.NaviPoints[DataManager.NaviPoints.Count - 1].INDEX;
            //info.NaviPosX = DataManager.NaviPoints[DataManager.NaviPoints.Count - 1].NaviPosX;
            //info.NaviPosY = DataManager.NaviPoints[DataManager.NaviPoints.Count - 1].NaviPosY;



            //tag에 삽입
            rect.Tag = info;


            navirectloc += 30;
            rect.Margin = new Thickness(navirectloc, 0, 0, 0);
            MainPointGrid.Children.Add(rect);
        }


        public void ObsRecLocation()
        {
            //사각형 그려주는데
            Rectangle rect = GetNewRect(Brushes.Gray);
            rect.SetValue(Grid.RowProperty, 0);
            rect.SetValue(Grid.ColumnProperty, 0);

            ObsInfo info = new ObsInfo();
            //info.Obs_Index = DataManager.ObsPoints[DataManager.ObsPoints.Count - 1].INDEX;
            //info.ObsPosX = DataManager.ObsPoints[DataManager.ObsPoints.Count - 1].ObsPosX;
            //info.ObsPosY = DataManager.ObsPoints[DataManager.ObsPoints.Count - 1].ObsPosY;

            rect.Tag = info;

            //if (DataManager.ObsDis != null)
            //{
            //    for (int i = DataManager.ObsDis.Count - 1; i < DataManager.ObsDis.Count; i++)
            //    {
            //        obsrectloc += 30 + DataManager.ObsDis[i].ObsDistance * 0.3;
            //        rect.Margin = new Thickness(obsrectloc, 0, 0, 0);

            //    }
            //}
            //ObsPointGrid.Children.Add(rect);

        }

        /// <summary>
        /// 경로를 선택하기전 색을 초기색으로 변경해준다.
        /// </summary>
        public void reSetNaviPointColor()
        {
            foreach (var item in MainPointGrid.Children)
            {
                if (item is Rectangle)
                {
                    Rectangle rec = item as Rectangle;
                    rec.Fill = new SolidColorBrush(Colors.DarkBlue);
                }
            }
        }

        private void MainPointGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //선택색 초기화
            reSetNaviPointColor();

            Point recPoint = e.GetPosition(MainPointGrid);

            int index = 0;
            foreach (Rectangle item in MainPointGrid.Children)
            {
                index++;
                if (item.IsMouseOver == true)
                {
                    item.Fill = new SolidColorBrush(Colors.Cyan);
                    var tag = item.Tag as NaviInfo;
                    //PropertiesPane.Instance.settingProperty("navi");
                    //PropertiesPane.Instance.property_navi.Item = null;
                    //PropertiesPane.Instance.property_navi.Item = tag;
                    break;
                }
            }

            // var data = DataManager.NaviPoints[index - 1];

            //VIewer3D.Instance.NaviPointGrid();

            //뷰포트에서 값 찾아옴
            //if (VIewer3D.Instance != null)
            //{
            //    foreach (var item in VIewer3D.Instance.myViewport3D.Children)
            //    {
            //        if (item is NaviPoint)
            //        {
            //            NaviPoint checkdata = item as NaviPoint;
            //            Model3DGroup group = checkdata.Content as Model3DGroup;
            //            if (group != null)
            //            {
            //                if (group.Children.Count == 1)
            //                {
            //                    var mesh = group.Children[0];

            //                    if (mesh.Bounds.X == data.NaviPosX - 1
            //                        && mesh.Bounds.Y == data.NaviPosY - 1)
            //                    {

            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        }

        public void reSetObsPointColor()
        {
            //foreach (var item in ObsPointGrid.Children)
            //{
            //    if (item is Rectangle)
            //    {
            //        Rectangle rec = item as Rectangle;
            //        rec.Fill = new SolidColorBrush(Colors.LightGray);
            //    }
            //}
        }

        //장애물 클릭하면
        private void ObsPointGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Point recPoint = e.GetPosition(ObsPointGrid);

            //int index = 0;
            //foreach (Rectangle item in ObsPointGrid.Children)
            //{
            //    index++;
            //    if (item.IsMouseOver == true)
            //    {
            //        item.Fill = new SolidColorBrush(Colors.Cyan);
            //        var tag = item.Tag as ObsInfo;
            //        PropertiesPane.Instance.settingProperty("");
            //        PropertiesPane.Instance.property_obs.Item = null;
            //        PropertiesPane.Instance.property_obs.Item = tag;
            //        break;
            //    }
            //}
        }

        //private void UserControl_Loaded(object sender, RoutedEventArgs e)
        //{
        //    //LeftGrid

        //    List<string> data = new List<string>();
        //    data.Add("경로");
        //    data.Add("횡단보도");
        //    data.Add("장애물");
        //    data.Add("계단");
        //    data.Add("출입구");



        //    for (int i = 0; i < data.Count; i++)
        //    {
        //        //왼쪽 제목부분 ROW 정의


        //        LeftGrid.RowDefinitions.Add(new RowDefinition());
        //        MainPointGrid.RowDefinitions.Add(new RowDefinition());

        //        Border leftborder = new Border();
        //        Border rightborder = new Border();

        //        //보더 색상 체크
        //        if (i % 2 == 0)
        //        {
        //            leftborder.Background = new SolidColorBrush(Colors.LightGray);
        //            rightborder.Background = new SolidColorBrush(Colors.LightGray);
        //        }
        //        else
        //        {
        //            leftborder.Background = new SolidColorBrush(Colors.WhiteSmoke);
        //            rightborder.Background = new SolidColorBrush(Colors.WhiteSmoke);
        //        }

        //        ////오른쪽 보더
        //        //Grid.SetRow(rightborder, i);
        //        //MainPointGrid.Children.Add(rightborder);

        //        ////왼쪽 보더
        //        //Grid.SetRow(leftborder, i);
        //        //LeftGrid.Children.Add(leftborder);

        //        ////TextBlock 생성
        //        //var textblock = headerblcok(data[i]);
        //        //Grid.SetRow(textblock, i);
        //        //LeftGrid.Children.Add(textblock);
        //    }





        //}


        //public TextBlock headerblcok(string title)
        //{
        //    TextBlock tb = new TextBlock();
        //    tb.VerticalAlignment = VerticalAlignment.Center;
        //    tb.HorizontalAlignment = HorizontalAlignment.Right;
        //    tb.FontSize = 14;
        //    tb.Text = title + " : ";
        //    tb.Margin = new Thickness(0, 0, 10, 0);
        //    return tb;
        //}


    }
}
