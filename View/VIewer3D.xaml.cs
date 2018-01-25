using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Slam_MapEditor.Model;
using HelixToolkit.Wpf;
using Slam_MapEditor.Shape;
using System;

namespace Slam_MapEditor.View
{
    /// <summary>
    /// VIewer3D.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class VIewer3D : UserControl
    {
        #region Field

        private GeometryModel3D NaviSelectedModel;
        private GeometryModel3D ChangedModel, CrossSelectedModel, GateSelectedModel, StairsSelectedModel, TrafficSelectedModel, TreesSelectedModel, EtcSelectedModel;

        private DiffuseMaterial RemovedMaterial;
        private DiffuseMaterial NaviMaterial = new DiffuseMaterial(Brushes.DodgerBlue);

        private DiffuseMaterial CrossMateiral;
        private DiffuseMaterial GateMaterial;
        private DiffuseMaterial StairsMaterial;
        private DiffuseMaterial TrafficMaterrial;
        private DiffuseMaterial TreesMaterial;
        private DiffuseMaterial EtcMaterial;
        private DiffuseMaterial NaviDisMaterial;


        private DiffuseMaterial NaviSelectedMaterial = new DiffuseMaterial(Brushes.Aqua);
        private DiffuseMaterial CrossSelectedMaterial, GateSelectedMaterial, StairsSelectedMaterial, TrafficSelectedMaterial, TreesSelectedMaterial, EtcSelectedMaterial;

        List<ModelVisual3D> NaviSelectedModels = new List<ModelVisual3D>();
        List<ModelVisual3D> CrossSelectedModels = new List<ModelVisual3D>();
        List<ModelVisual3D> GateSelectedModels = new List<ModelVisual3D>();
        List<ModelVisual3D> StairsSelectedModels = new List<ModelVisual3D>();
        List<ModelVisual3D> TrafficSelectedModels = new List<ModelVisual3D>();
        List<ModelVisual3D> TreesSelectedModels = new List<ModelVisual3D>();
        List<ModelVisual3D> EtcSelectedModels = new List<ModelVisual3D>();

        List<GeometryModel3D> NaviGeometry = new List<GeometryModel3D>();
        List<GeometryModel3D> CrossGeometry = new List<GeometryModel3D>();
        List<GeometryModel3D> GateGeometry = new List<GeometryModel3D>();
        List<GeometryModel3D> StairsGeometry = new List<GeometryModel3D>();
        List<GeometryModel3D> TrafficGeometry = new List<GeometryModel3D>();
        List<GeometryModel3D> TreesGeometry = new List<GeometryModel3D>();
        List<GeometryModel3D> EtcGeometry = new List<GeometryModel3D>();
        
        double SphereScale = 1;
        int DraggedIndex;
        int DraggedViewportIndex;
        int ActivaitionIndex;
        int naviindex;
        float lookatpointX, lookatpointY;
        //Determine whether to drag
        float determinestartdragX, determinestartdragY, determineenddragX, determineenddragY;
        public List<Tile> MapData { get; set; }
        //Navigation Lines
        public Model3DGroup navi = new Model3DGroup();
    
        MapPoint mappoints = new MapPoint();

        Point bef = new Point(-1, -1);

        //경로 위치
        public List<NaviPoint> NaviPoints = new List<NaviPoint>();
        //장애물 위치
        public List<ObsPoint> ObsPoints = new List<ObsPoint>();
        
        //Button Events
        private bool pathinputclicked = false;
        private bool deleteclicked = false;
        private bool obscrossclicked = false;
        private bool obsgateclicked = false;
        private bool obsstairsclicked = false;
        private bool obstrafficclicked = false;
        private bool obstreesclicked = false;
        private bool obsetcclicked = false;
        private bool obsinputclicked = false;

        bool drag_dot = false;
        bool drag_dot_ = false;
        bool file_opened = false;

         public Model3DGroup CurrentDot = new Model3DGroup();
         public Model3DGroup ClickedDot = new Model3DGroup();
        Model3DGroup DrawDot = new Model3DGroup();
        Model3DGroup Something = new Model3DGroup();
       
        PerspectiveCamera camera = new PerspectiveCamera(new Point3D(-15, -88, 104), new Vector3D(0, 1, -1), new Vector3D(0, 0, 1), 80);
        public static VIewer3D Instance { get; set; }
  
        #endregion

        public VIewer3D()
        {
            InitializeComponent();
            //cameraPosTheta = Math.PI / 4;

            Instance = this;

            // Define camera's horizontal field of view in degrees.
            //camera.FieldOfView = 80;
            myViewport3D.RotateAroundMouseDownPoint = true;

            myViewport3D.ZoomAroundMouseDownPoint = true;
            //Assign the camera to the viewport
            myViewport3D.Camera = camera;

            myViewport3D.MouseMove += QuarterViewer_MouseMove;
            myViewport3D.MouseDown += QuarterViewer_MouseDown;
            
            
            myViewport3D.MouseUp += MyViewport3D_MouseUp;
       
            //Keyboard Event
            myViewport3D.KeyDown += QuarterViewer_keyDown;
            
        }

        //MouseUp event for dragging navigation point
        private void MyViewport3D_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (drag_dot_ == true)
            {
                drag_dot_ = false;
                drag_dot = false;
                MoveDotsTo(mappoints.worldX, mappoints.worldY, DraggedIndex);

                //AddNavi(mappoints.worldX, mappoints.worldY, false);

                navi.Children.Clear();
                DrawLinesOnTimeline();
                RemoveDragDots();

            }
        }
        /// <summary>
        /// Dragging dots to specified location
        /// </summary>
       
        private void MoveDotsTo(float x, float y, int index)
        {
            NaviPoints.Insert(index, new NaviPoint(x, y));
       
            NaviPoint model_visual = new NaviPoint(x, y);
           
            model_visual.navipoint = "navipoint";

            Model3DGroup mainModel3Dgroup = new Model3DGroup();
            model_visual.Content = mainModel3Dgroup;
            MeshBuilder meshBuilder = new MeshBuilder();
            meshBuilder.AddBox(new Point3D(x, y, 0.15), SphereScale*2, SphereScale*2, 0.15);
           
            GeometryModel3D dots = new GeometryModel3D(meshBuilder.ToMesh(), NaviMaterial);
           
            NaviGeometry.Insert(index, dots);
            mainModel3Dgroup.Children.Add(dots);
            NaviSelectedModels.Insert(index, model_visual);
            myViewport3D.Children.Insert(DraggedViewportIndex, model_visual);

            NavinObsDiagram.Instance.move_NaviShape(x, y, DraggedIndex);

        }
      
        #region Add Paths and Obstacles
        //경로 추가
        public void AddNavi(float x, float y,  bool ok = true)
        {
           
            NaviPoints.Add(new NaviPoint(x, y));

           // NaviSelectedMaterial = new DiffuseMaterial(Brushes.Aqua);
            NaviPoint model_visual = new NaviPoint(x, y);
            model_visual.INDEX = naviindex;
            model_visual.navipoint = "navipoint";

            Model3DGroup mainModel3Dgroup = new Model3DGroup();
            model_visual.Content = mainModel3Dgroup;
            MeshBuilder meshBuilder = new MeshBuilder();
            meshBuilder.AddBox(new Point3D(x, y, 0.15), SphereScale*2, SphereScale*2, 0.15);
           // DiffuseMaterial NaviMaterial = new DiffuseMaterial(Brushes.Black);
            GeometryModel3D dots = new GeometryModel3D(meshBuilder.ToMesh(), NaviMaterial);

            NaviGeometry.Add(dots);

            mainModel3Dgroup.Children.Add(dots);

            NaviSelectedModels.Add(model_visual);
            myViewport3D.Children.Add(model_visual);
            naviindex++;

            if (ok)
                NavinObsDiagram.Instance.add_NaviePoint(x, y, NaviPoints.Count-1);
         
        }

