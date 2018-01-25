using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Controls;

namespace Slam_MapEditor.Shape
{
    public class ObsShape : RadDiagramShape
    {

        public delegate void changeItems(ObsShape item, string newColorCode, string oldColorCode);
        public event changeItems chitem;

        public override Telerik.Windows.Diagrams.Core.SerializationInfo Serialize()
        {
            var result = base.Serialize();

            result["Index"] = Index.ToString();
            result["PointType"] = PointType.ToString();

            result["ObsPointX"] = ObsPointX.ToString();
            result["ObsPointY"] = ObsPointY.ToString();

            result["Description"] = Description == null ? "" : Description;
            //result["Description2"] = Description2 == null ? "" : Description2;
            //result["Description3"] = Description3 == null ? "" : Description3;
            //result["Description4"] = Description4 == null ? "" : Description4;
            //result["Description5"] = Description5 == null ? "" : Description5;

            result["oldPointType"] = _oldPointtype == null ? "" : _oldPointtype;

            return result;
        }

        public override void Deserialize(Telerik.Windows.Diagrams.Core.SerializationInfo info)
        {
            base.Deserialize(info);
            if (info["Index"] != null)
                this.Index = int.Parse(info["Index"].ToString());

            if (info["PointType"] != null)
                this.PointType = info["PointType"].ToString();

            if (info["ObsPointX"] != null)
                this.ObsPointX = float.Parse(info["ObsPointX"].ToString());
            if (info["ObsPointY"] != null)
                this.ObsPointY = float.Parse(info["ObsPointY"].ToString());

            if (info["Description"] != null)
                this.Description = info["Description"].ToString();
            //if (info["Description2"] != null)
            //    this.Description2 = info["Description2"].ToString();
            //if (info["Description3"] != null)
            //    this.Description3 = info["Description3"].ToString();
            //if (info["Description4"] != null)
            //    this.Description4 = info["Description4"].ToString();
            //if (info["Description5"] != null)
            //    this.Description5 = info["Description5"].ToString();

            if (info["oldPointType"] != null)
                this._oldPointtype = info["oldPointType"].ToString();
        }

        private int _index;

        //인덱스
        public int Index { get { return _index; } set { Content = value; _index = value; } }

        //3D 포지션 값
        public float ObsPointX { get; set; }
        public float ObsPointY { get; set; }

        //설명 입력 변수
        public string Description { get; set; }
        //public string Description2 { get; set; }
        //public string Description3 { get; set; }
        //public string Description4 { get; set; }
        //public string Description5 { get; set; }
        public string _oldPointtype { get; set; }

        private string _Pointtype;
        //시작점, 중단점 start, end
        public string PointType
        {
            get { return _Pointtype; }

            set
            {
                _Pointtype = value;

                if (value == "0") //Crosswalk
                {
                    Background = new SolidColorBrush(Colors.MediumSlateBlue);
                }
                else if (value == "1") //Gate
                {
                    Background = new SolidColorBrush(Colors.Turquoise);
                }
                else if (value == "2") //Stairs
                {
                    Background = new SolidColorBrush(Colors.PaleVioletRed);
                }
                else if (value == "3") //Traffic lights
                {
                    Background = new SolidColorBrush(Colors.Pink);
                }
                else if (value == "4") //Trees
                {
                    Background = new SolidColorBrush(Colors.SkyBlue);
                }
                else if (value == "5") //Etc
                {
                    Background = new SolidColorBrush(Colors.Brown);
                }

                if (chitem != null)
                    chitem(this, value, _oldPointtype);

                _oldPointtype = value;
            }
        }

        public SolidColorBrush BorderBrushstroke
        {
            get
            {
                return (SolidColorBrush)GetValue(BorderBrushstrokeProperty);
            }
            set
            {
                SetValue(BorderBrushstrokeProperty, value);
            }
        }

        public static readonly DependencyProperty BorderBrushstrokeProperty =
           DependencyProperty.Register("BorderBrushstroke", typeof(SolidColorBrush), typeof(ObsShape), new PropertyMetadata(new SolidColorBrush(Colors.Transparent), null));


        public ObsShape()
        {

        }

        //nodeType 정의
        public List<string> NodeType
        {
            get
            {
                List<string> returnData = new List<string>();
                returnData.Add("Crosswalk");
                returnData.Add("Gate");
                returnData.Add("Stairs");
                returnData.Add("Traffic lights");
                returnData.Add("Trees");
                returnData.Add("Etc");
                return returnData;

            }
        }
    }
}
