using System.Windows.Media.Media3D;
using System.Windows.Media;
using System;
using Slam_MapEditor.View;

namespace Slam_MapEditor.Model
{
    public class ObsPoint : MapPoint
    {

        public float ObsPosX
        {
            get;set;
        }
        public float ObsPosY
        {
            get;set;
        }
        //public float ObsRelocPosX { get; set; }
        //public float ObsRelocPosY { get; set; }

        //public int INDEX

        //{
        //    get;set;

        //}
        //public float ObsDistance
        //{
        //    get;set;
        //}

        public enum Kinds
        {
            Crosswalk,
            Gate,
            Stairs,
            traffic_lights,
            trees,
            Etc
        };
        public Kinds Kind;

        public string ObsTAG
        {
            get;
            set
            ;
            
        }

        public string ObsMessage
        {

            get
            {
                return "[ X : " + ObsPosX.ToString() + " Y : " + ObsPosY.ToString() + " ]" ;
            }
        }

        public string obspoint { get; set; }
        
        public ObsPoint(float obsposx, float obsposy)
        {
            ObsPosX = obsposx;
            ObsPosY = obsposy;
         
            
        }
        //public ObsPoint(float obsrelocposx, float obsrelocposy, float obsdistance, int index)
        //{
        //    ObsRelocPosX = obsrelocposx;
        //    ObsRelocPosY = obsrelocposy;
        //    ObsDistance = obsdistance;
        //    INDEX = index;
        //}

    }
}