        //Add Obstacles
        public void AddObs(float x, float y, int kind, bool ok = true)
        {
            ObsPoints.Add(new ObsPoint(x, y));
            if (ok)
            {
                NavinObsDiagram.Instance.add_ObsPoint(x, y, (int)kind);
            }
            //Add Cross
            if ((int)kind == 0)
            {
                CrossMateiral = new DiffuseMaterial(Brushes.MediumSlateBlue);

                DrawCross(CrossMateiral, new ObsPoint(x, y), new Point3D(x, y, 0));
            }
            //Add Gate
            else if ((int)kind == 1)
            {
                GateMaterial = new DiffuseMaterial(Brushes.Turquoise);
                DrawGate(GateMaterial, new ObsPoint(x, y), new Point3D(x, y, 0));
            }
            //Add Stairs
            else if ((int)kind == 2)
            {
                StairsMaterial = new DiffuseMaterial(Brushes.PaleVioletRed);
                DrawStairs(StairsMaterial, new ObsPoint(x, y), new Point3D(x, y, 0));
            }
            //Add Traffic
            else if ((int)kind == 3)
            {
                TrafficMaterrial = new DiffuseMaterial(Brushes.Pink);
                DrawTraffic(TrafficMaterrial, new ObsPoint(x, y), new Point3D(x, y, 0));
            }
            //Add Trees
            else if ((int)kind == 4)
            {
                TreesMaterial = new DiffuseMaterial(Brushes.SkyBlue);
                DrawTrees(TreesMaterial, new ObsPoint(x, y), new Point3D(x, y, 0));
            }
            //Add Etc
            else if ((int)kind == 5)
            {
                EtcMaterial = new DiffuseMaterial(Brushes.Brown);
                DrawEtc(EtcMaterial, new ObsPoint(x, y), new Point3D(x, y, 0));

            }
    

        }
        // Change Obstacles Locations
        public void ChangeObs(float x, float y, int newtype, int old)
        {
            RemoveChangeObsPoints(x, y, old);
            // AddObs(x, y, newtype);
            ObsPoints.Add(new ObsPoint(x, y));

            if ((int)newtype == 0)
            {
                CrossMateiral = new DiffuseMaterial(Brushes.MediumSlateBlue);

                DrawCross(CrossMateiral, new ObsPoint(x, y), new Point3D(x, y, 0));
            }
            else if ((int)newtype == 1)
            {
                GateMaterial = new DiffuseMaterial(Brushes.Turquoise);
                DrawGate(GateMaterial, new ObsPoint(x, y), new Point3D(x, y, 0));
            }
            else if ((int)newtype == 2)
            {
                StairsMaterial = new DiffuseMaterial(Brushes.PaleVioletRed);
                DrawStairs(StairsMaterial, new ObsPoint(x, y), new Point3D(x, y, 0));
            }
            else if ((int)newtype == 3)
            {
                TrafficMaterrial = new DiffuseMaterial(Brushes.Pink);
                DrawTraffic(TrafficMaterrial, new ObsPoint(x, y), new Point3D(x, y, 0));
            }
            else if ((int)newtype == 4)
            {
                TreesMaterial = new DiffuseMaterial(Brushes.SkyBlue);
                DrawTrees(TreesMaterial, new ObsPoint(x, y), new Point3D(x, y, 0));
            }
            else if ((int)newtype == 5)
            {
                EtcMaterial = new DiffuseMaterial(Brushes.Brown);
                DrawEtc(EtcMaterial, new ObsPoint(x, y), new Point3D(x, y, 0));

            }
      
        }
        //Remove Obspoint before changing its location
        public void RemoveChangeObsPoints(float x, float y, int oldtype)
        {

            {
                pathinputclicked = false;
                obsinputclicked = false;
                deleteclicked = true;
         
                if (deleteclicked == true)
                {
                    switch (oldtype)
                    {
                        case 0:
                            {
                                var bb = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "cross").    ToList();
                                foreach (var data in bb)
                                {
                                    if (data.ObsPosX == x && data.ObsPosY == y)
                                    {

                                        CrossGeometry.RemoveAt(CrossSelectedModels.IndexOf(data));
                                    
                                        ObsPoints.Remove(data);
                                        CrossSelectedModels.Remove(data);
                                    
                                        myViewport3D.Children.Remove(data);

                                    }
                                }
                                CrossSelectedModel.Material = RemovedMaterial;

                                CrossSelectedModel = null;
                                break;

                            }
                        case 1:
                            {
                                var bb = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "gate").ToList();
                                foreach (var data in bb)
                                {
                                    if (data.ObsPosX == x && data.ObsPosY == y)
                                    {
                                        GateGeometry.RemoveAt(GateSelectedModels.IndexOf(data));
                                        ObsPoints.Remove(data);
                                        GateSelectedModels.Remove(data);
                                        myViewport3D.Children.Remove(data);
                                    }
                                }
                                GateSelectedModel.Material = RemovedMaterial;

                                GateSelectedModel = null;
                                break;

                            }
                        case 2:
                            {
                                var bb = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "stairs").ToList();
                                foreach (var data in bb)
                                {
                                    if (data.ObsPosX == x && data.ObsPosY == y)
                                    {
                                        StairsGeometry.RemoveAt(StairsSelectedModels.IndexOf(data));
                                        ObsPoints.Remove(data);
                                        StairsSelectedModels.Remove(data);
                                        myViewport3D.Children.Remove(data);
                                    }
                                }
                                StairsSelectedModel.Material = RemovedMaterial;

                                StairsSelectedModel = null;
                                break;

                            }
                        case 3:
                            {
                                var bb = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "traffic").ToList();
                                foreach (var data in bb)
                                {
                                    if (data.ObsPosX == x && data.ObsPosY == y)
                                    {
                                        TrafficGeometry.RemoveAt(TrafficSelectedModels.IndexOf(data));
                                        ObsPoints.Remove(data);
                                        TrafficSelectedModels.Remove(data);
                                        myViewport3D.Children.Remove(data);
                                    }
                                }
                                TrafficSelectedModel.Material = RemovedMaterial;

                                TrafficSelectedModel = null;
                                break;

                            }
                        case 4:
                            {
                                var bb = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "trees").ToList();
                                foreach (var data in bb)
                                {
                                    if (data.ObsPosX == x && data.ObsPosY == y)
                                    {
                                        TreesGeometry.RemoveAt(TreesSelectedModels.IndexOf(data));
                                        ObsPoints.Remove(data);
                                        TreesSelectedModels.Remove(data);
                                        myViewport3D.Children.Remove(data);
                                    }
                                }
                                TreesSelectedModel.Material = RemovedMaterial;

