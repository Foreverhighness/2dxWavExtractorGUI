using Microsoft.Win32;
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
using System.Windows.Navigation;

namespace WPF1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FileRadioButton.Click += FileRadioButton_Click;
            DirectoryRadioButton.Click += DirectoryRadioButton_Click;
            MyButton.Click += MyButton_Click;
        }

        private void DirectoryRadioButton_Click(object sender, RoutedEventArgs e)
        {
            FileTextBox.Text = string.Empty;

            FileRadioButton.IsChecked = false;
            FileTextBox.IsEnabled = false;
            FilesBrowser.IsEnabled = false;
            DirectoryBrowser.IsEnabled = true;
            DirectoryRadioButton.IsChecked = true;
            DirectoryTextBox.IsEnabled = true;
        }

        private void FileRadioButton_Click(object sender, RoutedEventArgs e)
        {
            DirectoryTextBox.Text = string.Empty;

            FileRadioButton.IsChecked = true;
            FileTextBox.IsEnabled = true;
            FilesBrowser.IsEnabled = true;
            DirectoryBrowser.IsEnabled = false;
            DirectoryRadioButton.IsChecked = false;
            DirectoryTextBox.IsEnabled = false;
        }

        private void MyButton_Click(object sender, RoutedEventArgs e)
        {
            if (FileTextBox.Text != string.Empty)
            {
                Solve(FileTextBox.Text, TextBox.Text);
                MessageBox.Show("OK");
            }
            else if (DirectoryTextBox.Text != string.Empty)
            {
                DirectoryInfo myDirectoryInfo = new DirectoryInfo(DirectoryTextBox.Text);
                foreach (FileInfo file in myDirectoryInfo.GetFiles("*.2dx"))
                    Solve(file.FullName, TextBox.Text);
                MessageBox.Show("OK");
            }
            if (checkBox.IsChecked == true)
                Close();
        }

        private void FilesBrowser_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog1.Filter = "2dx files (*.2dx)|*.2dx|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == true)
                FileTextBox.Text = openFileDialog1.FileName;
        }

        private void DirectoryBrowser_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog myFolderBrowserDialog = 
                new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = myFolderBrowserDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
                return;

            DirectoryTextBox.Text = myFolderBrowserDialog.SelectedPath.Trim();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog myFolderBrowserDialog =
                new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = myFolderBrowserDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
                return;

            TextBox.Text = myFolderBrowserDialog.SelectedPath.Trim();
        }

        public void Solve(string inputFilePath, string outputPath)
        {
            FileStream inputFileStream = null;
            try
            {
                outputPath = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(inputFilePath));
                //MessageBox.Show(outputPath);
                inputFileStream = new FileStream(inputFilePath, FileMode.Open);
                if (!Directory.Exists(outputPath))
                    Directory.CreateDirectory(outputPath);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            try
            {
                const string riff = "RIFF";
                int musicNumber = 1;
                byte[] myByte = new byte[4];
                while (inputFileStream.Read(myByte, 0, 4) != 0)
                {
                    if (Encoding.Default.GetString(myByte) == riff)
                    {
                        inputFileStream.Read(myByte, 0, 4);
                        int wavFileLength = BitConverter.ToInt32(myByte, 0);
                        //MessageBox.Show(".wav file size: " + wavFileLength.ToString());


                        // 文件输出位置调整
                        string outputFileName = Path.Combine(outputPath, string.Format("{0}.wav", musicNumber++));

                        FileStream outputFileStream = null;
                        try
                        {
                            outputFileStream = new FileStream(outputFileName, FileMode.Create);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }


                        outputFileStream.Write(Encoding.Default.GetBytes(riff), 0, 4);
                        outputFileStream.Write(myByte, 0, 4);

                        byte[] temp = new byte[wavFileLength];
                        inputFileStream.Read(temp, 0, wavFileLength);
                        outputFileStream.Write(temp, 0, wavFileLength);

                        outputFileStream.Close();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                inputFileStream.Close();
            }
        }

    }
}
