using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slam_MapEditor
{
    public static class Global
    {
        //exe 실행 폴더 path
        public static string basePath { get { return AppDomain.CurrentDomain.BaseDirectory; } }

        //프로젝트 폴더 path
        public static string projectPath { get { return System.IO.Path.Combine(basePath, "project"); } }

        //실행한 프로젝트 이름
        public static string projectName { get; set; }

        //Image Path
        public static string ImageDataPath { get { return System.IO.Path.Combine(projectPath, projectName, "MapImageData"); } }
        //Mapdata.xml
        public static string projectMapData { get { return System.IO.Path.Combine(projectPath, projectName, projectName + ".xml"); } }
        //Navi.xml
        public static string projectNaviDiagram { get { return System.IO.Path.Combine(projectPath, projectName, "Navi.xml"); } }
        //Obs.xml
        public static string projectObsDiagram { get { return System.IO.Path.Combine(projectPath, projectName, "Obs.xml"); } }

        public enum ObsCode
        {
            crosswalk = 0,
            gate = 1,
            stairs = 2,
            traffic_lights = 3,
            trees = 4,
            etc = 5
        }

        public static string getObsCodeName(int code)
        {
            string returnValue = string.Empty;

            switch(code)
            {
                case (int)ObsCode.crosswalk:
                    returnValue = "Crosswalk";
                    break;
                case (int)ObsCode.gate:
                    returnValue = "Gate";
                    break;
                case (int)ObsCode.stairs:
                    returnValue = "Stairs";
                    break;
                case (int)ObsCode.traffic_lights:
                    returnValue = "Traffic lights";
                    break;
                case (int)ObsCode.trees:
                    returnValue = "Trees";
                    break;
                case (int)ObsCode.etc:
                    returnValue = "Etc";
                    break;
            }

            return returnValue;
        }
    }
}
