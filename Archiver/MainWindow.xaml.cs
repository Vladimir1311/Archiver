using System;
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


        /*
         * О программе
         */
        private void MenuItem_ClickAbout(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            MessageBox.Show("О программе:" + "\n" +
                "Данный архиватор предназначен для открытия архивов, " +
                "а также архивирования и " +
                "разархивирования файлов");
        }


        /*
         * Выход из программы
         */ 
        private void MenuItem_ClickExit(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }


        /*
         * Открыть архив
         */
        private void MenuItem_ClickOpenArchive(object sender, RoutedEventArgs e)
        {
            string[] filesnames;
            System.Windows.Forms.OpenFileDialog dlg =
                new System.Windows.Forms.OpenFileDialog();
            dlg.DefaultExt = ".zip";
            dlg.Filter = "Архив|*.zip; *.7z; *.rar";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = dlg.FileName;
                string path = System.IO.Path.GetFullPath(fileName);
                string zipPath = System.IO.Path.GetFullPath(fileName);
                string directoryName = System.IO.Path.GetDirectoryName(fileName);
                string files = directoryName + "\\unarchiver";
                ZipFile.ExtractToDirectory(zipPath, files);
                int count = Directory.GetFiles(files).Length;
                MessageBox.Show(fileName+"\nПуть: "+path);
                if(count == 0)
                {
                    try
                    {
                        FilesNamesListBox.Items.Add("Нету файлов!");
                    }
                    catch(InvalidOperationException eee)
                    {
                        FilesNamesListBox.Items.Add("Архив содержит неизвестные файлы");
                    }
                    Directory.Delete(files, true);
                }
                else if(count>0)
                {
                    //ZipFile.ExtractToDirectory(zipPath, directoryName);
                    try
                    {
                        using (var fstream = File.Open(fileName, FileMode.Open))
                        {
                            var arch = new ZipArchive(fstream);
                            filesnames = arch.Entries.Select(s => s.Name).ToArray();
                        }
                        FilesNamesListBox.ItemsSource = filesnames;
                    }
                    catch (ArgumentException r)
                    {
                        MessageBox.Show("Укажите, пожалуйста, путь до архива! "
                            + r.GetType().Name);
                    }
                    Directory.Delete(files, true);
                }
            }
        }


        /*
         * Распаковать архив
         */
        private void MenuItem_ClickUnarchive(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dlg = 
                new System.Windows.Forms.OpenFileDialog();
            dlg.DefaultExt = ".zip";
            dlg.Filter = "Архив|*.zip; *.7z; *.rar";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] filesnames;
                string fileName = dlg.FileName;
                MessageBox.Show(fileName);
                string zipPath = System.IO.Path.GetFullPath(fileName);
                string directoryName = System.IO.Path.GetDirectoryName(fileName);
                try
                {
                    ZipFile.ExtractToDirectory(zipPath, directoryName);
                    int count = Directory.GetFiles(directoryName).Length;
                    if (count > 0)
                    {
                        try
                        {
                            using (var fstream = File.Open(fileName, FileMode.Open))
                            {
                                var arch = new ZipArchive(fstream);
                                filesnames = arch.Entries.Select(s => s.Name).ToArray();
                            }
                            FilesNamesListBox.ItemsSource = filesnames;
                        }
                        catch (ArgumentException r)
                        {
                            MessageBox.Show("Укажите, пожалуйста, путь до архива! "
                                + r.GetType().Name);
                        }
                    }
                    else if(count == 0)
                    {
                        FilesNamesListBox.Items.Add("Нету файлов!");
                    }
                }
                catch (IOException ee)
                {
                    MessageBox.Show("Операция записи не может быть" +
                    " выполнена, потому что указанный " +
                    "файл уже был распакован", "Ошибка!",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine(ee.GetType().Name);
                }
            }
        }


        /*
         * Заархивировать папку
         */
        private void MenuItem_ClickArchiveDirectory(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = 
                new System.Windows.Forms.FolderBrowserDialog()
            {
                SelectedPath = System.Windows.Forms.Application.StartupPath
            };
            string pathFile = string.Empty;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pathFile = fbd.SelectedPath.ToString();
            }
            System.Windows.Forms.MessageBox.Show(pathFile, "Путь", 
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Information);
            try
            {
                if (pathFile == "")
                {
                    MessageBox.Show("Операция архивации не может быть" +
                    " выполнена, потому что не была выбрана папка для арзивации ",
                    "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                string zipPath = pathFile + ".zip";
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
                "архив уже был создан", "Ошибка!", MessageBoxButton.OK, 
                MessageBoxImage.Error);
                Console.WriteLine(ee.GetType().Name);
            }
        }


        /*
         * Заархивировать файл
         */
        private void MenuItem_ClickArchiveFail(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Здесь будет работать архивирование файла");
            System.Windows.Forms.OpenFileDialog dlg =
                new System.Windows.Forms.OpenFileDialog();
            dlg.DefaultExt = ".*";
            dlg.Filter = "Все файлы|*.*";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = dlg.FileName;
                string path = System.IO.Path.GetFullPath(fileName);
                MessageBox.Show("Путь: " + path + "\n" +
                    "Файл: " + fileName);
                string compressedFile = fileName + ".zip";
                Compress(fileName, compressedFile);
            }
        }

        /*
         * Метод зжатия файлов
         */ 
        public static void Compress(string sourceFile, string compressedFile)
        {
            // поток для чтения исходного файла
            using (FileStream sourceStream = 
                new FileStream(sourceFile, FileMode.OpenOrCreate))
            {
                // поток для записи сжатого файла
                using (FileStream targetStream = File.Create(compressedFile))
                {
                    // поток архивации
                    using (GZipStream compressionStream = 
                        new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        // копируем байты из одного потока в другой
                        sourceStream.CopyTo(compressionStream);
                        MessageBox.Show("Сжатие файла " + 
                            sourceFile + " завершено.\n Исходный размер: "
                            + sourceStream.Length.ToString() + "\nСжатый размер: "
                            + targetStream.Length.ToString());
                    }
                }
            }
        }
    }
}