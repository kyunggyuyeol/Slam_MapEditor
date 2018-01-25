using Slam_MapEditor.Shape;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Diagrams;
using Telerik.Windows.Diagrams.Core;

namespace Slam_MapEditor.View
{
    /// <summary>
    /// NaninObs.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NavinObsDiagram : UserControl
    {
        public static NavinObsDiagram Instance { get; set; }

        //네비 시작 포인트
        int createNaviX = 0;
        int createNaviY = 0;
        //장애물 시작 포인트
        int createObsX = 0;
        int createObsY = 0;

        public bool loadTime = false;

        public NavinObsDiagram()
        {
            InitializeComponent();

            Instance = this;

            //네비다이어그램 삭제시 이벤트
            CommandBinding binding = new CommandBinding()
            {
                Command = DiagramCommands.Delete
            };
            this.Navidiagram.CommandBindings.Add(binding);
            binding.Executed += NaviDeleteCommandExecuted;

            //장애물다이어그램 삭제시 이벤트
            CommandBinding Obsbinding = new CommandBinding()
            {
                Command = DiagramCommands.Delete
            };
            this.ObsDiagram.CommandBindings.Add(Obsbinding);
            Obsbinding.Executed += ObsDeleteCommandExecuted;
        }

        /// <summary>
        /// 저장 되어있던 클래스의 델리게이트를 다시 연결해준다.
        /// </summary>
        public void SetObsDelegate()
        {
            var ObsShapes = ObsDiagram.Items.OfType<ObsShape>().ToList();
            foreach (ObsShape item in ObsShapes)
            {
                item.chitem += Shape_chitem;
            }
        }

        /// <summary>
        /// 경로, 장애물 추가시 발생하는 이벤트를 추가한다.
        /// </summary>
        public void additemChangeEvent()
        {
            Navidiagram.Items.CollectionChanged += Items_CollectionChanged;
            ObsDiagram.Items.CollectionChanged += Items_CollectionChanged;
        }

        /// <summary>
        /// 경로, 장애물 추가시 발생하는 이벤트를 삭제한다.
        /// </summary>
        public void removeItemChangeEvent()
        {
            Navidiagram.Items.CollectionChanged -= Items_CollectionChanged;
            ObsDiagram.Items.CollectionChanged -= Items_CollectionChanged;
        }

        /// <summary>
        /// 경로, 장애물이 추가될때 발생하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                //if (e.NewItems[0] is NaviShape)
                //{
                //    Save();
                //}
                //else if (e.NewItems[0] is ObsShape)
                //{
                //    Save();
                //}
                //3D 선 다시 그리기 이벤트
                reConnection();
            }
        }

        /// <summary>
        /// 기존에 있는 아이템의 포지션을 확인하여 다음 줄에 추가 할 수 있도록 포지션 조정
        /// </summary>
        public void checkedPoint()
        {
            //네비 시작 포지션 변경
            var navipoint = Navidiagram.Items.OfType<NaviShape>().OrderByDescending(x => x.Y).ToList();
            if (navipoint.Count > 0)
            {
                createNaviX = 0;
                createNaviY = int.Parse(navipoint[0].Position.Y.ToString()) + 120;
            }
            else
            {
                createNaviX = 0;
                createNaviY = 0;
            }

            //장애물 시작 포지션 변경
            var Obspoint = ObsDiagram.Items.OfType<ObsShape>().OrderByDescending(x => x.Y).ToList();
            if (Obspoint.Count > 0)
            {
                createObsX = 0;
                createObsY = int.Parse(Obspoint[0].Position.Y.ToString()) + 120;
            }
            else
            {
                createObsX = 0;
                createObsY = 0;
            }
        }

        /// <summary>
        /// 경로 다이어그램 모델 삭제시 발생하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NaviDeleteCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            List<NaviShape> deldatas = new List<NaviShape>();
            List<RadDiagramConnection> delcons = new List<RadDiagramConnection>();

            //삭제 shape 파악
            foreach (var item in Navidiagram.SelectedItems)
            {
                //NaviShape 가 삭제 되면 3D 부분에서도 삭제해야함.
                if (item is NaviShape)
                {
                    NaviShape delshape = item as NaviShape;
                    //VIewer3D.Instance.NaviPointGridRemove(delshape.NaviPointX, delshape.NaviPointY);

                    deldatas.Add(delshape);
                }
                else if (item is RadDiagramConnection)
                {
                    RadDiagramConnection delitem = item as RadDiagramConnection;
                    delcons.Add(delitem);
                }
            }

            if (deldatas.Count > 0)
                VIewer3D.Instance.RemoveNaviPoints();

            for (int i = 0; i < delcons.Count; i++)
            {
                Navidiagram.RemoveConnection(delcons[i]);
            }

            for (int i = 0; i < deldatas.Count; i++)
            {
                //삭제되는 shape에 connection 이 있는지 확인한다. 
                //있는경우는 전부 삭제한다.
                var srcCon = Navidiagram.Items.OfType<RadDiagramConnection>().Where(x => x.Source == deldatas[i]).ToList();
                if (srcCon.Count > 0)
                {
                    foreach (RadDiagramConnection item in srcCon)
                    {
                        Navidiagram.RemoveConnection(item);
                    }
                }
                var dstCon = Navidiagram.Items.OfType<RadDiagramConnection>().Where(x => x.Target == deldatas[i]).ToList();
                if (dstCon.Count > 0)
                {
                    foreach (RadDiagramConnection item in dstCon)
                    {
                        Navidiagram.RemoveConnection(item);
                    }
                }
                Navidiagram.RemoveShape(deldatas[i]);
            }

            reConnection();

            if (loadTime == false)
            {
                Save();
            }


            resetNodeNumber();
        }

        /// <summary>
        /// 네비 모델들을 list로 리턴한다.
        /// </summary>
        /// <returns></returns>
        public List<NaviShape> getNaviShape()
        {
            return Navidiagram.Items.OfType<NaviShape>().OrderBy(x=>x.Index).ToList();
        }

        /// <summary>
        /// 장애물 모델들을 list로 리턴한다.
        /// </summary>
        /// <returns></returns>
        public List<ObsShape> getObsShape()
        {
            return ObsDiagram.Items.OfType<ObsShape>().OrderBy(x => x.Index).ToList();
        }

        /// <summary>
        /// 경로 다이어그램에서 모델을 추가한다.
        /// </summary>
        /// <param name="pointX"></param>
        /// <param name="pointY"></param>
        /// <param name="index"></param>
        public void add_NaviePoint(float pointX, float pointY, int index)
        {
            //탭 네비쪽으로 전환하고
            tabCon.SelectedIndex = 0;

            //인덱스 번호를 찾는다 가장 큰 번호에 +1을 한다.
            //int index = 0;
            //var getindex = Navidiagram.Items.OfType<NaviShape>().OrderByDescending(x => x.Index).ToList();
            //if (getindex.Count == 0)
            //    index = 0;
            //else
            //    index = getindex[0].Index + 1;

            //다이어그램에 포인트 추가
            Navidiagram.AddShape(new NaviShape() { Position = new Point(createNaviX, createNaviY), Index = index, PointType = "Nomal", NaviPointX = pointX, NaviPointY = pointY });

            createNaviX += 150;

            if (createNaviX == 900)
            {
                createNaviX = 0;
                createNaviY += 120;
            }

            reConnection();
        }

        /// <summary>
        /// 경로 다이어그램 휠 클릭시 클릭 지점을 센터로 두고 이동 함
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NaviDiagram_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                //클릭 위치 확인 하여 클릭 시점 중앙으로 위치 이동
                Point ClickPoint = (this.Navidiagram as RadDiagram).GetTransformedPoint(e.GetPosition(this.Navidiagram as UIElement));
                double width = Navidiagram.Viewport.Width / 2;
                double heigh = Navidiagram.Viewport.Height / 2;
                Point setPoistion = new System.Windows.Point(ClickPoint.X - width, ClickPoint.Y - heigh);
                Navidiagram.BringIntoView(setPoistion, Navidiagram.Zoom, true);
            }
        }

        /// <summary>
        /// 네비 다이어그램에서 모델 선택시 발생하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NaviDiagram_ShapeClicked(object sender, ShapeRoutedEventArgs e)
        {
            var shape = e.Shape as NaviShape;

            if (shape != null)
            {
                //shape.Index
                VIewer3D.Instance.ActivateNaviPoints((float)shape.NaviPointX, (float)shape.NaviPointY);
                PropertiesPane.Instance.settingProperty("navi");
                PropertiesPane.Instance.property_navi.Item = shape;
            }
        }

        /// <summary>
        /// 경로 다이어그램에서 선택시 발생하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NaviDiagram_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectitem = Navidiagram.SelectedItem;

            if (selectitem == null)
                PropertiesPane.Instance.hidePropertypane();

            if (loadTime == false)
            {
                Save();
            }
        }

        private void NaviDiagram_ConnectionManipulationCompleted(object sender, ManipulationRoutedEventArgs e)
        {
            IShape source = null;
            IShape target = null;

            try
            {
                source = e.Connection.Source;
                target = e.Connector.Shape;

                RadDiagramConnection con = e.Connection as RadDiagramConnection;
                con.TargetCapType = CapType.Arrow2Filled;
                con.Stroke = new SolidColorBrush(Colors.Black);
                con.StrokeThickness = 2;
                con.Foreground = new SolidColorBrush(Colors.Black);
                con.IsEditable = false;
            }
            catch
            {
            }

            //소스가 null 이면 삭제
            if (source == null)
            {
                e.Handled = true;
                return;
            }

            //타겟이 null 이면 삭제
            if (target == null)
            {
                e.Handled = true;
                return;
            }

            //같은 포트에 연결 하였다면 삭제
            if (source == target)
            {
                e.Handled = true;
                return;
            }

            //같은 지점을 target으로 잡으면 안된다.
            var checkdata = Navidiagram.Items.OfType<RadDiagramConnection>().Where(x => x.Target == target).ToList();
            if (checkdata.Count >= 1)
            {
                e.Handled = true;
                return;
            }

            //목적지에서 다음 경로로 연결 할 수 없다.
            if (source is NaviShape)
            {
                NaviShape check = source as NaviShape;

                if (check.PointType == "2")
                {
                    e.Handled = true;
                    return;
                }
            }

            //시작지점이 중간 경로가 될 수 없다.
            if (target is NaviShape)
            {
                NaviShape check = target as NaviShape;

                if (check.PointType == "0")
                {
                    e.Handled = true;
                    return;
                }
            }

            //시작지점에서 커넥션이 1개만 있어야 한다.
            if (source is NaviShape)
            {
                NaviShape check = source as NaviShape;
                var cons = Navidiagram.Items.OfType<RadDiagramConnection>().Where(x => x.Source == check).ToList();
                if (cons.Count == 1)
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        /// <summary>
        /// 커넥션을 다시 그려준다. autofit 포함
        /// </summary>
        public void reConnection()
        {
            //Navidiagram.BeginInit();
            ////커넥션 전체 삭제
            //var delcon = Navidiagram.Items.OfType<RadDiagramConnection>().ToList();
            //foreach (RadDiagramConnection item in delcon)
            //{
            //    Navidiagram.RemoveConnection(item);
            //}


            //var shapes = Navidiagram.Items.OfType<NaviShape>().ToList();
            //if (shapes.Count >= 2)
            //{
            //    for (int i = 0; i < shapes.Count - 1; i++)
            //    {
            //        shapes[i].PointType = "1";

            //        if (i == 0)
            //        {
            //            shapes[i].PointType = "0";
            //        }

            //        RadDiagramConnection con = new RadDiagramConnection();
            //        con.Source = shapes[i];
            //        con.Target = shapes[i + 1];
            //        con.TargetCapType = CapType.Arrow2Filled;
            //        con.Stroke = new SolidColorBrush(Colors.Black);
            //        con.StrokeThickness = 2;
            //        con.IsEditable = false;

            //        Navidiagram.AddConnection(con);

            //        if (i + 1 == shapes.Count - 1)
            //        {
            //            shapes[i + 1].PointType = "2";
            //        }
            //    }
            //}
            //Navidiagram.EndInit();
            Navidiagram.AutoFit();

            VIewer3D.Instance.navi.Children.Clear();
            VIewer3D.Instance.DrawLinesOnTimeline();
        }

        /// <summary>
        /// 경로 포인트가 삭제시 경로포인트도 삭제해준다.
        /// </summary>
        /// <param name="xPoint"></param>
        /// <param name="yPoint"></param>
        public void deleteNaviShape(float xPoint, float yPoint)
        {
            var navi = Navidiagram.Items.OfType<NaviShape>().Where(x => x.NaviPointX == xPoint && x.NaviPointY == yPoint).ToList();
            if (navi.Count == 1)
            {
                Navidiagram.RemoveShape(navi[0]);
            }
            reConnection();
        }

        /// <summary>
        /// 3D 에서 장애물 삭제시 장애물 다이어그램에서도 삭제한다.
        /// </summary>
        /// <param name="xPoint"></param>
        /// <param name="yPoint"></param>
        public void deleteObsShape(double xPoint, double yPoint)
        {
            var Obs = ObsDiagram.Items.OfType<ObsShape>().Where(x => x.ObsPointX == xPoint && x.ObsPointY == yPoint).ToList();
            if (Obs.Count == 1)
            {
                ObsDiagram.RemoveShape(Obs[0]);
            }
            reConnection();
        }

        /// <summary>
        /// 경로, 장애물 다이어그램을 저장한다.
        /// </summary>
        public void Save()
        {
            writeDiagram(Navidiagram.Save(), Global.projectNaviDiagram);
            writeDiagram(ObsDiagram.Save(), Global.projectObsDiagram);
        }

        /// <summary>
        /// 다이어그램을 저장한다. xml 줄을 맞추기 위해 다시 불러 저장함
        /// </summary>
        /// <param name="writeString"></param>
        /// <param name="filepath"></param>
        public void writeDiagram(string writeString, string filepath)
        {
            try
            {
                //다이어그램을 저장함
                FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write);
                StreamWriter file = new StreamWriter(fs);
                file.WriteLine(writeString);
                file.Flush();
                file.Close();
                fs.Close();

                //줄 변경을 위해 불럿다 다시 저장
                XmlTextReader dataReader = new XmlTextReader(filepath);
                XmlDocument Obsdoc = new XmlDocument();
                Obsdoc.PreserveWhitespace = false;
                Obsdoc.Load(dataReader);
                dataReader.Close();
                Obsdoc.Save(filepath);
            }
            catch
            {

            }
        }

        /// <summary>
        /// 마우스 휠 클릭시 클릭한 시점을 중앙으로 이동해준다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ObsDiagram_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                //클릭 위치 확인 하여 클릭 시점 중앙으로 위치 이동
                Point ClickPoint = (this.ObsDiagram as RadDiagram).GetTransformedPoint(e.GetPosition(this.ObsDiagram as UIElement));
                double width = ObsDiagram.Viewport.Width / 2;
                double heigh = ObsDiagram.Viewport.Height / 2;
                Point setPoistion = new System.Windows.Point(ClickPoint.X - width, ClickPoint.Y - heigh);
                ObsDiagram.BringIntoView(setPoistion, ObsDiagram.Zoom, true);
            }
        }

        /// <summary>
        /// 장애물 다이어그램에서 클릭이 발생할때 동작하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ObsDiagram_ShapeClicked(object sender, ShapeRoutedEventArgs e)
        {
            var shape = e.Shape as ObsShape;

            if (shape != null)
            {
                VIewer3D.Instance.ActivateObsPoints((float)shape.ObsPointX, (float)shape.ObsPointY, int.Parse(shape.PointType));
                PropertiesPane.Instance.settingProperty("");
                PropertiesPane.Instance.property_obs.Item = shape;
            }
        }

        /// <summary>
        /// 장애물 다이어그램이 선택이 변경 될 경우 발생하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ObsDiagram_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectitem = ObsDiagram.SelectedItem;

            if (selectitem == null)
                PropertiesPane.Instance.hidePropertypane();

            if (loadTime == false)
            {
                Save();
            }
        }

        /// <summary>
        /// 장애물 노드를 추가해준다.
        /// </summary>
        /// <param name="pointX"></param>
        /// <param name="pointY"></param>
        /// <param name="pointType"></param>
        public void add_ObsPoint(float pointX, float pointY, int pointType)
        {

            //탭 장애물로 전환해주고
            tabCon.SelectedIndex = 1;
            //인덱스 번호를 찾는다 가장 큰 번호에 +1을 한다.
            int index = 0;
            var getindex = ObsDiagram.Items.OfType<ObsShape>().OrderByDescending(x => x.Index).ToList();
            if (getindex.Count == 0)
                index = 0;
            else
                index = getindex[0].Index + 1;

            //다이어그램에 포인트 추가

            var shape = new ObsShape() { Position = new Point(createObsX, createObsY), Index = index, PointType = pointType.ToString(), ObsPointX = pointX, ObsPointY = pointY, _oldPointtype = pointType.ToString() };
            shape.chitem += Shape_chitem;
            ObsDiagram.AddShape(shape);

            createObsX += 150;

            if (createObsX == 900)
            {
                createObsX = 0;
                createObsY += 120;
            }

            ObsDiagram.AutoFit();
        }

        /// <summary>
        /// 장애물의 상태가 변경되면 발생하는 이벤트 3D에 결과(색상)을 변경해준다.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="newCode"></param>
        /// <param name="oldCode"></param>
        private void Shape_chitem(ObsShape item, string newCode, string oldCode)
        {
            VIewer3D.Instance.ChangeObs((float)item.ObsPointX, (float)item.ObsPointY, int.Parse(newCode), int.Parse(oldCode));
            VIewer3D.Instance.ActivateObsPoints((float)item.ObsPointX, (float)item.ObsPointY, int.Parse(newCode));
        }

        /// <summary>
        /// 장애물이 삭제될 경우 발생하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ObsDeleteCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            List<ObsShape> deldata = new List<ObsShape>();

            foreach (var item in ObsDiagram.SelectedItems)
            {
                if (item is ObsShape)
                {
                    ObsShape del = item as ObsShape;
                    deldata.Add(del);
                }
            }

            VIewer3D.Instance.RemoveObsPoints();

            for (int i = 0; i < deldata.Count; i++)
            {
                ObsDiagram.RemoveShape(deldata[i]);
            }

            if (loadTime == false)
            {
                Save();
            }

        }

        //임시 저장되는 포지션과 인덱스
        double create_newPositionX = 0;
        double create_newPositionY = 0;
        int create_newIndex = 0;

        //임시저장되는 커넥션
        RadDiagramConnection src;
        RadDiagramConnection dst;

        //포인트 타입
        string pointtype;

        /// <summary>
        /// 이동하게 되는 노드의 정보를 임시 저장후 삭제한다.
        /// </summary>
        /// <param name="index"></param>
        public void tempMove_NaviPosition(int index)
        {
            //포지션을 이동했을 때 포지션을 새로 저장한다.
            var searchShape = Navidiagram.Items.OfType<NaviShape>().Where(x => x.Index == index).ToList();
            if (searchShape.Count == 1)
            {
                var srcCon = Navidiagram.Items.OfType<RadDiagramConnection>().Where(x => x.Target == searchShape[0]).ToList();
                if (srcCon.Count == 1)
                    src = srcCon[0];
                else
                    src = null;

                var dstCon = Navidiagram.Items.OfType<RadDiagramConnection>().Where(x => x.Source == searchShape[0]).ToList();
                if (dstCon.Count == 1)
                    dst = dstCon[0];
                else
                    dst = null;

                pointtype = searchShape[0].PointType;
                create_newIndex = searchShape[0].Index;
                create_newPositionX = searchShape[0].Position.X;
                create_newPositionY = searchShape[0].Position.Y;
                Navidiagram.RemoveShape(searchShape[0]);
            }
        }


        /// <summary>
        /// 3D 맵에서 경로를 이동시킬때 노드를 새로 그려준다.
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="index"></param>
        public void move_NaviShape(float X, float Y, int index)
        {
            //탭 네비쪽으로 전환하고
            tabCon.SelectedIndex = 0;

            //다이어그램에 포인트 추가
            NaviShape addsShape = new NaviShape() { Position = new Point(create_newPositionX, create_newPositionY), Index = index, PointType = "Nomal", NaviPointX = X, NaviPointY = Y };


            if (src != null)
                src.Target = addsShape;

            if (dst != null)
                dst.Source = addsShape;


            Navidiagram.AddShape(addsShape);
            Navidiagram.AutoFit();
        }

        /// <summary>
        /// 연결되지 않은 커넥션을 삭제해준다.
        /// </summary>
        public void checkConnectionState()
        {
            var cons = Navidiagram.Items.OfType<RadDiagramConnection>().Where(x => x.Source == null || x.Target == null).ToList();
            foreach (RadDiagramConnection item in cons)
            {
                Navidiagram.RemoveConnection(item);
            }
        }

        /// <summary>
        /// 노드를 기본사이즈로 정렬해준다. 일부 사이즈가 작은 노드가 발생함
        /// </summary>
        public void checkShapeClick()
        {
            var shapes = Navidiagram.Items.OfType<NaviShape>().ToList();
            foreach (NaviShape item in shapes)
            {
                item.Width = 30;
                item.Height = 30;
            }
        }

        /// <summary>
        /// 경로의 프로퍼티를 출력해준다.
        /// </summary>
        /// <param name="index"></param>
        public void setShowNaviProperty(float positionX, float positionY)
        {
            var shape = Navidiagram.Items.OfType<NaviShape>().Where(x => x.NaviPointX == positionX && x.NaviPointY == positionY).ToList();
            if (shape.Count == 1)
            {
                tabCon.SelectedIndex = 0;
                setShapeDefultColor();

                shape[0].StrokeColor = new SolidColorBrush(Colors.Crimson);

                PropertiesPane.Instance.settingProperty("navi");
                PropertiesPane.Instance.property_navi.Item = shape[0];
            }
        }

        /// <summary>
        /// 장애물의 프로퍼티를 줄력해준다.
        /// </summary>
        /// <param name="positionX">3D 맵상의 장애물 노드의 X 좌표</param>
        /// <param name="positionY">3D 맵상의 장애물 노드의 Y 좌표</param>
        public void setShowObsProperty(float positionX, float positionY)
        {
            var shape = ObsDiagram.Items.OfType<ObsShape>().Where(x => x.ObsPointX == positionX && x.ObsPointY == positionY).ToList();
            if (shape.Count == 1)
            {
                tabCon.SelectedIndex = 1;
                setShapeDefultColor();

                shape[0].BorderBrushstroke = new SolidColorBrush(Colors.Crimson);

                PropertiesPane.Instance.settingProperty("");
                PropertiesPane.Instance.property_obs.Item = shape[0];
            }
        }

        /// <summary>
        /// 네비, 장애물의 모든 노드들을 기본 테두리색상으로 변경한다.
        /// </summary>
        public void setShapeDefultColor()
        {
            var Navilist = Navidiagram.Items.OfType<NaviShape>().ToList();
            foreach (NaviShape item in Navilist)
            {
                item.StrokeColor = new SolidColorBrush(Colors.Transparent);
            }

            var Obslist = ObsDiagram.Items.OfType<ObsShape>().ToList();
            foreach (ObsShape item in Obslist)
            {
                item.BorderBrushstroke = new SolidColorBrush(Colors.Transparent);
            }
        }

        /// <summary>
        /// 노드 삭제시 노드 넘버 다시 재정렬 해줌
        /// </summary>
        public void resetNodeNumber()
        {
            var list = Navidiagram.Items.OfType<NaviShape>().OrderBy(x => x.Index).ToList();
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Index = i;
            }
        }

    }
}
