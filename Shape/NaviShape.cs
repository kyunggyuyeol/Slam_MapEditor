using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Diagrams.Extensions.ViewModels;
using Telerik.Windows.Controls.LayoutControl.Serialization;
using Telerik.Windows.Diagrams.Core;
using Slam_MapEditor;
using Slam_MapEditor.View;
using Telerik.Windows.Controls.Diagrams;
using System.Windows.Media;
using System.Windows;

namespace Slam_MapEditor.Shape
{
    public class NaviShape : RadDiagramShape
    {
        /// <summary>
        /// xml로 저장시 값을 저장해줌
        /// </summary>
        /// <returns></returns>
        public override Telerik.Windows.Diagrams.Core.SerializationInfo Serialize()
        {
            var result = base.Serialize();

            result["Index"] = Index.ToString();
            result["PointType"] = PointType.ToString();

            result["NaviPointX"] = NaviPointX.ToString();
            result["NaviPointY"] = NaviPointY.ToString();

            //result["Description1"] = Description1 == null ? "" : Description1;
            //result["Description2"] = Description2 == null ? "" : Description2;
            //result["Description3"] = Description3 == null ? "" : Description3;
            //result["Description4"] = Description4 == null ? "" : Description4;
            //result["Description5"] = Description5 == null ? "" : Description5;

            return result;
        }

        /// <summary>
        /// xml을 클래스화 할때 값을 프로퍼티에 추가함
        /// </summary>
        /// <param name="info"></param>
        public override void Deserialize(Telerik.Windows.Diagrams.Core.SerializationInfo info)
        {
            base.Deserialize(info);
            if (info["Index"] != null)
                this.Index = int.Parse(info["Index"].ToString());

            if (info["PointType"] != null)
                this.PointType = info["PointType"].ToString();

            if (info["NaviPointX"] != null)
                this.NaviPointX = float.Parse(info["NaviPointX"].ToString());
            if (info["NaviPointY"] != null)
                this.NaviPointY = float.Parse(info["NaviPointY"].ToString());

            //if (info["Description1"] != null)
            //    this.Description1 = info["Description1"].ToString();
            //if (info["Description2"] != null)
            //    this.Description2 = info["Description2"].ToString();
            //if (info["Description3"] != null)
            //    this.Description3 = info["Description3"].ToString();
            //if (info["Description4"] != null)
            //    this.Description4 = info["Description4"].ToString();
            //if (info["Description5"] != null)
            //    this.Description5 = info["Description5"].ToString();
        }

        private int _index;

        //인덱스
        public int Index { get { return _index; } set { Content = value; _index = value; } }

        public enum typeValue { start = 0, nomal = 1, end = 2 }

        private string _Pointtype;
        //시작점, 중단점 start, end
        public string PointType
        {
            get { return _Pointtype; }

            set
            {
                _Pointtype = value;

                if (value == "0") //start
                {
                    Background = new SolidColorBrush(Colors.Red);
                }
                else if (value == "1") //nomal
                {
                    //DodgerBlue
                    Background = new SolidColorBrush(Colors.DodgerBlue);
                    //Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#2F9D27"));
                }
                else if (value == "2") //end
                {
                    Background = new SolidColorBrush(Colors.Blue);
                }
            }
        }

        //3D 포지션 값
        public float NaviPointX { get; set; }
        public float NaviPointY { get; set; }

        ////설명 입력 변수
        //public string Description1 { get; set; }
        //public string Description2 { get; set; }
        //public string Description3 { get; set; }
        //public string Description4 { get; set; }
        //public string Description5 { get; set; }

        //nodeType 정의
        public List<custumData> _nodeType = new List<custumData>();

        public List<custumData> NodeType
        {
            get
            {
                return _nodeType;
            }
            set
            {
                _nodeType = value;
            }
        }

        public SolidColorBrush StrokeColor
        {
            get
            {
                return (SolidColorBrush)GetValue(CountProperty);
            }
            set
            {
                SetValue(CountProperty, value);
            }
        }

        public static readonly DependencyProperty CountProperty =
           DependencyProperty.Register("StrokeColor", typeof(SolidColorBrush), typeof(NaviShape), new PropertyMetadata(new SolidColorBrush(Colors.Transparent), null));

        /// <summary>
        /// 초기화
        /// </summary>
        public NaviShape()
        {
            NodeType.Add(new custumData("nomal", false));
            NodeType.Add(new custumData("start", false));
            NodeType.Add(new custumData("end", false));
            NodeType.Add(new custumData("D", false));
            NodeType.Add(new custumData("E", false));
            NodeType.Add(new custumData("F", false));
            NodeType.Add(new custumData("G", false));
            NodeType.Add(new custumData("H", false));
            NodeType.Add(new custumData("I", false));
            NodeType.Add(new custumData("J", false));
            NodeType.Add(new custumData("K", false));
        }


    }
}
