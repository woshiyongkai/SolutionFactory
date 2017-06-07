using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SolutionFactory
{
    public class GlobalProperty
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static String ConnectionString { set; get; }
        /// <summary>
        /// 解决方案文件夹保存的路径(只读)
        /// </summary>
        public static String SolutionFloderPath
        {
            get 
            {
                string path = @"c:/SolutionFactory/" + SolutionName;
                if (!Directory.Exists(path))
                {
                    //Directory.Delete(path, true);
                    Directory.CreateDirectory(path);
                }
                return path; 
            }
        }
        /// <summary>
        /// 实体类信息集合
        /// </summary>
        public static List<EntityClassInfo> entitysInfo = new List<EntityClassInfo>();
        /// <summary>
        /// 实体类类库信息
        /// </summary>
        private static LibraryClassInfo modelLibraryClassInfo;
        public static LibraryClassInfo ModelLibraryClassInfo
        {
            set { }
            get
            {
                if (modelLibraryClassInfo == null)
                {
                    modelLibraryClassInfo = new LibraryClassInfo();
                    modelLibraryClassInfo.NameSpace = SolutionName + ".Model";
                    modelLibraryClassInfo.Guid = Guid.NewGuid().ToString();
                    modelLibraryClassInfo.FloderPath = SolutionFloderPath + "/" + SolutionName + ".Model";
                    if (!Directory.Exists(modelLibraryClassInfo.FloderPath))
                    {
                        Directory.CreateDirectory(modelLibraryClassInfo.FloderPath);
                    }
                    modelLibraryClassInfo.AssemblyFloderPath = modelLibraryClassInfo.FloderPath + "/Properties";
                    if (!Directory.Exists(modelLibraryClassInfo.AssemblyFloderPath))
                    {
                        Directory.CreateDirectory(modelLibraryClassInfo.AssemblyFloderPath);
                    }
                }
                return modelLibraryClassInfo;
            }
        }
        /// <summary>
        /// 组装实体工厂类类库信息
        /// </summary>
        private static LibraryClassInfo entityFactoryLibraryClassInfo;
        public static LibraryClassInfo EntityFactoryLibraryClassInfo
        {
            set { }
            get
            {
                if (entityFactoryLibraryClassInfo == null)
                {
                    entityFactoryLibraryClassInfo = new LibraryClassInfo();
                    entityFactoryLibraryClassInfo.NameSpace = SolutionName + ".EntityFactory";
                    entityFactoryLibraryClassInfo.Guid = Guid.NewGuid().ToString();
                    entityFactoryLibraryClassInfo.FloderPath = SolutionFloderPath + "/" + SolutionName + ".EntityFactory";
                    if (!Directory.Exists(entityFactoryLibraryClassInfo.FloderPath))
                    {
                        Directory.CreateDirectory(entityFactoryLibraryClassInfo.FloderPath);
                    }
                    entityFactoryLibraryClassInfo.AssemblyFloderPath = entityFactoryLibraryClassInfo.FloderPath + "/Properties";
                    if (!Directory.Exists(entityFactoryLibraryClassInfo.AssemblyFloderPath))
                    {
                        Directory.CreateDirectory(entityFactoryLibraryClassInfo.AssemblyFloderPath);
                    }
                }
                return entityFactoryLibraryClassInfo;
            }
        }
        /// <summary>
        /// 数据库操作类类库信息
        /// </summary>
        private static LibraryClassInfo dataBaseAccessLibraryClassInfo;
        public static LibraryClassInfo DataBaseAccessLibraryClassInfo
        {

            set { }
            get
            {
                if (dataBaseAccessLibraryClassInfo == null)
                {
                    dataBaseAccessLibraryClassInfo = new LibraryClassInfo();
                    dataBaseAccessLibraryClassInfo.NameSpace = SolutionName + ".DataBaseAccess";
                    dataBaseAccessLibraryClassInfo.Guid = Guid.NewGuid().ToString();
                    dataBaseAccessLibraryClassInfo.FloderPath = SolutionFloderPath + "/" + SolutionName + ".DataBaseAccess";
                    if (!Directory.Exists(dataBaseAccessLibraryClassInfo.FloderPath))
                    {
                        Directory.CreateDirectory(dataBaseAccessLibraryClassInfo.FloderPath);
                    }
                    if (!Directory.Exists(dataBaseAccessLibraryClassInfo.FloderPath+"/DBHelper"))
                    {
                        Directory.CreateDirectory(dataBaseAccessLibraryClassInfo.FloderPath + "/DBHelper");
                    }
                    dataBaseAccessLibraryClassInfo.AssemblyFloderPath = dataBaseAccessLibraryClassInfo.FloderPath + "/Properties";
                    if (!Directory.Exists(dataBaseAccessLibraryClassInfo.AssemblyFloderPath))
                    {
                        Directory.CreateDirectory(dataBaseAccessLibraryClassInfo.AssemblyFloderPath);
                    }
                    
                }
                return dataBaseAccessLibraryClassInfo;
            }
        }
        /// <summary>
        /// 管理站的信息
        /// </summary>
        private static LibraryClassInfo managementWebClassInfo;
        public static LibraryClassInfo ManagementWebClassInfo
        {
            set { GlobalProperty.managementWebClassInfo = value; }
            get 
            {
                if (managementWebClassInfo==null)
                {
                    managementWebClassInfo = new LibraryClassInfo();
                    managementWebClassInfo.NameSpace = SolutionName + ".ManagementWeb";
                    managementWebClassInfo.Guid = Guid.NewGuid().ToString();
                    managementWebClassInfo.FloderPath = SolutionFloderPath + "/" + SolutionName + ".ManagementWeb";
                    if (!Directory.Exists(managementWebClassInfo.FloderPath))
                    {
                        Directory.CreateDirectory(managementWebClassInfo.FloderPath);
                    }
                    managementWebClassInfo.AssemblyFloderPath = managementWebClassInfo.FloderPath + "/Properties";
                    if (!Directory.Exists(managementWebClassInfo.AssemblyFloderPath))
                    {
                        Directory.CreateDirectory(managementWebClassInfo.AssemblyFloderPath);
                    }
                    //生成web站点的文件夹
                    if (!Directory.Exists(managementWebClassInfo.FloderPath + "/Controllers"))
                    {
                        Directory.CreateDirectory(managementWebClassInfo.FloderPath + "/Controllers");
                    }
                    if (!Directory.Exists(managementWebClassInfo.FloderPath + "/Views"))
                    {
                        Directory.CreateDirectory(managementWebClassInfo.FloderPath + "/Views");
                    }
                    if (!Directory.Exists(managementWebClassInfo.FloderPath + "/Views/Shared"))
                    {
                        Directory.CreateDirectory(managementWebClassInfo.FloderPath + "/Views/Shared");
                    }
                    if (!Directory.Exists(managementWebClassInfo.FloderPath + "/Scripts"))
                    {
                        Directory.CreateDirectory(managementWebClassInfo.FloderPath + "/Scripts");
                    }
                    if (!Directory.Exists(managementWebClassInfo.FloderPath + "/Content"))
                    {
                        Directory.CreateDirectory(managementWebClassInfo.FloderPath + "/Content");
                    }
                }
                return GlobalProperty.managementWebClassInfo; 
            }
        }
        /// <summary>
        /// 解决方案名称
        /// </summary>
        public static String SolutionName { set; get; }

    }

}
