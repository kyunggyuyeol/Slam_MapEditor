//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using System.ComponentModel;
//using System.Runtime.Serialization;
//using System.Xml.Serialization;
//using Telerik.Windows.Controls;
//using Telerik.Windows.Diagrams.Core;
//using System.Windows.Media;

//using System.Collections.ObjectModel;
//using System.IO;
//using System.IO.IsolatedStorage;

//using System.Windows.Input;
//using Slam_MapEditor.View;

//using static Slam_MapEditor.MainWindowViewModel;

//namespace Slam_MapEditor
//{

//    [XmlRoot("ImageMapViewModel")]
//    public class ImageMapViewModel : INotifyPropertyChanged
//    {
//        public ImageMapViewModel()
//        {
//            PreviousImageCommand = new RelayCommand(PreviousImage);
//            NextImageCommand = new RelayCommand(NextImage);
//        }


//        public event PropertyChangedEventHandler PropertyChanged;
        
//        public System.Windows.Input.ICommand PreviousImageCommand { get; set; }
//        public System.Windows.Input.ICommand NextImageCommand { get; set; }
//        public List<ImageSource> Images { get; set; }



//        public ImageSource CurrentImage
//        {
//            get
//            {
//                if (currentImageIndex < Images.Count)
//                {
//                    return Images[currentImageIndex];
//                }

//                return null;
//            }
//        }

//        private int currentImageIndex;

//        private void PreviousImage(object o)
//        {
//            if (Images.Count > 0)
//            {
//                // add Image.Count to avoid negative index
//                currentImageIndex = (currentImageIndex + Images.Count - 1) % Images.Count;
//                OnPropertyChanged("CurrentImage");
//            }
//        }

//        private void NextImage(object o)
//        {
//            if (Images.Count > 0)
//            {
//                currentImageIndex = (currentImageIndex + 1) % Images.Count;
//                OnPropertyChanged("CurrentImage");
//            }
//        }

//        private void OnPropertyChanged(string propertyName)
//        {
//            var handler = PropertyChanged;
//            if (handler != null)
//            {
//                handler(this, new PropertyChangedEventArgs(propertyName));
//            }
//        }
//    }
//}

