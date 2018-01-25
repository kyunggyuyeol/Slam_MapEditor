using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Docking;
using Slam_MapEditor.Model;
using Slam_MapEditor.View;
using Slam_MapEditor.popup;
using System.Yaml;
using Slam_MapEditor.common;
using Slam_MapEditor.Shape;
using System.Windows.Controls.Primitives;

namespace Slam_MapEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static MainWindow Instance { get; set; }

        List<NaviShape> saveNaviTemp = new List<NaviShape>();

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
            Application.Current.Exit += OnApplicationExit;
            KeyDown += MainWindow_KeyDown;
            Instance = this;
        }

        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {

                VIewer3D.Instance.QuarterViewer_keyDown(sender, e);
            }
            catch { }
        }

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            //var viewModel = this.DataContext as MainWindowViewModel;
            //if (viewModel != null)
            //{
            //    viewModel.Save(this.radDocking);
            //}

        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as MainWindowViewModel;
            if (viewModel != null)
            {
                viewModel.Load(this.radDocking);
            }

            //프로젝트 폴더 없으면 폴더 만듬
            if (!Directory.Exists(Global.projectPath))
            {
                Directory.CreateDirectory(Global.projectPath);
            }
        }

        private void OnPreviewShowCompass(object sender, PreviewShowCompassEventArgs e)
        {
            bool isRootCompass = e.Compass is RootCompass;
            var splitContainer = e.DraggedElement as RadSplitContainer;
            if (splitContainer != null)
            {
                bool isDraggingDocument = splitContainer.EnumeratePanes().Any(p => p is RadDocumentPane);
                var isTargetDocument = e.TargetGroup == null ? true : e.TargetGroup.EnumeratePanes().Any(p => p is RadDocumentPane);
                if (isDraggingDocument)
                {
                    e.Canceled = isRootCompass || !isTargetDocument;
                }
                else
                {
                    e.Canceled = !isRootCompass && isTargetDocument;
                }
            }
        }

        private void OnClose(object sender, StateChangeEventArgs e)
        {
            var documents = e.Panes.Select(p => p.DataContext).OfType<PaneViewModel>().Where(vm => vm.IsDocument).ToList();
            foreach (var document in documents)
            {
                ((MainWindowViewModel)this.DataContext).Panes.Remove(document);
            }
        }

        private void FilterActiveViewsSource(object sender, System.Windows.Data.FilterEventArgs e)
        {
            var vm = e.Item as PaneViewModel;
            e.Accepted = vm.IsDocument;
        }

        private void FilterToolboxesSource(object sender, System.Windows.Data.FilterEventArgs e)
        {
            var vm = e.Item as PaneViewModel;
            e.Accepted = !vm.IsDocument;
        }

        /// <summary>
        /// 도킹창을 찾아서 반환 해 준다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Object FindView(Type type)
        {
            Object result = null;

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                delegate
                {
                    foreach (var pane in radDocking.Panes)
                    {
                        if (pane.Content.GetType() == type)
                        {
                            result = pane.Content;
                        }
                    }
                }
            ));

            return result;
        }





        private void newdoc_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            //새 프로젝트
            NewProject pj = new NewProject();
            pj.ShowDialog();
            //OpenFileDialog open = new OpenFileDialog();
            //open.Multiselect = false;
            //open.Filter = "Map files (*.map)|*.map";
            //if (open.ShowDialog() == true)
            //{

            //    MainWindow mainWindow = ((MainWindow)System.Windows.Application.Current.MainWindow);
            //    MainWindowViewModel viewModel = mainWindow.DataContext as MainWindowViewModel;
            //    if (viewModel != null)
            //    {
            //        //3d 뷰어 켜고
            //        viewModel.Panes.Add(new PaneViewModel(typeof(VIewer3D)) { Header = "Map Viewer", IsDocument = true });

            //        List<Model.Tile> listdata = new List<Model.Tile>();

            //        BinaryReader reader = new BinaryReader(File.Open(open.FileName, FileMode.Open));
            //        float width = reader.ReadSingle();
            //        int size = reader.ReadInt32();

            //        Model.Tile.Width = width;
            //        Model.Tile.Size = size;


            //        for (int i = 0; i < size; i++)
            //        {
            //            float X = reader.ReadSingle();
            //            float Y = reader.ReadSingle();
            //            byte R = (byte)(reader.ReadSingle() * 255);
            //            byte G = (byte)(reader.ReadSingle() * 255);
            //            byte B = (byte)(reader.ReadSingle() * 255);

            //            listdata.Add(new Model.Tile(i, X, Y, R, G, B));
            //        }

            //        reader.Close();

            //        //3dViewer 창 가져와서
            //        VIewer3D viewer = FindView(typeof(VIewer3D)) as VIewer3D;
            //        //데이터 넣어주고
            //        viewer.MapData = listdata;

            //        //맵 그려줌
            //        viewer.startDrawMapData();
            //    }
            //}

        }

        //private void openfile_Click(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog open = new OpenFileDialog();
        //    open.Multiselect = false;
        //    open.Filter = "Map files (*.map)|*.map";
        //    if (open.ShowDialog() == true)
        //    {
        //        MainWindow mainWindow = ((MainWindow)System.Windows.Application.Current.MainWindow);
        //        MainWindowViewModel viewModel = mainWindow.DataContext as MainWindowViewModel;
        //        if (viewModel != null)
        //        {
        //            //3d 뷰어 켜고
        //            viewModel.Panes.Add(new PaneViewModel(typeof(VIewer3D)) { Header = "Map Viewer", IsDocument = true });

        //            List<Model.Tile> listdata = new List<Model.Tile>();

        //            BinaryReader reader = new BinaryReader(File.Open(open.FileName, FileMode.Open));
        //            float width = reader.ReadSingle();
        //            int size = reader.ReadInt32();

        //            Model.Tile.Width = width;
        //            Model.Tile.Size = size;


        //            for (int i = 0; i < size; i++)
        //            {
        //                float X = reader.ReadSingle();
        //                float Y = reader.ReadSingle();
        //                byte R = (byte)(reader.ReadSingle() * 255);
        //                byte G = (byte)(reader.ReadSingle() * 255);
        //                byte B = (byte)(reader.ReadSingle() * 255);

        //                listdata.Add(new Model.Tile(i, X, Y, R, G, B));
        //            }

        //            reader.Close();

        //            //3dViewer 창 가져와서
        //            VIewer3D viewer = FindView(typeof(VIewer3D)) as VIewer3D;
        //            //데이터 넣어주고
        //            viewer.MapData = listdata;

        //            //맵 그려줌
        //            viewer.startDrawMapData();
        //        }
        //    }
        //}

        private void newfile_Click(object sender, RoutedEventArgs e)
        {
            //새 프로젝트
            NewProject pj = new NewProject();
            pj.ShowDialog();

        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VIewer3D.Instance.path_input_event(sender, e);
                checkToggleBtn((sender as ToggleButton).Name);
            }
            catch
            {
                MessageBox.Show("맵을 먼저 열어주시기 바랍니다.");
                checkToggleBtn(string.Empty);
            }
        }
        private void RadButton_Click1(object sender, RoutedEventArgs e)
        {
            try
            {
                VIewer3D.Instance.obs_input_event(sender, e, ObsPoint.Kinds.Crosswalk);
                checkToggleBtn((sender as ToggleButton).Name);
            }
            catch
            {
                MessageBox.Show("맵을 먼저 열어주시기 바랍니다.");
                checkToggleBtn(string.Empty);
            }

        }
        private void RadButton_Click2(object sender, RoutedEventArgs e)
        {
            try
            {
                VIewer3D.Instance.obs_input_event(sender, e, ObsPoint.Kinds.Gate);
                checkToggleBtn((sender as ToggleButton).Name);
            }
            catch
            {
                MessageBox.Show("맵을 먼저 열어주시기 바랍니다.");
                checkToggleBtn(string.Empty);
            }

        }
        private void RadButton_Click3(object sender, RoutedEventArgs e)
        {
            try
            {
                VIewer3D.Instance.obs_input_event(sender, e, ObsPoint.Kinds.Stairs);
                checkToggleBtn((sender as ToggleButton).Name);
            }
            catch
            {
                MessageBox.Show("맵을 먼저 열어주시기 바랍니다.");
                checkToggleBtn(string.Empty);
            }
        }
        private void RadButton_Click4(object sender, RoutedEventArgs e)
        {
            try
            {
                VIewer3D.Instance.obs_input_event(sender, e, ObsPoint.Kinds.traffic_lights);
                checkToggleBtn((sender as ToggleButton).Name);
            }
            catch
            {
                MessageBox.Show("맵을 먼저 열어주시기 바랍니다.");
                checkToggleBtn(string.Empty);
            }

        }
        private void RadButton_Click5(object sender, RoutedEventArgs e)
        {
            try
            {
                VIewer3D.Instance.obs_input_event(sender, e, ObsPoint.Kinds.trees);
                checkToggleBtn((sender as ToggleButton).Name);
            }
            catch
            {
                MessageBox.Show("맵을 먼저 열어주시기 바랍니다.");
                checkToggleBtn(string.Empty);
            }

        }
        private void RadButton_Click6(object sender, RoutedEventArgs e)
        {
            try
            {
                VIewer3D.Instance.obs_input_event(sender, e, ObsPoint.Kinds.Etc);
                checkToggleBtn((sender as ToggleButton).Name);
            }
            catch
            {
                MessageBox.Show("맵을 먼저 열어주시기 바랍니다.");
                checkToggleBtn(string.Empty);
            }

        }

        public void checkToggleBtn(string name)
        {
            switch (name)
            {
                case "RadButton0":
                    RadButton0.IsChecked = true;
                    RadButton1.IsChecked = false;
                    RadButton2.IsChecked = false;
                    RadButton3.IsChecked = false;
                    RadButton4.IsChecked = false;
                    RadButton5.IsChecked = false;
                    RadButton6.IsChecked = false;
                    break;

                case "RadButton1":
                    RadButton0.IsChecked = false;
                    RadButton1.IsChecked = true;
                    RadButton2.IsChecked = false;
                    RadButton3.IsChecked = false;
                    RadButton4.IsChecked = false;
                    RadButton5.IsChecked = false;
                    RadButton6.IsChecked = false;
                    break;

                case "RadButton2":
                    RadButton0.IsChecked = false;
                    RadButton1.IsChecked = false;
                    RadButton2.IsChecked = true;
                    RadButton3.IsChecked = false;
                    RadButton4.IsChecked = false;
                    RadButton5.IsChecked = false;
                    RadButton6.IsChecked = false;
                    break;

                case "RadButton3":
                    RadButton0.IsChecked = false;
                    RadButton1.IsChecked = false;
                    RadButton2.IsChecked = false;
                    RadButton3.IsChecked = true;
                    RadButton4.IsChecked = false;
                    RadButton5.IsChecked = false;
                    RadButton6.IsChecked = false;
                    break;

                case "RadButton4":
                    RadButton0.IsChecked = false;
                    RadButton1.IsChecked = false;
                    RadButton2.IsChecked = false;
                    RadButton3.IsChecked = false;
                    RadButton4.IsChecked = true;
                    RadButton5.IsChecked = false;
                    RadButton6.IsChecked = false;
                    break;

                case "RadButton5":
                    RadButton0.IsChecked = false;
                    RadButton1.IsChecked = false;
                    RadButton2.IsChecked = false;
                    RadButton3.IsChecked = false;
                    RadButton4.IsChecked = false;
                    RadButton5.IsChecked = true;
                    RadButton6.IsChecked = false;
                    break;

                case "RadButton6":
                    RadButton0.IsChecked = false;
                    RadButton1.IsChecked = false;
                    RadButton2.IsChecked = false;
                    RadButton3.IsChecked = false;
                    RadButton4.IsChecked = false;
                    RadButton5.IsChecked = false;
                    RadButton6.IsChecked = true;
                    break;

                case "":
                    RadButton0.IsChecked = false;
                    RadButton1.IsChecked = false;
                    RadButton2.IsChecked = false;
                    RadButton3.IsChecked = false;
                    RadButton4.IsChecked = false;
                    RadButton5.IsChecked = false;
                    RadButton6.IsChecked = false;
                    break;
            }
        }

        //private void RadButton_Click2(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        VIewer3D.Instance.delete_event(sender, e);
        //    }
        //    catch
        //    {
        //        MessageBox.Show("맵을 먼저 열어주시기 바랍니다.");
        //    }
        //}

        private void savefile_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(Global.projectName))
            {
                MessageBox.Show("Please check the project", "Project Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NavinObsDiagram.Instance.Save();
        }

        private void createYaml_Click(object sender, RoutedEventArgs e)
        {
            saveYaml();
        }

        

        public void saveYaml()
        {

            if(string.IsNullOrEmpty(Global.projectName))
            {
                MessageBox.Show("Please check the project", "Project Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //네비 시작점 확인
            if (!checkStart())
            {
                MessageBox.Show("Please check the starting point.", "Start Point Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //종점 확인
            if (!checkend())
            {
                MessageBox.Show("Please check the end point.", "End Point Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            saveNaviTemp.Clear();

            var startshape = NavinObsDiagram.Instance.Navidiagram.Items.OfType<NaviShape>().Where(x => x.PointType == "0").ToList();
            saveNaviTemp.Add(startshape[0]);
            SetNaviPoistion(startshape[0]);

            //리스트에 시작 끝점이 존재하는지 체크
            if (!checkStartEnd(saveNaviTemp))
            {
                MessageBox.Show("It is not connected from start point to end point.", "End Point Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //저장을 시작한다.
            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog();
            save.Title = "";
            save.Filter = "Yaml file|*.yaml";

            if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (new WaitCursor())
                {
                    string filePath = save.FileName;
                    YamlMapping map = new YamlMapping();

                    //point 갯수
                    map.Add("size_points", saveNaviTemp.Count);

                    //장애물 갯수
                    List<ObsShape> obsdata = NavinObsDiagram.Instance.Navidiagram.Items.OfType<ObsShape>().ToList();
                    map.Add("size_objects", obsdata.Count);

                    //네비포인트 갯수
                    for (int i = 0; i < saveNaviTemp.Count; i++)
                    {
                        //points0_x
                        string Xkey = "points" + i + "_x";
                        string Ykey = "points" + i + "_y";

                        map.Add(Xkey, saveNaviTemp[i].NaviPointX);
                        map.Add(Ykey, saveNaviTemp[i].NaviPointY);
                    }

                    //장애물 갯수
                    for (int i = 0; i < obsdata.Count; i++)
                    {
                        //x y name comment type
                        //objects0_pos_x

                        string Xkey = "objects" + obsdata[i].Index.ToString() + "_pos_x";
                        string Ykey = "objects" + obsdata[i].Index.ToString() + "_pos_y";
                        string NameKey = "objects" + obsdata[i].Index.ToString() + "_name";
                        string commKey = "objects" + obsdata[i].Index.ToString() + "_comment";
                        string typeKey = "objects" + obsdata[i].Index.ToString() + "_type";

                        map.Add(Xkey, obsdata[i].ObsPointX);
                        map.Add(Ykey, obsdata[i].ObsPointY);
                        map.Add(NameKey, Global.getObsCodeName(int.Parse(obsdata[i].PointType)));
                        map.Add(commKey, obsdata[i].Description);
                        map.Add(typeKey, int.Parse(obsdata[i].PointType));
                    }

                    map.Add("wayWidth", float.Parse("2.5000000000000000e+00"));
                    map.Add("nextDirDist", float.Parse("10.00000"));
                    map.Add("nextDirThres", float.Parse("20.00000"));

                    YamlNode node = map;
                    node.ToYamlFile(filePath);

                    MessageBox.Show("yaml file creation is complete.", "Yaml Create.", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        /// <summary>
        /// 시작점과 종점이 있는지 체크한다.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool checkStartEnd(List<NaviShape> list)
        {
            bool returnValue = false;

            //시작점 0, 종점 2 가 각각 1개씩 있으면
            if(list.Where(x=>x.PointType == "0").ToList().Count == 1 
                && list.Where(x => x.PointType == "2").ToList().Count == 1)
                returnValue = true;
            else
                returnValue = false;

            return returnValue;
        }


        public void SetNaviPoistion(NaviShape shape)
        {
            var con = NavinObsDiagram.Instance.Navidiagram.Items.OfType<RadDiagramConnection>().Where(x => x.Source == shape).ToList();
            if(con.Count == 1)
            {
                if (con[0].Target != null)
                {
                    Console.WriteLine((con[0].Target as NaviShape).Index);
                    saveNaviTemp.Add(con[0].Target as NaviShape);
                    SetNaviPoistion(con[0].Target as NaviShape);
                }
            }
        }


        public bool checkStart()
        {
            bool returnValue = false;

            //시작지점 확인
            List<NaviShape> start_navidata = NavinObsDiagram.Instance.Navidiagram.Items.OfType<NaviShape>().Where(x => x.PointType == "0").ToList();
            if (start_navidata.Count == 1)
                returnValue = true;
            else
                returnValue = false;

            return returnValue;
        }

        public bool checkend()
        {
            bool returnValue = false;

            //종점 확인
            List<NaviShape> end_navidata = NavinObsDiagram.Instance.Navidiagram.Items.OfType<NaviShape>().Where(x => x.PointType == "2").ToList();
            if (end_navidata.Count == 1)
                returnValue = true;
            else
                returnValue = false;

            return returnValue;
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Global.projectName))
            {
                if (MessageBox.Show("Initialize project contents Are you sure you want to initialize?", "Initialize project", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                {
                    //3D
                    VIewer3D.Instance.ClearData(false);
                    //다이어그램 부분
                    NavinObsDiagram.Instance.Navidiagram.Clear();
                    NavinObsDiagram.Instance.ObsDiagram.Clear();
                    NavinObsDiagram.Instance.Save();
                }
            }
        }
    }
}