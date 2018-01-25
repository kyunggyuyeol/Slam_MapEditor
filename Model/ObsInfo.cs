using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slam_MapEditor.Model
{
    class ObsInfo
    {
        //장애물
        public float ObsPosX { get; set; }

        public float ObsPosY { get; set; }

        public int Obs_Index { get; set; }

        private string _obsType;

        public List<string> MyProperty
        {
            get
            {
                List<string> returnData = new List<string>();
                returnData.Add("AA");
                returnData.Add("BB");
                returnData.Add("CC");
                return returnData;

            }
        }

        public string ObsType
        {
            get
            {
                return _obsType;
            }
            set
            {
                if (value == "")
                {

                }
            }

        }
    }
}