                                TreesSelectedModel = null;
                                break;

                            }
                        case 5:
                            {
                                var bb = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "etc").ToList();
                                foreach (var data in bb)
                                {
                                    if (data.ObsPosX == x && data.ObsPosY == y)
                                    {
                                        EtcGeometry.RemoveAt(EtcSelectedModels.IndexOf(data));
                                        ObsPoints.Remove(data);
                                        EtcSelectedModels.Remove(data);
                                        myViewport3D.Children.Remove(data);
                                    }
                                }
                                EtcSelectedModel.Material = RemovedMaterial;

                                EtcSelectedModel = null;
                                break;

                            }
                    }

                }
            }
        }
        /// <summary>
        /// Draw instant dots for showing current paths
        /// </summary>
 
        public void DrawDots(float x, float y, Model3DGroup model3dgroup)
        {
            ModelVisual3D DotsVisual = new ModelVisual3D();

            DotsVisual.Content = model3dgroup;

            MeshBuilder meshBuilder = new MeshBuilder();
            meshBuilder.AddBox(new Point3D(x, y, 0.15), SphereScale * 2, SphereScale * 2, 0.15);
            DiffuseMaterial material = new DiffuseMaterial(Brushes.Aqua);
            GeometryModel3D dots = new GeometryModel3D(meshBuilder.ToMesh(), material);
            model3dgroup.Children.Add(dots);
            myViewport3D.Children.Add(DotsVisual);
        }

        //Draw Drag Points
        public void DrawDragDots(float x, float y)
        {

            ModelVisual3D DotsVisual = new ModelVisual3D();

            DotsVisual.Content = DrawDot;

            MeshBuilder meshBuilder = new MeshBuilder();
            meshBuilder.AddBox(new Point3D(x, y, 0.15), SphereScale * 2, SphereScale * 2, 0.15);
          
            DiffuseMaterial material = new DiffuseMaterial(Brushes.Aqua);
            GeometryModel3D dots = new GeometryModel3D(meshBuilder.ToMesh(), material);
            DrawDot.Children.Add(dots);
            myViewport3D.Children.Add(DotsVisual);


        }
        //경로 라인 그리기 컴포넌트
        private void DrawLines(NaviLine n, Point3D p0, Point3D p1)
        {
            NaviLine model_visual = n;
            model_visual.naviline = "naviline";

            model_visual.Content = navi;
            NaviDisMaterial = new DiffuseMaterial(Brushes.MidnightBlue);
            MeshBuilder meshBuilder = new MeshBuilder();
            meshBuilder.AddArrow(p0 , p1 , 0.3, 10, 18);
            GeometryModel3D drawlines = new GeometryModel3D(meshBuilder.ToMesh(), NaviDisMaterial);
            navi.Children.Add(drawlines);
            myViewport3D.Children.Add(model_visual);

        }
        //Remove dragged navipoints
        public void RemoveDragNaviPoints(Point p)
        {
       
            HitTestResult result = VisualTreeHelper.HitTest(myViewport3D, p);

            RayMeshGeometry3DHitTestResult mesh_result = result as RayMeshGeometry3DHitTestResult;
            ModelVisual3D navihit = null;

            if (mesh_result != null)
            {
                var model = (GeometryModel3D)mesh_result.ModelHit;



                foreach (var naviselectable in NaviSelectedModels)
                {
                    var model3DGroup = (Model3DGroup)naviselectable.Content;
                    if (model3DGroup.Children.Contains(model) )
                    {
                        navihit = naviselectable;
                        break;
                    }

                }

                if (navihit != null && NaviSelectedModels.IndexOf(navihit) == ActivaitionIndex)
                {
                    DraggedViewportIndex = myViewport3D.Children.IndexOf(navihit);
                    DraggedIndex = NaviSelectedModels.IndexOf(navihit);
                    NavinObsDiagram.Instance.tempMove_NaviPosition(DraggedIndex);
                    var aa = myViewport3D.Children.OfType<NaviPoint>().Where(a => a.navipoint == "navipoint").ToList();
                    //     Console.WriteLine(NaviSelectedModels.IndexOf(navihit));

                    NaviGeometry.RemoveAt(NaviSelectedModels.IndexOf(navihit));
                    NaviPoints.RemoveAt(NaviSelectedModels.IndexOf(navihit));
                    NaviSelectedModels.Remove(navihit);
                    myViewport3D.Children.Remove(navihit);

                    drag_dot_ = true;

                }
           
            }
        }
        //Remove Instant Dots for dragging
        public void RemoveDragDots()
        {
            DrawDot.Children.Clear();
        }
        //Remove Instant Dots for showing current paths of images
        public void RemoveDots(Model3DGroup model3dgroup)
        {
            model3dgroup.Children.Clear();
        }
        //Draw something that is not currently determined which
        public void DrawSomething(float x, float y, int kind)
        {
            ModelVisual3D DotsVisual = new ModelVisual3D();

            DotsVisual.Content = Something;

            MeshBuilder meshBuilder = new MeshBuilder();
            meshBuilder.AddSphere(new Point3D(x, y, 0), SphereScale, 48, 48);
            DiffuseMaterial material = null;
            switch (kind)
            {

                case 0:
                    {
                        material = new DiffuseMaterial(Brushes.MediumSlateBlue);
                        break;
                    }
                //Add Gate
                case 1:
                    {
                        material = new DiffuseMaterial(Brushes.Turquoise);
                        break;
                    }
                //Add Stairs
                case 2:
                    {
                        material = new DiffuseMaterial(Brushes.PaleVioletRed);
                        break;
                    }
                //Add Traffic
                case 3:
                    {
                        material = new DiffuseMaterial(Brushes.Pink);
                        break;
                    }
                //Add Trees
                case 4:
                    {
                        material = new DiffuseMaterial(Brushes.SkyBlue);
                        break;
                    }
                //Add Etc
                case 5:
                    {
                        material = new DiffuseMaterial(Brushes.Brown);
                        break;
                    }
                case 6:
                    {
                        material = new DiffuseMaterial(Brushes.LawnGreen);
                        break;
                    }
                  
            }
            if (material != null)
            {
                GeometryModel3D dots = new GeometryModel3D(meshBuilder.ToMesh(), material);
                Something.Children.Add(dots);
                myViewport3D.Children.Add(DotsVisual);
            }
        }
        public void RemoveSomething()
        {
            Something.Children.Clear();
        }
     
        #endregion
        #region DrawObstacles 

        public void DrawCross(DiffuseMaterial material, ObsPoint o, Point3D p0)
        {
            CrossSelectedMaterial = new DiffuseMaterial(Brushes.LawnGreen);


            ObsPoint model_visual = o;
            model_visual.obspoint = "cross";
            Model3DGroup cross = new Model3DGroup();
       
            model_visual.Content = cross;

            MeshBuilder meshBuilder = new MeshBuilder();
            meshBuilder.AddSphere(p0, SphereScale, 48, 48);

            GeometryModel3D dots = new GeometryModel3D(meshBuilder.ToMesh(), material);
            CrossGeometry.Add(dots);
            CrossSelectedModels.Add(model_visual);
            cross.Children.Add(dots);


            myViewport3D.Children.Add(model_visual);

        }
        public void DrawGate(DiffuseMaterial material, ObsPoint o, Point3D p0)
        {
            GateSelectedMaterial = new DiffuseMaterial(Brushes.LawnGreen);

            ObsPoint model_visual = o;
            model_visual.obspoint = "gate";
            Model3DGroup gate = new Model3DGroup();
           
            model_visual.Content = gate;

            MeshBuilder meshBuilder = new MeshBuilder();
            meshBuilder.AddSphere(p0, SphereScale, 48, 48);

            GeometryModel3D dots = new GeometryModel3D(meshBuilder.ToMesh(), material);
            GateGeometry.Add(dots);
            GateSelectedModels.Add(model_visual);
            gate.Children.Add(dots);

            myViewport3D.Children.Add(model_visual);

        }
        public void DrawStairs(DiffuseMaterial material, ObsPoint o, Point3D p0)
        {

            StairsSelectedMaterial = new DiffuseMaterial(Brushes.LawnGreen);

            ObsPoint model_visual = o;
            model_visual.obspoint = "stairs";
            Model3DGroup stairs = new Model3DGroup();
         
            model_visual.Content = stairs;

            MeshBuilder meshBuilder = new MeshBuilder();
            meshBuilder.AddSphere(p0, SphereScale, 48, 48);

            GeometryModel3D dots = new GeometryModel3D(meshBuilder.ToMesh(), material);
            StairsGeometry.Add(dots);
            StairsSelectedModels.Add(model_visual);
            stairs.Children.Add(dots);


            myViewport3D.Children.Add(model_visual);

        }
        public void DrawTraffic(DiffuseMaterial material, ObsPoint o, Point3D p0)
        {

            TrafficSelectedMaterial = new DiffuseMaterial(Brushes.LawnGreen);

            ObsPoint model_visual = o;
            model_visual.obspoint = "traffic";
            Model3DGroup traffic = new Model3DGroup();
          
            model_visual.Content = traffic;

            MeshBuilder meshBuilder = new MeshBuilder();
            meshBuilder.AddSphere(p0, SphereScale, 48, 48);

            GeometryModel3D dots = new GeometryModel3D(meshBuilder.ToMesh(), material);
            TrafficGeometry.Add(dots);
            TrafficSelectedModels.Add(model_visual);
            traffic.Children.Add(dots);


            myViewport3D.Children.Add(model_visual);

        }
        public void DrawTrees(DiffuseMaterial material, ObsPoint o, Point3D p0)
        {

            TreesSelectedMaterial = new DiffuseMaterial(Brushes.LawnGreen);

            ObsPoint model_visual = o;
            model_visual.obspoint = "trees";
            Model3DGroup trees = new Model3DGroup();
          
            model_visual.Content = trees;

            MeshBuilder meshBuilder = new MeshBuilder();
            meshBuilder.AddSphere(p0, SphereScale, 48, 48);

            GeometryModel3D dots = new GeometryModel3D(meshBuilder.ToMesh(), material);
            TreesGeometry.Add(dots);
            TreesSelectedModels.Add(model_visual);
            trees.Children.Add(dots);


            myViewport3D.Children.Add(model_visual);

        }
        public void DrawEtc(DiffuseMaterial material, ObsPoint o, Point3D p0)
        {

            EtcSelectedMaterial = new DiffuseMaterial(Brushes.LawnGreen);

            ObsPoint model_visual = o;
            model_visual.obspoint = "etc";
            Model3DGroup etc = new Model3DGroup();
            model_visual.Content = etc;

            MeshBuilder meshBuilder = new MeshBuilder();
            meshBuilder.AddSphere(p0, SphereScale, 48, 48);

            GeometryModel3D dots = new GeometryModel3D(meshBuilder.ToMesh(), material);
            EtcGeometry.Add(dots);
            EtcSelectedModels.Add(model_visual);
            etc.Children.Add(dots);


            myViewport3D.Children.Add(model_visual);

        }

        //Clear Data
        public void ClearData(bool drawmap = true)
        {
            CrossSelectedModels.Clear();
            GateSelectedModels.Clear();
            StairsSelectedModels.Clear();
            TrafficSelectedModels.Clear();
            TreesSelectedModels.Clear();
            EtcSelectedModels.Clear();

            NaviSelectedModels.Clear();
            NaviGeometry.Clear();
            GateGeometry.Clear();
            StairsGeometry.Clear();
            TrafficGeometry.Clear();
            TreesGeometry.Clear();
            EtcSelectedModels.Clear();

            NaviPoints.Clear();

            ObsPoints.Clear();

            myViewport3D.Children.Clear();
            file_opened = false;

            if (drawmap == false)
            {
                
                startDrawMapData();
            }
        }
      
        #endregion



        #region ActivatePoints

        //Activate a navigation point on timeline
        public void ActivateNaviPoints(float x, float y)
        {
           
            obsinputclicked = false;

            pathinputclicked = false;

            var aa = myViewport3D.Children.OfType<NaviPoint>().Where(a => a.navipoint == "navipoint").ToList();

            if (NaviSelectedModel != null)
            {
                NaviSelectedModel.Material = NaviMaterial;
                NaviSelectedModel = null;
            }

            ModelVisual3D navihit = null;


            int i = 0;
            int j = 0;
            bool check = false;
            NaviPoint navidata = new NaviPoint(x, y);

            for (i = 0; i < aa.Count; i++)
            {
                if (aa[i].NaviPosX == x && aa[i].NaviPosY == y)
                {
                    navihit = navidata;
                    check = true;
                    break;
                }
                if (check)
                    break;
            }

            if (navihit != null)
            {
                ActivaitionIndex = i;
                NaviSelectedModel = NaviGeometry[i];
                NaviSelectedModel.Material = NaviSelectedMaterial;
                drag_dot = true;
                var pp = myViewport3D.Children.OfType<NaviPoint>().Where(z => z.navipoint == "navipoint").ToList();
                determinestartdragX = pp[ActivaitionIndex].NaviPosX;
                determinestartdragY = pp[ActivaitionIndex].NaviPosY;
                lookatpointX = pp[ActivaitionIndex].NaviPosX;
                lookatpointY = pp[ActivaitionIndex].NaviPosY;

            }
           

            

        }

        //Activate an obstacle point on timeline
        public void ActivateObsPoints(float x, float y, int o)
        {
            obsinputclicked = false;
            pathinputclicked = false;

            if (CrossSelectedModel != null)
            {
                CrossSelectedModel.Material = CrossMateiral;
                CrossSelectedModel = null;
            }
            else if (GateSelectedModel != null)
            {
                GateSelectedModel.Material = GateMaterial;
                GateSelectedModel = null;
            }
            else if (StairsSelectedModel != null)
            {
                StairsSelectedModel.Material = StairsMaterial;
                StairsSelectedModel = null;
            }
            else if (TrafficSelectedModel != null)
            {
                TrafficSelectedModel.Material = TrafficMaterrial;
                TrafficSelectedModel = null;
            }
            else if (TreesSelectedModel != null)
            {
                TreesSelectedModel.Material = TreesMaterial;
                TreesSelectedModel = null;
            }
            else if (EtcSelectedModel != null)
            {
                EtcSelectedModel.Material = EtcMaterial;
                EtcSelectedModel = null;
            }

            int i = 0;
            int j = 0;
            int k = 0;
            int m = 0;
            int n = 0;
            int l = 0;
            bool check = false;
            ModelVisual3D obshit = null;
            ModelVisual3D hit = null;
            ObsPoint obsdata = new ObsPoint(x, y);
            var aa = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "cross").ToList();
            var bb = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "gate").ToList();
            var cc = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "stairs").ToList();
            var dd = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "traffic").ToList();
            var ee = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "trees").ToList();
            var ff = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "etc").ToList();

            switch (o)
            {
                case 0:
                    for (i = 0; i < aa.Count; i++)
                    {
                        if (aa[i].ObsPosX == x && aa[i].ObsPosY == y)
                        {
                            hit = obsdata;
                            check = true;
                            break;
                        }
                        if (check)
                            break;
                    }

                    CrossSelectedModel = CrossGeometry[i];
                    CrossSelectedModel.Material = CrossSelectedMaterial;
                    break;
                case 1:
                    for (j = 0; j < bb.Count; j++)
                    {
                        if (bb[j].ObsPosX == x && bb[j].ObsPosY == y)
                        {
                            hit = obsdata;
                            check = true;
                            break;
                        }
                        if (check)
                            break;
                    }

                    GateSelectedModel = GateGeometry[j];
                    GateSelectedModel.Material = GateSelectedMaterial;
                    break;
                case 2:
                    for (k = 0; k < cc.Count; k++)
                    {
                        if (cc[k].ObsPosX == x && cc[k].ObsPosY == y)
                        {
                            hit = obsdata;
                            check = true;
                            break;
                        }
                        if (check)
                            break;
                    }

                    StairsSelectedModel = StairsGeometry[k];
                    StairsSelectedModel.Material = StairsSelectedMaterial;
                    break;
                case 3:
                    for (l = 0; l < dd.Count; l++)
                    {
                        if (dd[l].ObsPosX == x && dd[l].ObsPosY == y)
                        {
                            hit = obsdata;
                            check = true;
                            break;
                        }
                        if (check)
                            break;
                    }

                    TrafficSelectedModel = TrafficGeometry[l];
                    TrafficSelectedModel.Material = TrafficSelectedMaterial;
                    break;
                case 4:
                    for (m = 0; m < ee.Count; m++)
                    {
                        if (ee[m].ObsPosX == x && ee[m].ObsPosY == y)
                        {
                            hit = obsdata;
                            check = true;
                            break;
                        }
                        if (check)
                            break;
                    }

                    TreesSelectedModel = TreesGeometry[m];
                    TreesSelectedModel.Material = TreesSelectedMaterial;
                    break;
                case 5:
                    for (n = 0; n < ff.Count; n++)
                    {
                        if (ff[n].ObsPosX == x && ff[n].ObsPosY == y)
                        {
                            hit = obsdata;
                            check = true;
                            break;
                        }
                        if (check)
                            break;
                    }

                    EtcSelectedModel = EtcGeometry[n];
                    EtcSelectedModel.Material = EtcSelectedMaterial;
                    break;
            }
            camera.LookAt(new Point3D(x, y, 1), camera.Position.DistanceTo(new Point3D(x, y, 0)), 100);
        }
        #endregion
        #region RemovePoints

        //Remove TimeLine Components via 3D MAP
        public void RemoveTimeLineNaviPoints()
        {

            pathinputclicked = false;
            obsinputclicked = false;
            deleteclicked = true;
          
            ModelVisual3D navihit = null;

            if (deleteclicked == true)
            {
                if (NaviSelectedModel != null)
                {

                    foreach (var selectable in NaviSelectedModels)
                    {
                        var model3DGroup = (Model3DGroup)selectable.Content;
                        if (model3DGroup.Children.Contains(NaviSelectedModel))
                        {

                            navihit = selectable;
                            break;
                        }

                    }
                    if (navihit != null)
                    {
                        var aa = myViewport3D.Children.OfType<NaviPoint>().Where(a => a.navipoint == "navipoint").ToList();

                        NavinObsDiagram.Instance.deleteNaviShape(aa[NaviSelectedModels.IndexOf(navihit)].NaviPosX, aa[NaviSelectedModels.IndexOf(navihit)].NaviPosY);
                        NaviGeometry.RemoveAt(NaviSelectedModels.IndexOf(navihit));
                        NaviPoints.RemoveAt(NaviSelectedModels.IndexOf(navihit));
                        NaviSelectedModels.RemoveAt(NaviSelectedModels.IndexOf(navihit));
                        myViewport3D.Children.Remove(navihit);
                    }


                }
            }


        }
        //Remove NaviPoints
        public void RemoveNaviPoints()
        {
            pathinputclicked = false;
            obsinputclicked = false;
            deleteclicked = true;
       
            ModelVisual3D navihit = null;

            if (deleteclicked == true)
            {
                if (NaviSelectedModel != null)
                {

                    foreach (var selectable in NaviSelectedModels)
                    {
                        var model3DGroup = (Model3DGroup)selectable.Content;
                        if (model3DGroup.Children.Contains(NaviSelectedModel))
                        {

                            navihit = selectable;
                            break;
                        }

                    }
                    if (navihit != null)
                    {

                        var aa = myViewport3D.Children.OfType<NaviPoint>().Where(a => a.navipoint == "navipoint").ToList();

                        NaviGeometry.RemoveAt(NaviSelectedModels.IndexOf(navihit));
                        NaviPoints.RemoveAt(NaviSelectedModels.IndexOf(navihit));
                        NaviSelectedModels.RemoveAt(NaviSelectedModels.IndexOf(navihit));
                        myViewport3D.Children.Remove(navihit);
                        
                    }
                    NaviSelectedModel.Material = RemovedMaterial;
                    NaviSelectedModel = null;

                }
            }

        }
        #region 장애물삭제
        //Remove Timeline Components via 3D MAP
        public void RemoveTimeLineObsPoints()
        {
            pathinputclicked = false;
            obsinputclicked = false;
            deleteclicked = true;
        
            ModelVisual3D crosshit = null;
            ModelVisual3D gatehit = null;
            ModelVisual3D stairshit = null;
            ModelVisual3D traffichit = null;
            ModelVisual3D treeshit = null;
            ModelVisual3D etchit = null;


            if (deleteclicked == true)
            {
                if (CrossSelectedModel != null)
                {
                    foreach (var selectable in CrossSelectedModels)
                    {
                        var model3DGroup = (Model3DGroup)selectable.Content;
                        if (model3DGroup.Children.Contains(CrossSelectedModel))
                        {
                            crosshit = selectable;
                            break;
                        }

                    }
                    if (crosshit != null)
                    {
                        var bb = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "cross").ToList();

                        NavinObsDiagram.Instance.deleteObsShape(bb[CrossSelectedModels.IndexOf(crosshit)].ObsPosX, bb[CrossSelectedModels.IndexOf(crosshit)].ObsPosY);

                        CrossGeometry.RemoveAt(CrossSelectedModels.IndexOf(crosshit));
                        ObsPoints.RemoveAt(CrossSelectedModels.IndexOf(crosshit));
                        CrossSelectedModels.RemoveAt(CrossSelectedModels.IndexOf(crosshit));
                        myViewport3D.Children.Remove(crosshit);

                    }
                    CrossSelectedModel.Material = RemovedMaterial;

                    CrossSelectedModel = null;

                }
                if (GateSelectedModel != null)
                {
                    foreach (var selectable in GateSelectedModels)
                    {
                        var model3DGroup = (Model3DGroup)selectable.Content;
                        if (model3DGroup.Children.Contains(GateSelectedModel))
                        {
                            gatehit = selectable;
                            break;
                        }

                    }
                    if (gatehit != null)
                    {
                        var bb = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "gate").ToList();
             
                        NavinObsDiagram.Instance.deleteObsShape(bb[GateSelectedModels.IndexOf(gatehit)].ObsPosX, bb[GateSelectedModels.IndexOf(gatehit)].ObsPosY);
                        ObsPoints.RemoveAt(GateSelectedModels.IndexOf(gatehit));
                        GateGeometry.RemoveAt(GateSelectedModels.IndexOf(gatehit));
                        GateSelectedModels.RemoveAt(GateSelectedModels.IndexOf(gatehit));
                        myViewport3D.Children.Remove(gatehit);

                    }
                    GateSelectedModel.Material = RemovedMaterial;

                    GateSelectedModel = null;

                }
                else if (StairsSelectedModel != null)
                {
                    foreach (var selectable in StairsSelectedModels)
                    {
                        var model3DGroup = (Model3DGroup)selectable.Content;
                        if (model3DGroup.Children.Contains(StairsSelectedModel))
                        {
                            stairshit = selectable;
                            break;
                        }

                    }
                    if (stairshit != null)
                    {
                        var bb = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "stairs").ToList();
                   
                        NavinObsDiagram.Instance.deleteObsShape(bb[StairsSelectedModels.IndexOf(stairshit)].ObsPosX, bb[StairsSelectedModels.IndexOf(stairshit)].ObsPosY);
                        ObsPoints.RemoveAt(StairsSelectedModels.IndexOf(stairshit));
                        StairsGeometry.RemoveAt(StairsSelectedModels.IndexOf(stairshit));
                        StairsSelectedModels.RemoveAt(StairsSelectedModels.IndexOf(stairshit));
                        myViewport3D.Children.Remove(stairshit);

                    }
                    StairsSelectedModel.Material = RemovedMaterial;

                    StairsSelectedModel = null;
                }
                else if (TrafficSelectedModel != null)
                {
                    foreach (var selectable in TrafficSelectedModels)
                    {
                        var model3DGroup = (Model3DGroup)selectable.Content;
                        if (model3DGroup.Children.Contains(TrafficSelectedModel))
                        {
                            traffichit = selectable;
                            break;
                        }

                    }
                    if (traffichit != null)
                    {
                        var bb = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "traffic").ToList();
     
                        NavinObsDiagram.Instance.deleteObsShape(bb[TrafficSelectedModels.IndexOf(traffichit)].ObsPosX, bb[TrafficSelectedModels.IndexOf(traffichit)].ObsPosY);
                        ObsPoints.RemoveAt(TrafficSelectedModels.IndexOf(traffichit));
                        TrafficGeometry.RemoveAt(TrafficSelectedModels.IndexOf(traffichit));
                        TrafficSelectedModels.RemoveAt(TrafficSelectedModels.IndexOf(traffichit));
                        myViewport3D.Children.Remove(traffichit);

                    }
                    TrafficSelectedModel.Material = RemovedMaterial;

                    TrafficSelectedModel = null;
                }
                else if (TreesSelectedModel != null)
                {
                    foreach (var selectable in TreesSelectedModels)
                    {
                        var model3DGroup = (Model3DGroup)selectable.Content;
                        if (model3DGroup.Children.Contains(TreesSelectedModel))
                        {
                            treeshit = selectable;
                            break;
                        }

                    }
                    if (treeshit != null)
                    {
                        var bb = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "trees").ToList();

                        NavinObsDiagram.Instance.deleteObsShape(bb[TreesSelectedModels.IndexOf(treeshit)].ObsPosX, bb[TreesSelectedModels.IndexOf(treeshit)].ObsPosY);
                        ObsPoints.RemoveAt(TreesSelectedModels.IndexOf(treeshit));
                        TreesGeometry.RemoveAt(TreesSelectedModels.IndexOf(treeshit));
                        TreesSelectedModels.RemoveAt(TreesSelectedModels.IndexOf(treeshit));
                        myViewport3D.Children.Remove(treeshit);

                    }
                    TreesSelectedModel.Material = RemovedMaterial;

                    TreesSelectedModel = null;
                }
                else if (EtcSelectedModel != null)
                {
                    foreach (var selectable in EtcSelectedModels)
                    {
                        var model3DGroup = (Model3DGroup)selectable.Content;
                        if (model3DGroup.Children.Contains(EtcSelectedModel))
                        {
                            etchit = selectable;
                            break;
                        }

                    }
                    if (etchit != null)
                    {
                        var bb = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "etc").ToList();
                        
                        NavinObsDiagram.Instance.deleteObsShape(bb[EtcSelectedModels.IndexOf(etchit)].ObsPosX, bb[EtcSelectedModels.IndexOf(etchit)].ObsPosY);
                        ObsPoints.RemoveAt(EtcSelectedModels.IndexOf(etchit));
                        EtcGeometry.RemoveAt(EtcSelectedModels.IndexOf(etchit));
                        EtcSelectedModels.RemoveAt(EtcSelectedModels.IndexOf(etchit));
                        myViewport3D.Children.Remove(etchit);

                    }
                    EtcSelectedModel.Material = RemovedMaterial;

                    EtcSelectedModel = null;
                }

            }

        }

        public void RemoveObsPoints()
        {
            pathinputclicked = false;
            obsinputclicked = false;
            deleteclicked = true;
        
            ModelVisual3D crosshit = null;
            ModelVisual3D gatehit = null;
            ModelVisual3D stairshit = null;
            ModelVisual3D traffichit = null;
            ModelVisual3D treeshit = null;
            ModelVisual3D etchit = null;


            if (deleteclicked == true)
            {



                if (CrossSelectedModel != null)
                {
                    foreach (var selectable in CrossSelectedModels)
                    {
                        var model3DGroup = (Model3DGroup)selectable.Content;
                        if (model3DGroup.Children.Contains(CrossSelectedModel))
                        {
                            crosshit = selectable;
                            break;
                        }

                    }
                    if (crosshit != null)
                    {
                        var bb = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "cross").ToList();
                        CrossGeometry.RemoveAt(CrossSelectedModels.IndexOf(crosshit));
                        ObsPoints.RemoveAt(CrossSelectedModels.IndexOf(crosshit));
                        CrossSelectedModels.RemoveAt(CrossSelectedModels.IndexOf(crosshit));
                        myViewport3D.Children.Remove(crosshit);

                    }
                    CrossSelectedModel.Material = RemovedMaterial;

                    CrossSelectedModel = null;

                }
                if (GateSelectedModel != null)
                {
                    foreach (var selectable in GateSelectedModels)
                    {
                        var model3DGroup = (Model3DGroup)selectable.Content;
                        if (model3DGroup.Children.Contains(GateSelectedModel))
                        {
                            gatehit = selectable;
                            break;
                        }

                    }
                    if (gatehit != null)
                    {
                        var bb = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "gate").ToList();
                        //  ObsGeometry.RemoveAt(ObsSelectedModels.IndexOf(obshit));
                        ObsPoints.RemoveAt(GateSelectedModels.IndexOf(gatehit));
                        GateGeometry.RemoveAt(GateSelectedModels.IndexOf(gatehit));
                        GateSelectedModels.RemoveAt(GateSelectedModels.IndexOf(gatehit));
                        myViewport3D.Children.Remove(gatehit);

                    }
                    GateSelectedModel.Material = RemovedMaterial;

                    GateSelectedModel = null;

                }
                else if (StairsSelectedModel != null)
                {
                    foreach (var selectable in StairsSelectedModels)
                    {
                        var model3DGroup = (Model3DGroup)selectable.Content;
                        if (model3DGroup.Children.Contains(StairsSelectedModel))
                        {
                            stairshit = selectable;
                            break;
                        }

                    }
                    if (stairshit != null)
                    {
                        var bb = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "stairs").ToList();
                        //  ObsGeometry.RemoveAt(ObsSelectedModels.IndexOf(obshit));
                        ObsPoints.RemoveAt(StairsSelectedModels.IndexOf(stairshit));
                        StairsGeometry.RemoveAt(StairsSelectedModels.IndexOf(stairshit));
                        StairsSelectedModels.RemoveAt(StairsSelectedModels.IndexOf(stairshit));
                        myViewport3D.Children.Remove(stairshit);

                    }
                    StairsSelectedModel.Material = RemovedMaterial;

                    StairsSelectedModel = null;
                }
                else if (TrafficSelectedModel != null)
                {
                    foreach (var selectable in TrafficSelectedModels)
                    {
                        var model3DGroup = (Model3DGroup)selectable.Content;
                        if (model3DGroup.Children.Contains(TrafficSelectedModel))
                        {
                            traffichit = selectable;
                            break;
                        }

                    }
                    if (traffichit != null)
                    {
                        var bb = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "traffic").ToList();
                   
                        ObsPoints.RemoveAt(TrafficSelectedModels.IndexOf(traffichit));
                        TrafficGeometry.RemoveAt(TrafficSelectedModels.IndexOf(traffichit));
                        TrafficSelectedModels.RemoveAt(TrafficSelectedModels.IndexOf(traffichit));
                        myViewport3D.Children.Remove(traffichit);

                    }
                    TrafficSelectedModel.Material = RemovedMaterial;

                    TrafficSelectedModel = null;
                }
                else if (TreesSelectedModel != null)
                {
                    foreach (var selectable in TreesSelectedModels)
                    {
                        var model3DGroup = (Model3DGroup)selectable.Content;
                        if (model3DGroup.Children.Contains(TreesSelectedModel))
                        {
                            treeshit = selectable;
                            break;
                        }

                    }
                    if (treeshit != null)
                    {
                        var bb = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "trees").ToList();
                     
                        ObsPoints.RemoveAt(TreesSelectedModels.IndexOf(treeshit));
                        TreesGeometry.RemoveAt(TreesSelectedModels.IndexOf(treeshit));
                        TreesSelectedModels.RemoveAt(TreesSelectedModels.IndexOf(treeshit));
                        myViewport3D.Children.Remove(treeshit);

                    }
                    TreesSelectedModel.Material = RemovedMaterial;

                    TreesSelectedModel = null;
                }
                else if (EtcSelectedModel != null)
                {
                    foreach (var selectable in EtcSelectedModels)
                    {
                        var model3DGroup = (Model3DGroup)selectable.Content;
                        if (model3DGroup.Children.Contains(EtcSelectedModel))
                        {
                            etchit = selectable;
                            break;
                        }

                    }
                    if (etchit != null)
                    {
                        var bb = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "etc").ToList();

                        ObsPoints.RemoveAt(EtcSelectedModels.IndexOf(etchit));
                        EtcGeometry.RemoveAt(EtcSelectedModels.IndexOf(etchit));
                        EtcSelectedModels.RemoveAt(EtcSelectedModels.IndexOf(etchit));
                        myViewport3D.Children.Remove(etchit);

                    }
                    EtcSelectedModel.Material = RemovedMaterial;

                    EtcSelectedModel = null;
                }

            }

        }

        #endregion

        #endregion


            //Keyboard Event for deselecting objects and deactivating objects
        public void QuarterViewer_keyDown(object sender, KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.Space:
                    {
                        camera.LookAt(new Point3D(lookatpointX, lookatpointY, 1), camera.Position.DistanceTo(new Point3D(lookatpointX, lookatpointY, 0)), 100);

                        break;
                    }

                case Key.LeftCtrl:

                    deselectobject();
                    //Hide PropertyPane
                    PropertiesPane.Instance.hidePropertypane();
                    obsinputclicked = false;
               
                    pathinputclicked = false;
                    obscrossclicked = false;
                    obsgateclicked = false;
                    obsstairsclicked = false;
                    obstrafficclicked = false;
                    obstreesclicked = false;
                    obsetcclicked = false;
                    drag_dot = false;
                    drag_dot_ = false;
                    break;

                case Key.Delete:
                    {
                        obsinputclicked = false;
                        pathinputclicked = false;
                        obscrossclicked = false;
                        obsgateclicked = false;
                        obsstairsclicked = false;
                        obstrafficclicked = false;
                        obstreesclicked = false;
                        obsetcclicked = false;
                        drag_dot = false;
                        drag_dot_ = false;

                        RemoveTimeLineNaviPoints();
                        //RemoveNaviPoints();
                        RemoveTimeLineObsPoints();
                        DrawLinesOnTimeline();
                        NavinObsDiagram.Instance.checkConnectionState();
                        PropertiesPane.Instance.hidePropertypane();
                        NavinObsDiagram.Instance.resetNodeNumber();
                        break;
                    }
           
            }

        }
        private void deselectobject()
        {
            if (NaviSelectedModel != null)
            {
                NaviSelectedModel.Material = NaviMaterial;
                NaviSelectedModel = null;
            }
            if (CrossSelectedModel != null)
            {
                CrossSelectedModel.Material = CrossMateiral;
                CrossSelectedModel = null;
            }
            else if (GateSelectedModel != null)
            {
                GateSelectedModel.Material = GateMaterial;
                GateSelectedModel = null;
            }
            else if (StairsSelectedModel != null)
            {
                StairsSelectedModel.Material = StairsMaterial;
                StairsSelectedModel = null;
            }
            else if (TrafficSelectedModel != null)
            {
                TrafficSelectedModel.Material = TrafficMaterrial;
                TrafficSelectedModel = null;
            }
            else if (TreesSelectedModel != null)
            {
                TreesSelectedModel.Material = TreesMaterial;
                TreesSelectedModel = null;
            }
            else if (EtcSelectedModel != null)
            {
                EtcSelectedModel.Material = EtcMaterial;
                EtcSelectedModel = null;
            }
        }
    
        //Activate Point on 3D Map
        private void ActivatePointon3D(Point p)
        {
            if (NaviSelectedModel != null)
            {
                NaviSelectedModel.Material = NaviMaterial;

                NaviSelectedModel = null;
            }
            else if (CrossSelectedModel != null)
            {
                CrossSelectedModel.Material = CrossMateiral;
                CrossSelectedModel = null;
            }
            else if (GateSelectedModel != null)
            {
                GateSelectedModel.Material = GateMaterial;
                GateSelectedModel = null;
            }
            else if (StairsSelectedModel != null)
            {
                StairsSelectedModel.Material = StairsMaterial;
                StairsSelectedModel = null;
            }
            else if (TrafficSelectedModel != null)
            {
                TrafficSelectedModel.Material = TrafficMaterrial;
                TrafficSelectedModel = null;
            }
            else if (TreesSelectedModel != null)
            {
                TreesSelectedModel.Material = TreesMaterial;
                TreesSelectedModel = null;
            }
            else if (EtcSelectedModel != null)
            {
                EtcSelectedModel.Material = EtcMaterial;
                EtcSelectedModel = null;
            }
            HitTestResult result = VisualTreeHelper.HitTest(myViewport3D, p);

            RayMeshGeometry3DHitTestResult mesh_result = result as RayMeshGeometry3DHitTestResult;

            if (mesh_result != null)
            {

                var model = (GeometryModel3D)mesh_result.ModelHit;
                
                ModelVisual3D crosshit = null;
                ModelVisual3D gatehit = null;
                ModelVisual3D stairshit = null;
                ModelVisual3D traffichit = null;
                ModelVisual3D treeshit = null;
                ModelVisual3D etchit = null;
                ModelVisual3D navihit = null;

                var aa = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "cross").ToList();
                var bb = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "gate").ToList();
                var cc = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "stairs").ToList();
                var dd = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "traffic").ToList();
                var ee = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "trees").ToList();
                var ff = myViewport3D.Children.OfType<ObsPoint>().Where(a => a.obspoint == "etc").ToList();

                foreach (var naviselectable in NaviSelectedModels)
                {
                    var model3DGroup = (Model3DGroup)naviselectable.Content;
                    if (model3DGroup.Children.Contains(model))
                    {
                        navihit = naviselectable;
                        break;
                    }

                }
                foreach (var obsselectable in CrossSelectedModels)
                {
                    var model3DGroup = (Model3DGroup)obsselectable.Content;
                    if (model3DGroup.Children.Contains(model))
                    {
                        crosshit = obsselectable;
                        break;
                    }

                }

                foreach (var obsselectable in GateSelectedModels)
                {
                    var model3DGroup = (Model3DGroup)obsselectable.Content;
                    if (model3DGroup.Children.Contains(model))
                    {
                        gatehit = obsselectable;
                        break;
                    }

                }
               
                foreach (var obsselectable in StairsSelectedModels)
                {
                    var model3DGroup = (Model3DGroup)obsselectable.Content;
                    if (model3DGroup.Children.Contains(model))
                    {
                        stairshit = obsselectable;
                        break;
                    }

                }
               
                foreach (var obsselectable in TrafficSelectedModels)
                {
                    var model3DGroup = (Model3DGroup)obsselectable.Content;
                    if (model3DGroup.Children.Contains(model))
                    {
                        traffichit = obsselectable;
                        break;
                    }

                }
              
                foreach (var obsselectable in TreesSelectedModels)
                {
                    var model3DGroup = (Model3DGroup)obsselectable.Content;
                    if (model3DGroup.Children.Contains(model))
                    {
                        treeshit = obsselectable;
                        break;
                    }

                }
            
                foreach (var obsselectable in EtcSelectedModels)
                {
                    var model3DGroup = (Model3DGroup)obsselectable.Content;
                    if (model3DGroup.Children.Contains(model))
                    {
                        etchit = obsselectable;
                        break;
                    }

                }
                if (navihit != null)
                {
                    var pp = myViewport3D.Children.OfType<NaviPoint>().Where(z => z.navipoint == "navipoint").ToList();
                    
                    
                    ActivaitionIndex = NaviSelectedModels.IndexOf(navihit);
                    lookatpointX = pp[ActivaitionIndex].NaviPosX;
                    lookatpointY = pp[ActivaitionIndex].NaviPosY;
                    NaviSelectedModel = model;
                    NaviSelectedModel.Material = NaviSelectedMaterial;
                    drag_dot = true;
                    NavinObsDiagram.Instance.setShowNaviProperty(pp[ActivaitionIndex].NaviPosX, pp[ActivaitionIndex].NaviPosY);
                   
                  
                }
                else if (crosshit != null)
                {
                    NavinObsDiagram.Instance.setShowObsProperty(aa[CrossSelectedModels.IndexOf(crosshit)].ObsPosX, aa[CrossSelectedModels.IndexOf(crosshit)].ObsPosY);
                    CrossSelectedModel = model;
                    CrossSelectedModel.Material = CrossSelectedMaterial;
                    camera.LookAt(new Point3D(mappoints.worldX, mappoints.worldY, 1), camera.Position.DistanceTo(new Point3D(mappoints.worldX, mappoints.worldY, 0)), 100);
                    drag_dot = false;
                    
                }
                else if (gatehit != null)
                {
                    GateSelectedModel = model;
                    GateSelectedModel.Material = GateSelectedMaterial;
                    camera.LookAt(new Point3D(mappoints.worldX, mappoints.worldY, 1), camera.Position.DistanceTo(new Point3D(mappoints.worldX, mappoints.worldY, 0)), 100);
                    NavinObsDiagram.Instance.setShowObsProperty(bb[GateSelectedModels.IndexOf(gatehit)].ObsPosX, bb[GateSelectedModels.IndexOf(gatehit)].ObsPosY);
                    drag_dot = false;
                }
                else if (stairshit != null)
                {
                    StairsSelectedModel = model;
                    StairsSelectedModel.Material = StairsSelectedMaterial;
                    camera.LookAt(new Point3D(mappoints.worldX, mappoints.worldY, 1), camera.Position.DistanceTo(new Point3D(mappoints.worldX, mappoints.worldY, 0)), 100);
                    NavinObsDiagram.Instance.setShowObsProperty(cc[StairsSelectedModels.IndexOf(stairshit)].ObsPosX, cc[StairsSelectedModels.IndexOf(stairshit)].ObsPosY);
                    drag_dot = false;
                }
                else if (traffichit != null)
                {
                    TrafficSelectedModel = model;
                    TrafficSelectedModel.Material = TrafficSelectedMaterial;
                    camera.LookAt(new Point3D(mappoints.worldX, mappoints.worldY, 1), camera.Position.DistanceTo(new Point3D(mappoints.worldX, mappoints.worldY, 0)), 100);
                    NavinObsDiagram.Instance.setShowObsProperty(dd[TrafficSelectedModels.IndexOf(traffichit)].ObsPosX, dd[TrafficSelectedModels.IndexOf(traffichit)].ObsPosY);
                    drag_dot = false;
                }
                else if (treeshit != null)
                {
                    TreesSelectedModel = model;
                    TreesSelectedModel.Material = TreesSelectedMaterial;
                    camera.LookAt(new Point3D(mappoints.worldX, mappoints.worldY, 1), camera.Position.DistanceTo(new Point3D(mappoints.worldX, mappoints.worldY, 0)), 100);
                    NavinObsDiagram.Instance.setShowObsProperty(ee[TreesSelectedModels.IndexOf(treeshit)].ObsPosX, ee[TreesSelectedModels.IndexOf(treeshit)].ObsPosY);
                    drag_dot = false;
                }
                else if (etchit != null)
                {
                    EtcSelectedModel = model;
                    EtcSelectedModel.Material = EtcSelectedMaterial;
                    camera.LookAt(new Point3D(mappoints.worldX, mappoints.worldY, 1), camera.Position.DistanceTo(new Point3D(mappoints.worldX, mappoints.worldY, 0)), 100);
                    NavinObsDiagram.Instance.setShowObsProperty(ff[EtcSelectedModels.IndexOf(etchit)].ObsPosX, ff[EtcSelectedModels.IndexOf(etchit)].ObsPosY);
                    drag_dot = false;
                }

                else 
                    {
                    PropertiesPane.Instance.hidePropertypane();
                }
            }



        }
        // Mouse Down event
        private void QuarterViewer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            if (pathinputclicked == true)
            {
                drag_dot = false;
                obsinputclicked = false;
                AddNavi(mappoints.worldX, mappoints.worldY);
                deselectobject();
                  PropertiesPane.Instance.hidePropertypane();

            }
            else if (obsinputclicked == true)
            {
                deselectobject();
                PropertiesPane.Instance.hidePropertypane();
                pathinputclicked = false;
                if (obscrossclicked == true)
                {

                    obsgateclicked = false;
                    obsstairsclicked = false;
                    obstrafficclicked = false;
                    obstreesclicked = false;
                    obsetcclicked = false;
                    drag_dot = false;
                    AddObs(mappoints.worldX, mappoints.worldY, 0);


                }
                else if (obsgateclicked == true)
                {

                    obscrossclicked = false;
                    drag_dot = false;
                    obsstairsclicked = false;
                    obstrafficclicked = false;
                    obstreesclicked = false;
                    obsetcclicked = false;
                    AddObs(mappoints.worldX, mappoints.worldY, 1);

                }
                else if (obsstairsclicked == true)
                {
                    drag_dot = false;
                    obscrossclicked = false;
                    obsgateclicked = false;
                    obstrafficclicked = false;
                    obstreesclicked = false;
                    obsetcclicked = false;
                    AddObs(mappoints.worldX, mappoints.worldY, 2);
                }
                else if (obstrafficclicked == true)
                {

                    obscrossclicked = false;
                    obsgateclicked = false;
                    obsstairsclicked = false;
                    drag_dot = false;
                    obstreesclicked = false;
                    obsetcclicked = false;
                    AddObs(mappoints.worldX, mappoints.worldY, 3);
                }
                else if (obstreesclicked == true)
                {

                    obscrossclicked = false;
                    obsgateclicked = false;
                    obsstairsclicked = false;
                    obstrafficclicked = false;
                    drag_dot = false;
                    obsetcclicked = false;
                    AddObs(mappoints.worldX, mappoints.worldY, 4);
                }
                else if (obsetcclicked == true)
                {
                    obscrossclicked = false;
                    drag_dot = false;
                    obsgateclicked = false;
                    obsstairsclicked = false;
                    obstrafficclicked = false;
                    obstreesclicked = false;

                    AddObs(mappoints.worldX, mappoints.worldY, 5);
                }
            }
            else
            {
                pathinputclicked = false;
                obsinputclicked = false;
                //Start dragging if point is activated
                if (drag_dot == true)
                {
                    drag_dot = false;
                    RemoveDragNaviPoints(e.GetPosition(myViewport3D));

                }
                //Activate Point by clicking it
                ActivatePointon3D(e.GetPosition(myViewport3D));


            }

        }

        //Mouse move event 
        private void QuarterViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (file_opened == true)
            {
                bool ok;
                double z = 0;
                double worldx, worldy;

                Viewport3DVisual vpv = VisualTreeHelper.GetParent(myViewport3D.Children[0]) as Viewport3DVisual;

                // Get the world to viewport transform matrix
                Matrix3D m = _3DTools.MathUtils.TryWorldToViewportTransform(vpv, out ok);
                m.Invert();

                Point p1 = Mouse.GetPosition(myViewport3D);
                
                Point3D w10 = new Point3D(p1.X, p1.Y, 10);
                Point3D w20 = new Point3D(p1.X, p1.Y, 20);

                Point3D WorldPointw10 = m.Transform(w10);
                Point3D WorldPointw20 = m.Transform(w20);

                worldx = (z - WorldPointw10.Z) * (WorldPointw20.X - WorldPointw10.X) / (WorldPointw20.Z - WorldPointw10.Z) + WorldPointw10.X;
                worldy = (z - WorldPointw10.Z) * (WorldPointw20.Y - WorldPointw10.Y) / (WorldPointw20.Z - WorldPointw10.Z) + WorldPointw10.Y;
                //Wordl Coordinate from using 3DTools
                mappoints.worldX = (float)worldx;
                mappoints.worldY = (float)worldy;

                determineenddragX = Math.Abs((determinestartdragX - mappoints.worldX) * (determinestartdragX - mappoints.worldX));
                determineenddragY = Math.Abs((determinestartdragY - mappoints.worldY) * (determinestartdragY - mappoints.worldY));

                if(determineenddragX <5 && determineenddragY<5)
                {
                    drag_dot = false;
                }

                if (drag_dot_ == true)
                {
                    RemoveDragDots();

                    DrawDragDots((float)worldx, (float)worldy);
                }
            }
        }

        private void myViewport3D_MouseEnter(object sender, MouseEventArgs e)
        {
            NavinObsDiagram.Instance.Navidiagram.SelectedItem = null;
            NavinObsDiagram.Instance.ObsDiagram.SelectedItem = null;
        }

        private void myViewport3D_MouseLeave(object sender, MouseEventArgs e)
        {
            NavinObsDiagram.Instance.Navidiagram.SelectedItem = null;
            NavinObsDiagram.Instance.ObsDiagram.SelectedItem = null;
        }
      
        #region Button Control
        //Button event for path
        public void path_input_event(object sender, RoutedEventArgs e)

        {
            pathinputclicked = true;
            obsinputclicked = false;
            deleteclicked = false;
       
            MessageBox.Show("경로를 입력하세요");

        }
        //Button event for delete
        public void delete_event(object sender, RoutedEventArgs e)
        {
            RemoveNaviPoints();
            RemoveObsPoints();


        }

        //Button event for obstacle
        public void obs_input_event(object sender, RoutedEventArgs e, ObsPoint.Kinds kind)

        {
            pathinputclicked = false;
            obsinputclicked = true;
            deleteclicked = false;
        
            MessageBox.Show("장애물을 입력하세요");
            switch (kind)
            {
                case ObsPoint.Kinds.Crosswalk:
                    {

                        obsgateclicked = false;
                        obsstairsclicked = false;
                        obstrafficclicked = false;
                        obstreesclicked = false;
                        obsetcclicked = false;
                        obscrossclicked = true;
                        break;
                    }
                case ObsPoint.Kinds.Gate:
                    {
                        obscrossclicked = false;

                        obsstairsclicked = false;
                        obstrafficclicked = false;
                        obstreesclicked = false;
                        obsetcclicked = false;
                        obsgateclicked = true;
                        break;
                    }
                case ObsPoint.Kinds.Stairs:
                    {
                        obscrossclicked = false;
                        obsgateclicked = false;

                        obstrafficclicked = false;
                        obstreesclicked = false;
                        obsetcclicked = false;
                        obsstairsclicked = true;
                        break;
                    }
                case ObsPoint.Kinds.traffic_lights:
                    {
                        obscrossclicked = false;
                        obsgateclicked = false;
                        obsstairsclicked = false;
                        obstreesclicked = false;
                        obsetcclicked = false;
                        obstrafficclicked = true;
                        break;
                    }
                case ObsPoint.Kinds.trees:
                    {
                        obscrossclicked = false;
                        obsgateclicked = false;
                        obsstairsclicked = false;
                        obstrafficclicked = false;
                        obsetcclicked = false;
                        obstreesclicked = true;
                        break;
                    }
                case ObsPoint.Kinds.Etc:
                    {
                        obscrossclicked = false;
                        obsgateclicked = false;
                        obsstairsclicked = false;
                        obstrafficclicked = false;
                        obstreesclicked = false;
                        obsetcclicked = true;
                        break;
                    }
            }
            

        }
        #endregion


        #region Camera and Map
     
      //Get Image Coordinates from 3D Map
        public void ClickedCoordinate()
        {

            bool ok;

            Viewport3DVisual vpv = VisualTreeHelper.GetParent(myViewport3D.Children[0]) as Viewport3DVisual;
            Matrix3D m = _3DTools.MathUtils.TryWorldToViewportTransform(vpv, out ok);

            
            Point3D real = new Point3D(ImageMap.instance.pt.X, ImageMap.instance.pt.Z, ImageMap.instance.pt.Y);
            Point3D worldreal = m.Transform(real);


            m.Invert();

            Point3D coordinatew10 = new Point3D(worldreal.X, worldreal.Y, worldreal.Z + 10);
            Point3D coordinatew20 = new Point3D(worldreal.X, worldreal.Y, worldreal.Z + 20);

            Point3D realtoworldw10 = m.Transform(coordinatew10);
            Point3D realtoworldw20 = m.Transform(coordinatew20);

            double realtoworldx, realtoworldy;
            realtoworldx = (0 - realtoworldw10.Z) * (realtoworldw20.X - realtoworldw10.X) / (realtoworldw20.Z - realtoworldw10.Z) + realtoworldw10.X;
            realtoworldy = (0 - realtoworldw10.Z) * (realtoworldw20.Y - realtoworldw10.Y) / (realtoworldw20.Z - realtoworldw10.Z) + realtoworldw10.Y;
            RemoveSomething();
            DrawSomething((float)realtoworldx, (float)realtoworldy, 6);
            
        }
        //Start to Draw MapData
        public void startDrawMapData()
        {
            if (MapData == null)
                return;
            myViewport3D.Children.Clear();
            myViewport3D.Items.Clear();

            myViewport3D.Children.Add(new ModelVisual3D() { Content = new AmbientLight(Colors.White) });

            OpenCvSharp.Rect2f area;
            OpenCvSharp.Mat mat = Mice.Tools.GetTexture(MapData, out area);

            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = OpenCvSharp.Extensions.WriteableBitmapConverter.ToWriteableBitmap(mat);

            drawRectangleTexture(
                new Point3D(area.Left, area.Top, 0),
                new Point3D(area.Right, area.Top, 0),
                new Point3D(area.Right, area.Bottom, 0),
                new Point3D(area.Left, area.Bottom, 0),
                imageBrush);
            file_opened = true;
        }
        #endregion
        #region 그리기 컴포넌트

  
        void drawTriangleRT(Point3D p0, Point3D p1, Point3D p2, ImageBrush imageBrush)
        {
            MeshGeometry3D triangleMesh = new MeshGeometry3D();

            triangleMesh.Positions.Add(p0);
            triangleMesh.Positions.Add(p1);
            triangleMesh.Positions.Add(p2);

            triangleMesh.TextureCoordinates.Add(new Point(0, 0));
            triangleMesh.TextureCoordinates.Add(new Point(1, 0));
            triangleMesh.TextureCoordinates.Add(new Point(1, 1));

            int n0 = 0;
            int n1 = 1;
            int n2 = 2;

            triangleMesh.TriangleIndices.Add(n0);
            triangleMesh.TriangleIndices.Add(n1);
            triangleMesh.TriangleIndices.Add(n2);

            Material frontMaterial = new DiffuseMaterial(imageBrush);

            GeometryModel3D triangleModel = new GeometryModel3D(triangleMesh, frontMaterial);

            triangleModel.BackMaterial = frontMaterial;

            ModelVisual3D visualModel = new ModelVisual3D();
            visualModel.Content = triangleModel;

            myViewport3D.Children.Add(visualModel);
        }

        void drawTriangleLB(Point3D p0, Point3D p1, Point3D p2, ImageBrush imageBrush)
        {
            MeshGeometry3D triangleMesh = new MeshGeometry3D();

            triangleMesh.Positions.Add(p0);
            triangleMesh.Positions.Add(p1);
            triangleMesh.Positions.Add(p2);

            triangleMesh.TextureCoordinates.Add(new Point(1, 1));
            triangleMesh.TextureCoordinates.Add(new Point(0, 1));
            triangleMesh.TextureCoordinates.Add(new Point(0, 0));

            int n0 = 0;
            int n1 = 1;
            int n2 = 2;

            triangleMesh.TriangleIndices.Add(n0);
            triangleMesh.TriangleIndices.Add(n1);
            triangleMesh.TriangleIndices.Add(n2);

            Material frontMaterial = new DiffuseMaterial(imageBrush);

            GeometryModel3D triangleModel = new GeometryModel3D(triangleMesh, frontMaterial);

            triangleModel.BackMaterial = frontMaterial;

            ModelVisual3D visualModel = new ModelVisual3D();
            visualModel.Content = triangleModel;

            myViewport3D.Children.Add(visualModel);
        }


        void drawRectangleTexture(Point3D p0, Point3D p1, Point3D p2, Point3D p3, ImageBrush imageBrush)
        {
            drawTriangleRT(p0, p1, p2, imageBrush);
            drawTriangleLB(p2, p3, p0, imageBrush);
        }

        #endregion

        /// <summary>
        /// Draw ExistingPath from Images
        /// </summary>
        public void startDrawExistingPath(int imageNumber, bool check = true)
        {

            bool ok;

            SlamImageAnalyzer slam = new SlamImageAnalyzer();
            
            OpenCvSharp.Mat rgb;
            OpenCvSharp.Mat depth;

            try
            {
                string rgbpath = System.IO.Path.Combine(Global.ImageDataPath, "rgb_" + imageNumber.ToString("D5") + ".jpg");
                string depthpath = System.IO.Path.Combine(Global.ImageDataPath, "depth_" + imageNumber.ToString("D5") + ".png");

                if (!System.IO.File.Exists(rgbpath))
                    return;
                if (!System.IO.File.Exists(depthpath))
                    return;

                rgb = OpenCvSharp.Cv2.ImRead(rgbpath);
                depth = OpenCvSharp.Cv2.ImRead(depthpath);
            }
            catch
            {
                return;
            }

            OpenCvSharp.Mat tcw = slam.GetTcw(rgb);
            OpenCvSharp.Vec3f pos = slam.GetPos(rgb);

            Viewport3DVisual vpv = VisualTreeHelper.GetParent(myViewport3D.Children[0]) as Viewport3DVisual;
            Matrix3D m = _3DTools.MathUtils.TryWorldToViewportTransform(vpv, out ok);

            if ((pos.Item0 == 0 || float.IsNaN(pos.Item0)) && (pos.Item1 == 0 || float.IsNaN(pos.Item1)) && (pos.Item2 == 0 || float.IsNaN(pos.Item2)))
            {
                RemoveDots(CurrentDot);
                if (check == false)
                {
                    MessageBox.Show("현재 경로를 추가 할 수 없습니다.");
                }
            }
            else
            {

                Point3D real = new Point3D(pos.Item0, pos.Item2, pos.Item1);
                Point3D worldreal = m.Transform(real);


                m.Invert();

                Point3D coordinatew10 = new Point3D(worldreal.X, worldreal.Y, worldreal.Z + 10);
                Point3D coordinatew20 = new Point3D(worldreal.X, worldreal.Y, worldreal.Z + 20);

                Point3D realtoworldw10 = m.Transform(coordinatew10);
                Point3D realtoworldw20 = m.Transform(coordinatew20);

                double realtoworldx, realtoworldy;
                realtoworldx = (0 - realtoworldw10.Z) * (realtoworldw20.X - realtoworldw10.X) / (realtoworldw20.Z - realtoworldw10.Z) + realtoworldw10.X;
                realtoworldy = (0 - realtoworldw10.Z) * (realtoworldw20.Y - realtoworldw10.Y) / (realtoworldw20.Z - realtoworldw10.Z) + realtoworldw10.Y;
                if (check == true)
                {
                    RemoveDots(CurrentDot);
                    DrawDots((float)realtoworldx, (float)realtoworldy, CurrentDot);
                }
                else if (check == false)

                {
                    RemoveDots(CurrentDot);
                    AddNavi((float)realtoworldx, (float)realtoworldy);

                }
                
            }
        }
        public void startDrawClickedPathOrObstacle(int kind, bool preview = true)
        {

            bool ok;

            Viewport3DVisual vpv = VisualTreeHelper.GetParent(myViewport3D.Children[0]) as Viewport3DVisual;
            Matrix3D m = _3DTools.MathUtils.TryWorldToViewportTransform(vpv, out ok);


            Point3D real = new Point3D(ImageMap.instance.pt.X, ImageMap.instance.pt.Z, ImageMap.instance.pt.Y);
            Point3D worldreal = m.Transform(real);


            m.Invert();

            Point3D coordinatew10 = new Point3D(worldreal.X, worldreal.Y, worldreal.Z + 10);
            Point3D coordinatew20 = new Point3D(worldreal.X, worldreal.Y, worldreal.Z + 20);

            Point3D realtoworldw10 = m.Transform(coordinatew10);
            Point3D realtoworldw20 = m.Transform(coordinatew20);

            double realtoworldx, realtoworldy;
            realtoworldx = (0 - realtoworldw10.Z) * (realtoworldw20.X - realtoworldw10.X) / (realtoworldw20.Z - realtoworldw10.Z) + realtoworldw10.X;
            realtoworldy = (0 - realtoworldw10.Z) * (realtoworldw20.Y - realtoworldw10.Y) / (realtoworldw20.Z - realtoworldw10.Z) + realtoworldw10.Y;
            RemoveSomething();
            if (preview)
            {
                RemoveDots(ClickedDot);
                RemoveSomething();
                switch (kind)
                {
                    case 0:
                        AddObs((float)realtoworldx, (float)realtoworldy, 0);
                        break;
                    case 1:
                        AddObs((float)realtoworldx, (float)realtoworldy, 1);
                        break;
                    case 2:
                        AddObs((float)realtoworldx, (float)realtoworldy, 2);
                        break;
                    case 3:
                        AddObs((float)realtoworldx, (float)realtoworldy, 3);
                        break;
                    case 4:
                        AddObs((float)realtoworldx, (float)realtoworldy, 4);
                        break;
                    case 5:
                        AddObs((float)realtoworldx, (float)realtoworldy, 5);
                        break;
                    case 6:
                        AddNavi((float)realtoworldx, (float)realtoworldy);
                        break;
                }
            }
            else
            {

              
               
                if (kind == 7)
                {
                    RemoveDots(ClickedDot);
                    RemoveSomething();
                    DrawDots((float)realtoworldx, (float)realtoworldy, ClickedDot);
            }
                else
                {
                    RemoveDots(ClickedDot);
                    RemoveSomething();
                    DrawSomething((float)realtoworldx, (float)realtoworldy, kind);
                }
            }
        }
        //Draw Lines on 3D map using timeline's information
        public void DrawLinesOnTimeline(bool drawwhole = true)
        {
           
            var aa = myViewport3D.Children.OfType<Model.NaviLine>().Where(a => a.naviline == "naviline").ToList();
            var bb = myViewport3D.Children.OfType<NaviPoint>().Where(b => b.navipoint == "navipoint").ToList();
            var con = NavinObsDiagram.Instance.Navidiagram.Items.OfType <Telerik.Windows.Controls.RadDiagramConnection>().ToList();
            var navipoint = NavinObsDiagram.Instance.Navidiagram.Items.OfType<NaviShape>().ToList();

            foreach (Telerik.Windows.Controls.RadDiagramConnection item in con)
            {

                if (item.Source != null && item.Target != null)
                {
                    NaviShape src = item.Source as NaviShape;
                    NaviShape dst = item.Target as NaviShape;
                    DrawLines(new NaviLine(), new Point3D(src.NaviPointX , src.NaviPointY, 0.05), new Point3D(dst.NaviPointX , dst.NaviPointY , 0.05));
               
                }
            }
            
        }
       
    }


}
