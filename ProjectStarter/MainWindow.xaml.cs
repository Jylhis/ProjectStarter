using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Shell;

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

            // Load projects
            Projects = Load_Projects("D:\\Projects");
            Init_JumpMenu();
            Init_ListView();
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
                JumpTask task = new JumpTask
                {
                    Title = Key,
                    //Arguments = Projects[Key],
                    CustomCategory = "Last Opened Projects",
                    IconResourcePath = Assembly.GetEntryAssembly().CodeBase,
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
                            tmp_proj.Add(dir, file);
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
    }
}
