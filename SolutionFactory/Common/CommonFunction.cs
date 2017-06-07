using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace SolutionFactory
{
    public class CommonFunction
    {
        #region 公用方法
        /// <summary>
        /// sql数据类型转换成c#数据类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string SqlToCSharpType(string type)
        {
            string result = string.Empty;
            switch (type.ToLower())
            {
                case "int":
                    result = "Int32";
                    break;
                case "text":
                    result = "String";
                    break;
                case "bigint":
                    result = "Int64";
                    break;
                case "binary":
                    result = "Byte[]";
                    break;
                case "bit":
                    result = "Boolean";
                    break;
                case "char":
                    result = "String";
                    break;
                case "datetime":
                    result = "DateTime";
                    break;
                case "decimal":
                    result = "Decimal";
                    break;
                case "float":
                    result = "Double";
                    break;
                case "image":
                    result = "Byte[]";
                    break;
                case "money":
                    result = "Decimal";
                    break;
                ///////
                case "nchar":
                    result = "String";
                    break;
                case "ntext":
                    result = "String";
                    break;
                case "numeric":
                    result = "Decimal";
                    break;
                case "nvarchar":
                    result = "String";
                    break;
                case "real":
                    result = "Single";
                    break;
                case "smalldatetime":
                    result = "DateTime";
                    break;
                case "smallint":
                    result = "Int16";
                    break;
                case "smallmoney":
                    result = "Decimal";
                    break;
                case "timestamp":
                    result = "DateTime";
                    break;
                case "tinyint":
                    result = "Byte";
                    break;
                case "uniqueidentifier":
                    result = "Guid";
                    break;
                case "varbinary":
                    result = "Byte[]";
                    break;
                case "varchar":
                    result = "String";
                    break;
                case "Variant":
                    result = "Object";
                    break;
                ///////
                default:
                    result = "String";
                    break;
            }
            return result;
        }
        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FirstNumberToUpper(string str)
        {
            str = str.Substring(0, 1).ToUpper() + str.Substring(1);
            return str;
        }
        /// <summary>
        /// 生成解决方案文件
        /// </summary>
        public static void GenerateSlnFile()
        {
            WriteFiles.WriteSolutionFile();
        }
        /// <summary>
        /// 工程文件
        /// </summary>
        public static void GenerateProjectFile(LibraryClassInfo libInfo)
        {
            WriteFiles.WriteCsprojFile(libInfo);
        }
        /// <summary>
        /// 程序集信息
        /// </summary>
        public static void GenerateAssemblyInfo(LibraryClassInfo libInfo)
        {
            WriteFiles.WriteAssemblyInfo(libInfo.AssemblyFloderPath, libInfo.Guid);
        }
        #endregion

        #region 生成实体层
        /// <summary>
        /// 根据表名生成实体类
        /// </summary>
        /// <param name="tables"></param>
        public static void GenerateEntityClasses()
        {
            foreach (var item in GlobalProperty.entitysInfo)
            {
                string sql = string.Format(@"SELECT
	                                        c.name AS columname,
	                                        t.name AS typename
                                        FROM
	                                        syscolumns c
                                        INNER JOIN systypes t ON c.xtype = t.xtype
                                        WHERE
	                                        id = object_id('{0}')", item.TableName);
                DataTable table = DBHelper.ReadTable(sql);
                string fksql = string.Format(@"select c.name,OBJECT_NAME(f.referenced_object_id) as ftableName from sysobjects s
inner join sysforeignkeys k on k.constid=s.id
inner join syscolumns c on c.id=k.fkeyid and k.fkey=c.colid
inner join sys.foreign_keys f on s.name=f.name
where OBJECT_NAME(parent_obj)='{0}' and s.xtype='F' ", item.TableName);
                DataTable fktable = DBHelper.ReadTable(fksql);
                foreach (DataRow row in table.Rows)
                {
                    bool isfk = false;
                    EntityFieldsInfo field = new EntityFieldsInfo();
                    foreach (DataRow fkrow in fktable.Rows)
                    {
                        if (row["columname"].ToString() == fkrow["name"].ToString())
                        {
                            isfk = true;
                            field.FieldName = row["columname"].ToString();
                            field.TypeName = fkrow["ftableName"].ToString() + "Model";
                            field.TableColumnName = row["columname"].ToString();
                            field.IsFK = true;
                            item.HasFK = true;
                            item.Fields.Add(field);
                            break;
                        }
                    }
                    if (!isfk)
                    {
                        field.FieldName = row["columname"].ToString();
                        field.TypeName = SqlToCSharpType(row["typename"].ToString());
                        field.TableColumnName = row["columname"].ToString();
                        field.IsFK = false;
                        item.Fields.Add(field);
                    }
                }
                //写入文件
                WriteFiles.WriteEntityFiles();
            }
        }
        #endregion

        #region 生成实体工厂层

        public static void GenerateEntityFactory()
        {
            WriteFiles.WriteBuildEntity();
        }
            
        #endregion

        #region 生成数据库操作
        public static void GenerateDBAccess()
        {
            WriteFiles.WriteDBAccess();
        }
        #endregion

        #region 生成MVC站点
        public static void GenerateMVCManagementWeb()
        {
            GenerateControllers();
            CopyFile();
            GenerateHtmls();
        }
        private static void GenerateControllers()
        {
            WriteFiles.WriteController();
        }
        private static void CopyFile()
        {
            WriteFiles.CopyFileToMVCWeb();
        }
        private static void GenerateHtmls()
        {
            WriteFiles.WriteViewHtml();
        }
        /// <summary>
        /// 拷贝文件夹到指定文件夹
        /// </summary>
        /// <param name="fromDir"></param>
        /// <param name="toDir"></param>
        public static void CopyDir(string fromDir, string toDir)
        {
            if (!Directory.Exists(fromDir))
                return;

            if (!Directory.Exists(toDir))
            {
                Directory.CreateDirectory(toDir);
            }

            string[] files = Directory.GetFiles(fromDir);
            foreach (string formFileName in files)
            {
                string fileName = Path.GetFileName(formFileName);
                string toFileName = Path.Combine(toDir, fileName);
                File.Copy(formFileName, toFileName,true);
            }
            string[] fromDirs = Directory.GetDirectories(fromDir);
            foreach (string fromDirName in fromDirs)
            {
                string dirName = Path.GetFileName(fromDirName);
                string toDirName = Path.Combine(toDir, dirName);
                CopyDir(fromDirName, toDirName);
            }
        }
        
        #endregion
    }
}
