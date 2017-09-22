using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Shell;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ProjectStarter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, string> Projects;

        public MainWindow()
        {
            InitializeComponent();

            string FolderPath = (string)Properties.Settings.Default.ProjectFolder;

            // Load projects
            Projects = Load_Projects(FolderPath);
            Init_JumpMenu();
            Init_ListView();

            // Checkbox
            CheckBox check = ((CheckBox)FindName("Settings_Save"));
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rk.GetValue("ProjectStarter") != null)
            {
                check.IsChecked = true;
            }
        }

        private void Init_ListView()
        {
            ListView list = (ListView)FindName("Project_List");

            foreach (string key in Projects.Keys)
            {
                ListViewItem item = new ListViewItem { Content = key };

                item.MouseDoubleClick += new MouseButtonEventHandler(ListViewItem_MouseDoubleClick);

                list.Items.Add(item);
            }
        }

        private void Init_JumpMenu()
        {
            JumpList jumpList = new JumpList();

            foreach (var Key in Projects.Keys)
            {
                JumpPath pat = new JumpPath
                {
                    Path = Projects[Key],
                    CustomCategory = "Projects"
                };

                JumpTask task = new JumpTask
                {
                    Title = Key,
                    //Arguments = Projects[Key],
                    CustomCategory = "Projects",
                    // IconResourcePath = Assembly.GetEntryAssembly().CodeBase,
                    ApplicationPath = Projects[Key]
                };

                jumpList.JumpItems.Add(task);
            }


            JumpList.SetJumpList(Application.Current, jumpList);
        }

        private Dictionary<string, string> Load_Projects(string path)
        {
            Dictionary<string, string> tmp_proj = new Dictionary<string, string>();

            try
            {
                foreach (string dir in Directory.GetDirectories(path))
                {
                    Console.WriteLine(dir);
                    foreach (string file in Directory.GetFiles(dir))
                    {
                        string extension = System.IO.Path.GetExtension(file);

                        if (extension.Equals(".sln")
                            || extension.Equals(".pro")
                            || extension.Equals(".csproj"))
                        {
                            try
                            {
                                tmp_proj.Add(dir, file);
                            }
                            catch (System.ArgumentException)
                            {
                                Debug.WriteLine("Duplicate");
                            }
                        }
                    }

                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
            return tmp_proj;
        }

        private void Move_Window(object sender, RoutedEventArgs e)
        {
            DragMove();
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = (ListViewItem)sender;
            Debug.WriteLine(item.Content.ToString());
            System.Diagnostics.Process.Start(Projects[item.Content.ToString()]);
            System.Windows.Application.Current.Shutdown();
        }

        private void Edit_Folder_Path(object sender, RoutedEventArgs e)
        {

            var dlg = new CommonOpenFileDialog();
            dlg.Title = "My Title";
            dlg.IsFolderPicker = true;
            //dlg.InitialDirectory = currentDirectory;

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            // dlg.DefaultDirectory = currentDirectory;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dlg.FileName;

                // Save Settings
                Properties.Settings.Default.ProjectFolder = folder;
                Properties.Settings.Default.Save();

                // Restart
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox check = ((CheckBox)FindName("Settings_Save"));

            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (check.IsChecked == true)
            {
                rk.SetValue("ProjectStarter", System.Reflection.Assembly.GetExecutingAssembly().Location.ToString());
            }
            else
            {
                rk.DeleteValue("ProjectStarter");
            }

        }
    }
}
