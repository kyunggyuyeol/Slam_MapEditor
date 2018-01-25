using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slam_MapEditor.Shape
{
    public class custumData
    {
        public string Name { get; set; }

        public bool IsChecked { get; set; }

        public custumData(string name, bool check)
        {
            this.Name = name;
            this.IsChecked = check;
        }
   
    }
}
