using Slam_MapEditor.common;
using Slam_MapEditor.View;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Slam_MapEditor.popup
{
    /// <summary>
    /// NewProject.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NewProject : Window
    {
        public NewProject()
        {
            InitializeComponent();
        }

        private void wizard_Finish(object sender, Telerik.Windows.Controls.NavigationButtonsEventArgs e)
        {

        }

        private void btn_getfile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Multiselect = false;
            of.Filter = "MAP Files (.map)|*.map";
            of.FilterIndex = 1;
            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txt_filename.Text = of.FileName;
            }
        }

        private void btn_getfolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txt_folderName.Text = fb.SelectedPath;
            }
        }

        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            //finish
            //프로젝트 생성
            if (string.IsNullOrEmpty(tb_SimulationName.Text))
            {
                System.Windows.MessageBox.Show("Please enter your project name.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                tb_SimulationName.Focus();
                return;
            }

            //프로젝트 이름 중복 확인
            DirectoryInfo dir = new DirectoryInfo(Global.projectPath);
            var search = dir.GetDirectories().Where(x => x.Name == tb_SimulationName.Text).ToList();
            if (search.Count > 0)
            {
                //여기는 이미 프로젝트가 있다는 소리
                System.Windows.MessageBox.Show("The name of the project that exists.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                tb_SimulationName.Focus();
                return;
            }

            //파일 선택여부
            if (string.IsNullOrEmpty(txt_filename.Text))
            {
                System.Windows.MessageBox.Show("Please select a MAP file.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                btn_getfile.Focus();
                return;
            }
            //image 폴더 선택여부
            if (string.IsNullOrEmpty(txt_folderName.Text))
            {
                System.Windows.MessageBox.Show("Please select image folder.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                btn_getfolder.Focus();
                return;
            }

            //마우스 모양 바꾸고
            using (new WaitCursor())
            {
                //조건을 다 만족한다면 프로젝트를 복사하자
                string createFolder = System.IO.Path.Combine(Global.projectPath, tb_SimulationName.Text);

                //폴더 생성
                Directory.CreateDirectory(createFolder);

                //map 파일 복사
                string dstfileName = System.IO.Path.Combine(createFolder, System.IO.Path.GetFileName(txt_filename.Text));
                File.Copy(txt_filename.Text, dstfileName);

                //image 폴더 복사
                string imagefolder = System.IO.Path.Combine(createFolder, "MapImageData");
                Directory.CreateDirectory(imagefolder);

                //image 폴더 이미지 복사
                DirectoryInfo srcImage = new DirectoryInfo(txt_folderName.Text);
                foreach (FileInfo item in srcImage.GetFiles())
                {
                    string srcfile = item.FullName;
                    string dstfile = System.IO.Path.Combine(imagefolder, System.IO.Path.GetFileName(srcfile));
                    File.Copy(srcfile, dstfile);
                }

                //프로젝트를 연다.
                SolutionExplorer.Instance.projectOpen(dstfileName);
            }

            DialogResult = true;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
