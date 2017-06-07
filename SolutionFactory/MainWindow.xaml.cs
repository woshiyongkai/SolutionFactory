using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;

namespace SolutionFactory
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //数据库表集合
        DataTable table = null;
        public MainWindow()
        {
            InitializeComponent();
            mainGrid.RowDefinitions[1].Height = new GridLength(0);
        }
        //加载数据库列表
        private void cmbDataBase_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //选择master数据库
                GlobalProperty.ConnectionString = @"Data Source=" + txtAddress.Text + ";Initial Catalog=master;User Id="
                    + txtUsername.Text + ";Password=" + txtPassword.Password + ";";
                DataTable table = DBHelper.ReadTable("select name from sys.databases");
                cmbDataBase.ItemsSource = table.DefaultView;
                cmbDataBase.DisplayMemberPath = "name";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        //显示tables
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            //选定数据库
            GlobalProperty.ConnectionString = @"Data Source=" + txtAddress.Text + ";Initial Catalog=" + cmbDataBase.Text + ";User Id="
                + txtUsername.Text + ";Password=" + txtPassword.Password + ";";
            LoadAllTables();
            //设置样式
            mainGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
            mainGrid.RowDefinitions[0].Height = new GridLength(0);
        }
        //加载表
        private void LoadAllTables()
        {
            table = DBHelper.ReadTable("select name as 'tableName' from sys.objects where type='u'");
            if (table == null || table.Rows.Count <= 0)
            {
                MessageBox.Show("数据库中没有查询到表！");
                return;
            }
            WPanelTables.Children.Clear();
            foreach (DataRow item in table.Rows)
            {
                CheckBox box = new CheckBox();
                box.Content = item["tableName"].ToString();
                box.FontSize = 14;
                box.Margin = new Thickness(10, 5, 10, 5);
                box.MinWidth = 120;
                WPanelTables.Children.Add(box);
            }
        }
        //全选
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (UIElement item in WPanelTables.Children)
            {
                if (item.GetType().ToString() == "System.Windows.Controls.CheckBox")
                {
                    (item as CheckBox).IsChecked = true;
                }
            }
        }
        //全不选
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (UIElement item in WPanelTables.Children)
            {
                if (item.GetType().ToString() == "System.Windows.Controls.CheckBox")
                {
                    (item as CheckBox).IsChecked = false;
                }
            }
        }
        //生成解决方案
        private void btnCreateSolution_Click(object sender, RoutedEventArgs e)
        {
            ShowProgressBar(progbar);

            #region 准备共有的信息
            GlobalProperty.SolutionName = txtSolutionName.Text.Trim();
            
            GlobalProperty.ModelLibraryClassInfo.Classes = new string[table.Rows.Count];
            GlobalProperty.DataBaseAccessLibraryClassInfo.Classes = new string[table.Rows.Count];
            GlobalProperty.EntityFactoryLibraryClassInfo.Classes = new string[table.Rows.Count];
            GlobalProperty.ManagementWebClassInfo.Classes = new string[table.Rows.Count];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                string tableName = table.Rows[i]["tableName"].ToString();
                string className = CommonFunction.FirstNumberToUpper(tableName);
                //实体类信息
                EntityClassInfo entityInfo = new EntityClassInfo();
                entityInfo.ClassName = className;
                entityInfo.ModelClassName = className + "Model";
                entityInfo.TableName = tableName;
                string sql = string.Format(@"SELECT SYSCOLUMNS.name
  FROM SYSCOLUMNS,SYSOBJECTS,SYSINDEXES,SYSINDEXKEYS 
  WHERE SYSCOLUMNS.id = object_id('{0}') 
  AND SYSOBJECTS.xtype = 'PK' 
  AND SYSOBJECTS.parent_obj = SYSCOLUMNS.id 
  AND SYSINDEXES.id = SYSCOLUMNS.id 
  AND SYSOBJECTS.name = SYSINDEXES.name 
  AND SYSINDEXKEYS.id = SYSCOLUMNS.id 
  AND SYSINDEXKEYS.indid = SYSINDEXES.indid   
  AND SYSCOLUMNS.colid = SYSINDEXKEYS.colid", tableName);
                entityInfo.PrimaryKey = DBHelper.GetScalar(sql).ToString();
                GlobalProperty.entitysInfo.Add(entityInfo);
                //各类库下类的集合
                //LibraryClassInfo libinfo = GlobalProperty.ModelLibraryClassInfo;
                GlobalProperty.ModelLibraryClassInfo.Classes[i] = className+"Model.cs";
                GlobalProperty.DataBaseAccessLibraryClassInfo.Classes[i] = className + "DataBaseAccess.cs";
                GlobalProperty.EntityFactoryLibraryClassInfo.Classes[i] = className + "EntityFactory.cs";
                GlobalProperty.ManagementWebClassInfo.Classes[i] = className + "Controller.cs";
            }
            #endregion

            GenAllSolution();
        }
        //生成整体方案
        private void GenAllSolution()
        {
            CommonFunction.GenerateEntityClasses();
            CommonFunction.GenerateEntityFactory();
            CommonFunction.GenerateDBAccess();
            CommonFunction.GenerateMVCManagementWeb();
            
            //实体层的程序集信息和工程文件
            CommonFunction.GenerateAssemblyInfo(GlobalProperty.ModelLibraryClassInfo);
            CommonFunction.GenerateProjectFile(GlobalProperty.ModelLibraryClassInfo);
            //实体工厂层的程序集信息和工程文件
            CommonFunction.GenerateAssemblyInfo(GlobalProperty.EntityFactoryLibraryClassInfo);
            CommonFunction.GenerateProjectFile(GlobalProperty.EntityFactoryLibraryClassInfo);
            //数据库操作层的程序集信息和工程文件
            CommonFunction.GenerateAssemblyInfo(GlobalProperty.DataBaseAccessLibraryClassInfo);
            CommonFunction.GenerateProjectFile(GlobalProperty.DataBaseAccessLibraryClassInfo);
            //管理站点的程序集信息和工程文件
            CommonFunction.GenerateAssemblyInfo(GlobalProperty.ManagementWebClassInfo);
            CommonFunction.GenerateProjectFile(GlobalProperty.ManagementWebClassInfo);
            CommonFunction.GenerateSlnFile();
        }

        //进度条
        public static void ShowProgressBar(System.Windows.Controls.ProgressBar bar)
        {
            double value = 0;
            bar.Minimum = 0;
            bar.Maximum = 100;

            for (int i = 0; i < 100; i++)
            {
                bar.Dispatcher.Invoke(new Action<System.Windows.DependencyProperty, object>(bar.SetValue), System.Windows.Threading.DispatcherPriority.Background, System.Windows.Controls.ProgressBar.ValueProperty, value);
                value++;
                bar.Value = value;
            }
        }
    }
}
