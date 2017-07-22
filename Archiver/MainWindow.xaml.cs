﻿using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.IO;
using System.IO.Compression;
using Microsoft.Win32;
using static System.Object;

namespace Archiver
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //О программе
        private void MenuItem_ClickAbout(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            MessageBox.Show("О программе:" +"\n" +
                "Данный архиватор предназначен для открытия архивов, а также архивирования и " +
                "разархивирования файлов");
        }

        //Выход из программы
        private void MenuItem_ClickExit(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        //Открыть архив
        private void MenuItem_ClickOpenArchive(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            //dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".zip";
            dlg.Filter = "Zip files (*.zip, *.7z)|*.zip";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                var dir = new System.IO.DirectoryInfo("C:\\");
                FileInfo[] files = dir.GetFiles("*.zip");
                listBox1.Items.Clear();
                listBox1.ItemsSource = files;
                listBox1.DisplayMemberPath = "Name";
            }
        }

        //Распаковать архив
        private void MenuItem_ClickUnarchive(object sender, RoutedEventArgs e)
        {
            string zipPath = @"c:\Users\gd\Downloads\widgets.zip";
            string extractPath = @"c:\Users\gd\Desktop\unzip";
            try
            {
                ZipFile.ExtractToDirectory(zipPath, extractPath);
            }
            catch(IOException ee)
            {
                MessageBox.Show("Операция записи не может быть" +
                " выполнена, потому что указанный " +
                "файл уже был распакован", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine(ee.GetType().Name);
            }
        }

        //Заархивировать папку
        private void MenuItem_ClickArchiveDirectory(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog()
            { SelectedPath = System.Windows.Forms.Application.StartupPath };
            string pathFile = string.Empty;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pathFile = fbd.SelectedPath.ToString();
            }
            System.Windows.Forms.MessageBox.Show(pathFile, "Путь", System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Information);
            try
            {
                if(pathFile == "")
                {
                    MessageBox.Show("Операция архивации не может быть" +
                    " выполнена, потому что не была выбрана папка для арзивации ",
                    "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                string zipPath = pathFile+".zip";
                ZipFile.CreateFromDirectory(pathFile, zipPath);
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Операция архивации не может быть" +
                " выполнена, потому что не была выбрана папка для арзивации ",
                "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException ee)
            {
                MessageBox.Show("Операция архивации не может быть" +
                " выполнена, потому что указанный " +
                "архив уже был создан", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine(ee.GetType().Name);
            }
        }

        //Заархивировать файл
        private void MenuItem_ClickArchiveFail(object sender, RoutedEventArgs e)
        {
        }
}