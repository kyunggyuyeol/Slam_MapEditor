using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Slam_MapEditor.View
{
    /// <summary>
    /// Interaction logic for SolutionExplorer.xaml
    /// </summary>
    public partial class SolutionExplorer : UserControl
    {
        public static SolutionExplorer Instance { get; set; }

        public SolutionExplorer()
        {
            InitializeComponent();
            Instance = this;
        }

        private object dummyNode = null;
        public FileSystemWatcher filedect;
        private TreeViewItem mSelectedItem = null;

        /// <summary>
        /// 선택된아이템에 따라 우클릭 메뉴 이벤트를 출력한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void foldersItem_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            //선택된 트리 임시 저장
            TreeView tree = (TreeView)sender;
            mSelectedItem = ((TreeViewItem)tree.SelectedItem);

            tree.ContextMenu = null;

            if (mSelectedItem == null)
                return;

            string filepath = mSelectedItem.Tag.ToString();
            if ((filepath.ToLower().Contains(@".map") == false && (filepath.ToLower().Contains("mapimagedata") == false)))
            {
                tree.ContextMenu = tree.Resources["FolderContext"] as System.Windows.Controls.ContextMenu;
            }


        }

        /// <summary>
        /// 마우스 오른쪽 버튼이 클릭 됐을 때 sender가 TreeViewItem이 아닌경우 컨텍스트 메뉴가 출력되지 않도록 무시 한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item != null)
            {
                item.IsSelected = true;
                e.Handled = true;
            }
        }

        /// <summary>
        /// 프로젝트명 확인해서 트리뷰를 확장해줌
        /// </summary>
        public void openSelectProject()
        {
            for (int i = 0; i < foldersItem.Items.Count; i++)
            {
                var a = foldersItem.Items[i] as TreeViewItem;

                if (a.Header.ToString() == Global.projectName)
                {
                    a.IsExpanded = true;
                    break;
                }
            }

        }

        /// <summary>
        /// 맵 패스를 확인하여 프로젝트를 불러온다.
        /// </summary>
        /// <param name="mapPath"></param>
        public void projectOpen(string mapPath)
        {
            //트리뷰 다시 그려주고
            reSetTreeview();

            //프로젝트 이름 확인
            Global.projectName = mapPath.Replace(Global.projectPath, "").Replace(Path.GetFileName(mapPath), "").Replace(@"\", "");

            //해서 프로젝트명 폴더만 열어줌
            openSelectProject();

            MainWindow mainWindow = ((MainWindow)Application.Current.MainWindow);
            MainWindowViewModel viewModel = mainWindow.DataContext as MainWindowViewModel;
            if (viewModel != null)
            {
                //기존에 열린 뷰어 확인
                VIewer3D viewer = mainWindow.FindView(typeof(VIewer3D)) as VIewer3D;

                if (viewer == null)
                    viewModel.Panes.Add(new PaneViewModel(typeof(VIewer3D)) { Header = "Map Viewer", IsDocument = true });

                List<Model.Tile> listdata = new List<Model.Tile>();

                BinaryReader reader = new BinaryReader(File.Open(mapPath, FileMode.Open));
                float width = reader.ReadSingle();
                int size = reader.ReadInt32();

                Model.Tile.Width = width;
                Model.Tile.Size = size;

                for (int i = 0; i < size; i++)
                {
                    float X = reader.ReadSingle();
                    float Y = reader.ReadSingle();
                    byte R = (byte)(reader.ReadSingle() * 255);
                    byte G = (byte)(reader.ReadSingle() * 255);
                    byte B = (byte)(reader.ReadSingle() * 255);

                    listdata.Add(new Model.Tile(i, X, Y, R, G, B));
                }

                reader.Close();

                //3dViewer 창 가져와서
                viewer = mainWindow.FindView(typeof(VIewer3D)) as VIewer3D;
                //데이터 넣어주고
                viewer.MapData = listdata;

                //맵 그려줌
                viewer.startDrawMapData();
                //스크롤뷰 초기화, 텍스트박스 초기화
                ImageMap.instance.slider.Value = 0;
                ImageMap.instance.textBox.Clear();



                //네비 포인트    
                //네비, 장애물 다이어그램 파일 존재 할 경우 다이어그램에 출력함
                NavinObsDiagram.Instance.Navidiagram.SelectedItem = null;
                NavinObsDiagram.Instance.ObsDiagram.SelectedItem = null;

                NavinObsDiagram.Instance.Navidiagram.Clear();
                NavinObsDiagram.Instance.ObsDiagram.Clear();
                NavinObsDiagram.Instance.removeItemChangeEvent();
                NavinObsDiagram.Instance.loadTime = true;
                if (File.Exists(Global.projectNaviDiagram))
                {
                    FileStream fs = new FileStream(Global.projectNaviDiagram, FileMode.Open);
                    StreamReader NaviData = new StreamReader(fs);
                    string serializedString = NaviData.ReadToEnd();
                    NavinObsDiagram.Instance.Navidiagram.Load(serializedString);
                    fs.Close();
                    NaviData.Close();
                }
                if (File.Exists(Global.projectObsDiagram))
                {
                    FileStream fs = new FileStream(Global.projectObsDiagram, FileMode.Open);
                    StreamReader ObsData = new StreamReader(fs);
                    string serializedString = ObsData.ReadToEnd();
                    NavinObsDiagram.Instance.ObsDiagram.Load(serializedString);
                    fs.Close();
                    ObsData.Close();

                    NavinObsDiagram.Instance.SetObsDelegate();
                }
                NavinObsDiagram.Instance.loadTime = false;
                NavinObsDiagram.Instance.additemChangeEvent();
                NavinObsDiagram.Instance.checkedPoint();

                //작은 원 발생시 크기 변경
                NavinObsDiagram.Instance.checkShapeClick();

                //diagrma 확인해서 3D에 점 찍어줌
                var Navi = NavinObsDiagram.Instance.getNaviShape();
                  VIewer3D.Instance.ClearData(false);
                //for(int i = 0; i<Navi.Count-1; i++)
                //{
                //    VIewer3D.Instance.AddNavi(Navi[i].NaviPointX, Navi[i].NaviPointY, false);
                //}

                foreach (var data in Navi)
                {
                    VIewer3D.Instance.AddNavi((float)data.NaviPointX, (float)data.NaviPointY, false);
                }
                var Obs = NavinObsDiagram.Instance.getObsShape();
                foreach (var data in Obs)
                {
                    VIewer3D.Instance.AddObs((float)data.ObsPointX, (float)data.ObsPointY, int.Parse(data.PointType), false);
                }
                VIewer3D.Instance.DrawLinesOnTimeline();

                //이미지패스확인
                if (Directory.Exists(Global.ImageDataPath))
                {
                    ImageMap.instance.fileNames.Clear();
                    ImageMap.instance.ListViewfileNames.Clear();
                    ImageMap.instance.textBox.Clear();
                    ImageMap.instance.slider.Value = 0;
                    DirectoryInfo dir = new DirectoryInfo(Global.ImageDataPath);

                    //        OpenFileDialog ofd = new OpenFileDialog(){ Multiselect = true, ValidateNames = true, Filter = "PNG|*.png" };

                    foreach (FileInfo item in dir.GetFiles())
                    {
                        //item.Name
                        if (item.Extension == ".jpg")
                        {
                            string fullpath = Path.Combine(Global.ImageDataPath, item.Name);

                            ImageMap.instance.fileNames.Add(fullpath);
                            ImageMap.instance.ListViewfileNames.Add(item.Name);
                        }

                    }

                    if (ImageMap.instance.imageshow == null)
                        ImageMap.instance.imageshow = new Image();


                    if (ImageMap.instance.fileNames.Count == 0)
                    {
                        MessageBox.Show("Jpg files do not exist in current folder", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.UriSource = new Uri(ImageMap.instance.fileNames[0]);
                        bitmap.EndInit();
                        ImageMap.instance.imageshow.Source = bitmap;

                        //ImageMap.instance.imageshow.Source = new BitmapImage(new Uri(ImageMap.instance.fileNames[0]));
                        ImageMap.instance.listView.ItemsSource = ImageMap.instance.ListViewfileNames;
                        ImageMap.instance.getimagefile();
                    }

                }
            }
        }

        /// <summary>
        /// 트리뷰에서 마우스를 더블클릭 했을때 발생하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void foldersItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TreeView tree = (TreeView)sender;
            TreeViewItem temp = (TreeViewItem)tree.SelectedItem;

            NavinObsDiagram.Instance.Navidiagram.SelectedItem = null;
            NavinObsDiagram.Instance.ObsDiagram.SelectedItem = null;

            if (temp == null) return;

            //맵 파일이면 프로젝트를 열어줌
            if (temp.Tag.ToString().Contains(@".map"))
            {
                string filepath = temp.Tag.ToString();
                projectOpen(filepath);
            }
        }

        /// <summary>
        /// 트리뷰를 새로 그린다.
        /// </summary>
        public void reSetTreeview()
        {
            Dispatcher.Invoke(new Action(delegate ()
            {
                //트리뷰 실행
                foldersItem.Items.Clear();

                string folderimage = "pack://application:,,,/Images/icon/folder.png";

                Image img = new Image();
                img.Source = new BitmapImage(new Uri(folderimage));

                foreach (string pathName in Directory.GetDirectories(Global.projectPath))
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Padding = new Thickness(0, 5, 0, 5);
                    item.Header = pathName.Substring(pathName.LastIndexOf("\\") + 1); ;
                    item.Tag = pathName;

                    item.FontWeight = FontWeights.Normal;
                    item.Items.Add(dummyNode);
                    item.Expanded += new RoutedEventHandler(folder_Expanded);
                    foldersItem.Items.Add(item);
                }
            }));
        }

        /// <summary>
        /// 프로젝트 폴더가 확장 되었을때 하위 값을 추가해준다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void folder_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == dummyNode)
            {
                item.Items.Clear();
                try
                {
                    foreach (string s in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
                        subitem.Tag = s;
                        subitem.FontWeight = FontWeights.Normal;
                        //subitem.Items.Add(dummyNode);
                        //subitem.Expanded += new RoutedEventHandler(folder_Expanded);
                        item.Items.Add(subitem);
                    }

                    foreach (string s in Directory.GetFiles(item.Tag.ToString()))
                    {
                        if (s.Contains(@".xml"))
                            continue;

                        TreeViewItem subitem = new TreeViewItem();
                        subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
                        subitem.Tag = s;
                        subitem.FontWeight = FontWeights.Normal;
                        item.Items.Add(subitem);
                    }
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// 솔루션이 로드 했을때 파일을 감시하여 트리를 그려주어야 한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            reSetTreeview();

            //폴더 감시 실행
            filedect = new FileSystemWatcher();
            filedect.Path = Global.projectPath;
            filedect.NotifyFilter = NotifyFilters.DirectoryName;
            filedect.Created += Filedect_Created;
            filedect.Deleted += Filedect_Deleted;
            filedect.EnableRaisingEvents = true;
        }

        /// <summary>
        /// 파일이 삭제 되었을 경우 트리를 다시 그려준다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Filedect_Deleted(object sender, FileSystemEventArgs e)
        {
            reSetTreeview();
        }

        /// <summary>
        /// 파일이 생성되었을 경우 트리를 다시 그려준다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Filedect_Created(object sender, FileSystemEventArgs e)
        {
            reSetTreeview();
        }

        /// <summary>
        /// 프로젝트를 삭제한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteProject_Click(object sender, RoutedEventArgs e)
        {
            //선택한 프로젝트를 삭제한다.
            string filepath = mSelectedItem.Tag.ToString();
            string headerName = mSelectedItem.Header.ToString();

            //여부 물어봄
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete project " + headerName + " ?", "Delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                ImageMap.instance.listView.ItemsSource = null;

                ImageMap.instance.textBox.Clear();
                ImageMap.instance.slider.Value = 0;
                ImageMap.instance.imageshow.Source = null;
                //죄다 삭제
                if (VIewer3D.Instance != null)
                    VIewer3D.Instance.ClearData();

                Directory.Delete(filepath, true);

                //diagram 클리어
                NavinObsDiagram.Instance.Navidiagram.Clear();
                NavinObsDiagram.Instance.ObsDiagram.Clear();
            }
        }

        /// <summary>
        /// yaml 파일을 생성한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Get_NaviFile_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.saveYaml();
        }

        /// <summary>
        /// 마우스 들어올때 다이어그램 부분 선택 빠지도록 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void foldersItem_MouseEnter(object sender, MouseEventArgs e)
        {
            NavinObsDiagram.Instance.Navidiagram.SelectedItem = null;
            NavinObsDiagram.Instance.ObsDiagram.SelectedItem = null;
        }

        /// <summary>
        /// 마우스 나갈때 다이어그램 부분 선택 빠지도록 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void foldersItem_MouseLeave(object sender, MouseEventArgs e)
        {
            NavinObsDiagram.Instance.Navidiagram.SelectedItem = null;
            NavinObsDiagram.Instance.ObsDiagram.SelectedItem = null;
        }
    }
}