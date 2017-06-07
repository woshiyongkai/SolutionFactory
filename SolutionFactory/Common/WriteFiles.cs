using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace SolutionFactory
{
    public class WriteFiles
    {
        /// <summary>
        /// 输出实体类.cs文件
        /// </summary>
        /// <param name="fields">字段名称，字段类型</param>
        /// <param name="entityName">实体名称</param>
        public static void WriteEntityFiles()
        {
            foreach (var entity in GlobalProperty.entitysInfo)
            {
                TextWriter tw = new StreamWriter(new FileStream(GlobalProperty.ModelLibraryClassInfo.FloderPath + "/" + entity.ModelClassName+".cs", FileMode.Create, FileAccess.Write));
                tw.WriteLine("using System;");
                tw.WriteLine("using System.Text;");
                tw.WriteLine("\r\n");

                tw.WriteLine("namespace " + GlobalProperty.SolutionName + ".Model");
                tw.WriteLine("{");
                tw.WriteLine("\tpublic class " + entity.ModelClassName);
                tw.WriteLine("\t{");

                foreach (var item in entity.Fields)
                {
                    string str = string.Format("\t\tpublic {0} {1} {2}", item.TypeName, item.FieldName, "{ get; set; }");
                    tw.WriteLine(str);
                }

                tw.WriteLine("\t}");
                tw.WriteLine("}");
                tw.Close();
            }
        }
        /// <summary>
        /// 程序集信息
        /// </summary>
        /// <param name="assemblyPath"></param>
        public static void WriteAssemblyInfo(string assemblyPath,string guid)
        {
            string assemblyInfo = File.ReadAllText("ResourceFile/Template/AssemblyInfo.txt");
            assemblyInfo = assemblyInfo.Replace("[assembly: Guid(\"", "[assembly: Guid(\"" + guid);
            TextWriter tw = new StreamWriter(new FileStream(assemblyPath + "/AssemblyInfo.cs", FileMode.Create, FileAccess.Write));
            tw.WriteLine(assemblyInfo);
            tw.Close();
        }
        /// <summary>
        /// 工程文件
        /// </summary>
        public static void WriteCsprojFile(LibraryClassInfo libInfo)
        {
            string assemblyInfo = File.ReadAllText("ResourceFile/Template/CsprojTemplate.txt");
            assemblyInfo = assemblyInfo.Replace("<ProjectGuid></ProjectGuid>", "<ProjectGuid>"+libInfo.Guid+"</ProjectGuid>");
            assemblyInfo = assemblyInfo.Replace("<RootNamespace></RootNamespace>", "<RootNamespace>" + libInfo.NameSpace + "</RootNamespace>");
            assemblyInfo = assemblyInfo.Replace("<AssemblyName></AssemblyName>", "<AssemblyName>" + libInfo.NameSpace + "</AssemblyName>");
            string compiles = "";

            string Controllers = "";
            string HtmlViews="";
            for (int i = 0; i < libInfo.Classes.Length; i++)
            {
                compiles += "<Compile Include=\""+libInfo.Classes[i]+"\" />";
            }
            //如果是管理站点，读取特有的csproj文件
            if (libInfo.NameSpace.EndsWith("ManagementWeb"))
            {
                assemblyInfo = File.ReadAllText("ResourceFile/MVCTemplate/MVCCsprojTemplate.txt");
                assemblyInfo = assemblyInfo.Replace("<ProjectGuid></ProjectGuid>", "<ProjectGuid>" + libInfo.Guid + "</ProjectGuid>");
                assemblyInfo = assemblyInfo.Replace("<RootNamespace></RootNamespace>", "<RootNamespace>" + libInfo.NameSpace + "</RootNamespace>");
                assemblyInfo = assemblyInfo.Replace("<AssemblyName></AssemblyName>", "<AssemblyName>" + libInfo.NameSpace + "</AssemblyName>");
                foreach (var item in GlobalProperty.entitysInfo)
                {
                    Controllers += "<Compile Include=\"Controllers\\"+item.ClassName+"Controller.cs\" />";
                    HtmlViews += "<Content Include=\"Views\\" + item.ClassName + "\\Index.cshtml\" />";
                    HtmlViews += "<Content Include=\"Views\\" + item.ClassName + "\\" + item.ClassName + "Info.cshtml\" />";
                }
                assemblyInfo = assemblyInfo.Replace("#Controllers#", Controllers);
                assemblyInfo = assemblyInfo.Replace("#HtmlViews#", HtmlViews);

                //引用其他类库
                string projectReference = "";
                projectReference += string.Format("<ProjectReference Include=\"..\\{0}\\{0}.csproj\">", GlobalProperty.DataBaseAccessLibraryClassInfo.NameSpace);
                projectReference += "    <Project>{" + GlobalProperty.DataBaseAccessLibraryClassInfo.Guid + "}</Project>";
                projectReference += "       <Name>" + GlobalProperty.DataBaseAccessLibraryClassInfo.NameSpace + "</Name>";
                projectReference += "</ProjectReference>";

                projectReference += string.Format("<ProjectReference Include=\"..\\{0}\\{0}.csproj\">", GlobalProperty.ModelLibraryClassInfo.NameSpace);
                projectReference += "    <Project>{" + GlobalProperty.ModelLibraryClassInfo.Guid + "}</Project>";
                projectReference += "       <Name>" + GlobalProperty.ModelLibraryClassInfo.NameSpace + "</Name>";
                projectReference += "</ProjectReference>";

                assemblyInfo = assemblyInfo.Replace("<ProjectReference></ProjectReference>", projectReference);
            }
            //如果是数据库操作类库，需要引用实体类库和实体工厂类库，Configuration引用及包含DBHelper
            if (libInfo.NameSpace.EndsWith("DataBaseAccess"))
            {
                //引用其他类库
                string projectReference = "";
                projectReference +=string.Format("<ProjectReference Include=\"..\\{0}\\{0}.csproj\">",GlobalProperty.EntityFactoryLibraryClassInfo.NameSpace);
                projectReference += "    <Project>{" + GlobalProperty.EntityFactoryLibraryClassInfo.Guid+ "}</Project>";
                projectReference += "       <Name>"+GlobalProperty.EntityFactoryLibraryClassInfo.NameSpace+"</Name>";
                projectReference += "</ProjectReference>";

                projectReference += string.Format("<ProjectReference Include=\"..\\{0}\\{0}.csproj\">", GlobalProperty.ModelLibraryClassInfo.NameSpace);
                projectReference += "    <Project>{" + GlobalProperty.ModelLibraryClassInfo.Guid + "}</Project>";
                projectReference += "       <Name>" + GlobalProperty.ModelLibraryClassInfo.NameSpace + "</Name>";
                projectReference += "</ProjectReference>";

                assemblyInfo = assemblyInfo.Replace("<ProjectReference></ProjectReference>", projectReference);
                string config = " <Reference Include=\"System\" />";
                config+="<Reference Include=\"System.configuration\" />";
    
                assemblyInfo = assemblyInfo.Replace("<Reference Include=\"System\" />", config);
                //dbhelper
                compiles += "<Compile Include=\"DBHelper\\DBHelper.cs\" />" + "\r\n";
                compiles += "<Compile Include=\"DBHelper\\DBHelper_Static.cs\" />" + "\r\n";
                compiles += "<Compile Include=\"DBHelper\\HelperBase.cs\" />" + "\r\n";
                compiles += "<Compile Include=\"AccessBase.cs\" />" + "\r\n";
            }
            //如果是实体工厂类库，引用Model
            if (libInfo.NameSpace.EndsWith("EntityFactory"))
            {
                //引用其他类库
                string projectReference = "";
                projectReference += string.Format("<ProjectReference Include=\"..\\{0}\\{0}.csproj\">", GlobalProperty.ModelLibraryClassInfo.NameSpace);
                projectReference += "    <Project>{" + GlobalProperty.ModelLibraryClassInfo.Guid + "}</Project>";
                projectReference += "       <Name>" + GlobalProperty.ModelLibraryClassInfo.NameSpace + "</Name>";
                projectReference += "</ProjectReference>";

                assemblyInfo = assemblyInfo.Replace("<ProjectReference></ProjectReference>", projectReference);
            }
            //如果是实体类库去掉引用
            if (libInfo.NameSpace.EndsWith("Model"))
            {
                //引用其他类库
                string projectReference = "";

                assemblyInfo = assemblyInfo.Replace("<ProjectReference></ProjectReference>", projectReference);
            }
            //非管理站点
            if (!libInfo.NameSpace.EndsWith("ManagementWeb"))
            {
                assemblyInfo = assemblyInfo.Replace("<Compile></Compile>", compiles);
            }
            TextWriter tw = new StreamWriter(new FileStream(libInfo.FloderPath+ "/" + libInfo.NameSpace+".csproj", FileMode.Create, FileAccess.Write));
            tw.WriteLine(assemblyInfo);
            tw.Close();
        }
        /// <summary>
        /// 解决方案文件
        /// </summary>
        public static void WriteSolutionFile()
        {
            TextWriter tw = new StreamWriter(new FileStream(GlobalProperty.SolutionFloderPath +"/" + GlobalProperty.SolutionName+".sln", FileMode.Create, FileAccess.Write));
            tw.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00");
            tw.WriteLine("# Visual Studio 2012");
            List<LibraryClassInfo> list = new List<LibraryClassInfo>();
            list.Add(GlobalProperty.ManagementWebClassInfo);
            list.Add(GlobalProperty.ModelLibraryClassInfo);
            list.Add(GlobalProperty.EntityFactoryLibraryClassInfo);
            list.Add(GlobalProperty.DataBaseAccessLibraryClassInfo);
            foreach (LibraryClassInfo item in list)
            {
                tw.WriteLine("Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"" + item.NameSpace + "\", \"" + item.NameSpace + "\\" + item.NameSpace + ".csproj\", \"{" + item.Guid + "}\"");
                tw.WriteLine("EndProject");
            }
            tw.Close();
        }
        /// <summary>
        /// 组装实体类
        /// </summary>
        public static void WriteBuildEntity()
        {
            foreach (var entity in GlobalProperty.entitysInfo)
            {
                TextWriter tw = new StreamWriter(new FileStream(GlobalProperty.EntityFactoryLibraryClassInfo.FloderPath + "/" + entity.ClassName + "EntityFactory.cs", FileMode.Create, FileAccess.Write));
                tw.WriteLine("using System;");
                tw.WriteLine("using System.Text;");
                tw.WriteLine("using System.Data;");
                tw.WriteLine("using " + GlobalProperty.ModelLibraryClassInfo.NameSpace + ";");
                tw.WriteLine("\r\n");

                tw.WriteLine("namespace " + GlobalProperty.EntityFactoryLibraryClassInfo.NameSpace);
                tw.WriteLine("{");
                tw.WriteLine("\tpublic class " + entity.ClassName + "EntityFactory");
                tw.WriteLine("\t{");
                tw.WriteLine("\t\tpublic static " + entity.ModelClassName + " Build" + entity.ClassName + "Entity(IDataReader reader)");
                tw.WriteLine("\t\t{");
                tw.WriteLine(string.Format("\t\t\t{0} entity = new {0}();", entity.ModelClassName));
                if (entity.HasFK)
                {
                    #region 有外键的,列名格式为"表名.列名"

                    foreach (var field in entity.Fields)
                    {
                        if (!field.IsFK)
                        {
                            tw.WriteLine("\t\t\tif (reader[" + "\"" + entity.TableName + "." + field.TableColumnName + "\"" + "] != DBNull.Value)");
                            tw.WriteLine("\t\t\t\tentity." + field.FieldName + " = Convert.To" + field.TypeName + "(reader[" + "\"" + entity.TableName + "." + field.TableColumnName + "\"" + "]);");
                        }
                        else
                        {
                            foreach (EntityClassInfo fkentity in GlobalProperty.entitysInfo)
                            {
                                if (fkentity.ModelClassName.ToString() == field.TypeName.ToString())
                                {
                                    tw.WriteLine("\t\t\tif (reader[" + "\"" + entity.TableName + "." + field.TableColumnName + "\"" + "] != DBNull.Value)");
                                    tw.WriteLine("\t\t\t{");
                                    tw.WriteLine("\t\t\t\t" + fkentity.ModelClassName + " " + fkentity.ModelClassName + " = new " + fkentity.ModelClassName + "();");
                                    foreach (var fkitem in fkentity.Fields)
                                    {
                                        if (!fkitem.IsFK)
                                        {
                                            tw.WriteLine("\t\t\t\tif (reader[" + "\"" + fkentity.TableName + "." + fkitem.TableColumnName + "\"" + "] != DBNull.Value)");
                                            tw.WriteLine("\t\t\t\t\t" + fkentity.ModelClassName + "." + fkitem.FieldName + " = Convert.To" + fkitem.TypeName + "(reader[" + "\"" + fkentity.TableName + "." + fkitem.TableColumnName + "\"" + "]);");


                                        }
                                    }
                                    tw.WriteLine("\t\t\t\tentity." + field.FieldName + " = " + fkentity.ModelClassName + ";");
                                    tw.WriteLine("\t\t\t}");
                                    break;
                                }
                            }
                        }
                    }
                    #endregion

                }
                else
                {
                    #region 没有外键直接组装
                    foreach (var field in entity.Fields)
                    {
                        tw.WriteLine("\t\t\tif (reader[" + "\""  + field.TableColumnName + "\"" + "] != DBNull.Value)");
                        tw.WriteLine("\t\t\t\tentity." + field.FieldName + " = Convert.To" + field.TypeName + "(reader[" + "\""  + field.TableColumnName + "\"" + "]);");
                        
                    }
                    #endregion
                }

                tw.WriteLine("\t\t\treturn entity;");
                tw.WriteLine("\t\t}");
                tw.WriteLine("\t}");
                tw.WriteLine("}");
                tw.Close();
            }
        }
        /// <summary>
        /// 数据库操作
        /// </summary>
        public static void WriteDBAccess()
        {
            //DBHelper
            InitDBHelper();
            //DatabaseAccess基类
            InitAccessBase();

            foreach (EntityClassInfo entity in GlobalProperty.entitysInfo)
            {
                //写入文件信息
                string AccessBase = File.ReadAllText("ResourceFile/Template/AccessFile.txt");
                AccessBase = AccessBase.Replace("#SolutionName#", GlobalProperty.SolutionName);
                AccessBase = AccessBase.Replace("#ClassName#", entity.ClassName);
                AccessBase = AccessBase.Replace("#ModelClassName#", entity.ModelClassName);
                AccessBase = AccessBase.Replace("#TableName#", entity.TableName);
                AccessBase = AccessBase.Replace("#PrimaryKey#", entity.PrimaryKey);

                #region 增，改的字符串
                string AllAddColumns = string.Empty;
                string AllAddValues = string.Empty;
                string AllFields = string.Empty;
                string AllUpdateFields = string.Empty;
                string AllUpdateColumns = string.Empty;
                int LastFieldsIndex = 0;
                int i = 0;
                int j = 0;
                foreach (var item in entity.Fields)
                {
                    
                    AllAddColumns+= "[" + item.TableColumnName + "], ";
                    AllAddValues += "'{" + i + "}'" + ", ";
                    if (item.IsFK)
                    {
                        AllFields += "," + "entity." + item.FieldName+"."+item.FieldName;
                    }
                    else
                    {
                        AllFields += "," + "entity." + item.FieldName;
                    }
                    //要更新的字段
                    if (item.FieldName!=entity.PrimaryKey)
                    {
                        if (item.IsFK)
                        {
                            AllUpdateFields += "," + "entity." + item.FieldName + "." + item.FieldName;
                        }
                        else
                        {
                            AllUpdateFields += "," + "entity." + item.FieldName;
                        }
                        AllUpdateColumns += "[" + item.TableColumnName + "] = '{" + j + "}'" + ", ";
                        j++;
                    }
                    i++;
                }
                AllUpdateFields += "," + "entity." + entity.PrimaryKey;
                if (AllAddColumns.EndsWith(", "))
                    AllAddColumns = AllAddColumns.Substring(0, AllAddColumns.Length - 2);
                if (AllAddValues.EndsWith(", "))
                    AllAddValues = AllAddValues.Substring(0, AllAddValues.Length - 2);
                if (AllUpdateColumns.EndsWith(", "))
                    AllUpdateColumns = AllUpdateColumns.Substring(0, AllUpdateColumns.Length - 2);
                LastFieldsIndex = i - 1;
                #endregion

                #region 联合查询Sql
                //根据主键查询
                string PKSelectSql = string.Empty;
                string WhereSelectSql = string.Empty;
                if (entity.HasFK)
                {
                    string allcolumns = string.Empty;
                    string alltables = entity.TableName;
                    foreach (var item in entity.Fields)
                    {
                        allcolumns += entity.TableName + "." + item.TableColumnName + " as[" + entity.TableName + "." + item.TableColumnName + "], ";
                        if (item.IsFK)
                        {
                            foreach (EntityClassInfo fkentity in GlobalProperty.entitysInfo)
                            {
                                if (fkentity.ModelClassName.ToString() == item.TypeName.ToString())
                                {
                                    foreach (var fkitem in fkentity.Fields)
                                    {
                                        allcolumns +=fkentity.TableName+"."+ fkitem.TableColumnName+" as [" + fkentity.TableName + "." + fkitem.TableColumnName + "], ";
                                    }
                                    alltables += " left join " + fkentity.TableName + " on " + entity.TableName + "." + item.TableColumnName + " = " + fkentity.TableName+"."+fkentity.PrimaryKey;
                                    break;
                                }
                            }
                        }
                    }
                    if (allcolumns.EndsWith(", "))
                        allcolumns = allcolumns.Substring(0, allcolumns.Length - 2);
                    PKSelectSql = String.Format("SELECT {0} FROM {1} where {2} ='\" + PrimaryKeyId + \" '", allcolumns, alltables, entity.PrimaryKey);
                    WhereSelectSql = string.Format("{0} FROM {1}", allcolumns,alltables);
                }
                else
                {
                    PKSelectSql = String.Format("SELECT * FROM {0} where {1} ='\" + PrimaryKeyId + \" '", entity.TableName, entity.PrimaryKey);
                    WhereSelectSql = string.Format("* FROM {0}", entity.TableName);
                }
                
                #endregion

                AccessBase = AccessBase.Replace("#AllAddColumns#", AllAddColumns);
                AccessBase = AccessBase.Replace("#AllAddValues#", AllAddValues);
                AccessBase = AccessBase.Replace("#AllFields#", AllFields);
                AccessBase = AccessBase.Replace("#AllUpdateFields#", AllUpdateFields);
                AccessBase = AccessBase.Replace("#AllUpdateColumns#", AllUpdateColumns);
                AccessBase = AccessBase.Replace("#LastFieldsIndex#", LastFieldsIndex.ToString());
                AccessBase = AccessBase.Replace("#PKSelectSql#", PKSelectSql.ToString());
                AccessBase = AccessBase.Replace("#WhereSelectSql#", WhereSelectSql.ToString());
                AccessBase = AccessBase.Replace("#PagerSelectSql#", WhereSelectSql.ToString());
                
                TextWriter tw = new StreamWriter(new FileStream(GlobalProperty.DataBaseAccessLibraryClassInfo.FloderPath + "/" + entity.ClassName + "DataBaseAccess.cs", FileMode.Create, FileAccess.Write));
                tw.WriteLine(AccessBase);
                tw.Close();
                
            }
        }
        /// <summary>
        /// 准备dbhelper
        /// </summary>
        public static void InitDBHelper()
        {
            string DBHelper = File.ReadAllText("ResourceFile/DBHelper/DBHelper.cs");
            TextWriter tw = new StreamWriter(new FileStream(GlobalProperty.DataBaseAccessLibraryClassInfo.FloderPath + "/DBHelper/" + "DBHelper.cs", FileMode.Create, FileAccess.Write));
            tw.WriteLine(DBHelper);
            tw.Close();

            string DBHelper_Static = File.ReadAllText("ResourceFile/DBHelper/DBHelper_Static.cs");
            TextWriter tws = new StreamWriter(new FileStream(GlobalProperty.DataBaseAccessLibraryClassInfo.FloderPath + "/DBHelper/" + "DBHelper_Static.cs", FileMode.Create, FileAccess.Write));
            tws.WriteLine(DBHelper_Static);
            tws.Close();

            string HelperBase = File.ReadAllText("ResourceFile/DBHelper/HelperBase.cs");
            TextWriter twb = new StreamWriter(new FileStream(GlobalProperty.DataBaseAccessLibraryClassInfo.FloderPath + "/DBHelper/" + "HelperBase.cs", FileMode.Create, FileAccess.Write));
            twb.WriteLine(HelperBase);
            twb.Close();
        }
        /// <summary>
        /// 准备继承的AccessBase
        /// </summary>
        public static void InitAccessBase()
        {
            string AccessBase = File.ReadAllText("ResourceFile/Template/AccessBase.txt");
            AccessBase = AccessBase.Replace("#namespace#", GlobalProperty.DataBaseAccessLibraryClassInfo.NameSpace);
            TextWriter tw = new StreamWriter(new FileStream(GlobalProperty.DataBaseAccessLibraryClassInfo.FloderPath + "/AccessBase.cs", FileMode.Create, FileAccess.Write));
            tw.WriteLine(AccessBase);
            tw.Close();
        }

        /*****************************生成MVC界面文件******************************/
        public static void WriteController()
        {
            //控制器Base类
            string controlBase = File.ReadAllText("ResourceFile/MVCTemplate/ControllerBaseFile.txt");
            controlBase = controlBase.Replace("#SolutionName#", GlobalProperty.SolutionName);


            TextWriter tw = new StreamWriter(new FileStream(GlobalProperty.ManagementWebClassInfo.FloderPath + "/Controllers/" + "BaseController.cs", FileMode.Create, FileAccess.Write));
            tw.WriteLine(controlBase);
            tw.Close();
            //控制器
            foreach (var entity in GlobalProperty.entitysInfo)
            {
                controlBase = File.ReadAllText("ResourceFile/MVCTemplate/ControllerFile.txt");
                controlBase = controlBase.Replace("#SolutionName#", GlobalProperty.SolutionName);
                controlBase = controlBase.Replace("#ClassName#", entity.ClassName);
                controlBase = controlBase.Replace("#ModelName#", entity.ModelClassName);
                controlBase = controlBase.Replace("#PrimaryKeyName#", entity.PrimaryKey);
                controlBase = controlBase.Replace("#DefaultOrder#", entity.PrimaryKey);

                tw = new StreamWriter(new FileStream(GlobalProperty.ManagementWebClassInfo.FloderPath + "/Controllers/" + entity.ClassName + "Controller.cs", FileMode.Create, FileAccess.Write));
                tw.WriteLine(controlBase);
                tw.Close();
            }
        }

        public static void WriteViewHtml()
        {
            foreach (var entity in GlobalProperty.entitysInfo)
            {
                //列表页面
                string IndexHtmlBase = File.ReadAllText("ResourceFile/MVCTemplate/ViewIndex.txt");
                IndexHtmlBase = IndexHtmlBase.Replace("#ClassName#", entity.ClassName);
                IndexHtmlBase = IndexHtmlBase.Replace("#ModelName#", entity.ModelClassName);
                IndexHtmlBase = IndexHtmlBase.Replace("#PrimaryKeyName#", entity.PrimaryKey);
                IndexHtmlBase = IndexHtmlBase.Replace("#SolutionName#", GlobalProperty.SolutionName);
                string THHtmlStr = string.Empty;
                string TDHtmlStr = string.Empty;
                foreach (var item in entity.Fields)
                {
                    //如果是外键
                    if (item.IsFK)
                    {
                    }
                    else if (item.FieldName!=entity.PrimaryKey)
                    {
                        THHtmlStr += "<th class=\"sorting\"  aria-controls=\"sample_3\"  >"+item.FieldName+" </th>";
                        TDHtmlStr += "<td>@Model[i]."+item.FieldName+"</td>";
                    }

                }
                IndexHtmlBase = IndexHtmlBase.Replace("#THHtmlStr#", THHtmlStr);
                IndexHtmlBase = IndexHtmlBase.Replace("#TDHtmlStr#", TDHtmlStr);

                if (!Directory.Exists(GlobalProperty.ManagementWebClassInfo.FloderPath + "/Views/" + entity.ClassName))
                {
                    Directory.CreateDirectory(GlobalProperty.ManagementWebClassInfo.FloderPath + "/Views/" + entity.ClassName);
                }
                TextWriter tw = new StreamWriter(new FileStream(GlobalProperty.ManagementWebClassInfo.FloderPath + "/Views/" + entity.ClassName + "/Index.cshtml", FileMode.Create, FileAccess.Write));
                tw.WriteLine(IndexHtmlBase);
                tw.Close();
                //详细信息页面
                string DetailsHtmlBase = File.ReadAllText("ResourceFile/MVCTemplate/ViewDetails.txt");
                DetailsHtmlBase = DetailsHtmlBase.Replace("#ClassName#", entity.ClassName);
                DetailsHtmlBase = DetailsHtmlBase.Replace("#ModelName#", entity.ModelClassName);
                DetailsHtmlBase = DetailsHtmlBase.Replace("#PrimaryKeyName#", entity.PrimaryKey);
                DetailsHtmlBase = DetailsHtmlBase.Replace("#SolutionName#", GlobalProperty.SolutionName);
                string RowHtmlStr = string.Empty;
                string ValidataHtmlStr = string.Empty;
                foreach (var item in entity.Fields)
                {
                    //如果是外键
                    if (item.IsFK)
                    {
                        RowHtmlStr += "@Html.HiddenFor(x => x." + item.FieldName+"."+item.FieldName + ")";
                    }
                    //非主键
                    else if (item.FieldName != entity.PrimaryKey)
                    {
                        RowHtmlStr += "<div class=\"form-group\"><label class=\"control-label col-md-3\">"
                            + item.FieldName +
                        "<span class=\"required\" aria-required=\"true\">* </span></label><div class=\"col-md-4\">" +
                        "<div class=\"input-icon right\"><i class=\"fa\"></i>@Html.TextBoxFor(x => x."
                        + item.FieldName + ", new { @class = \"form-control\" })" +
                        @"</div>
                    </div>
                </div>";

                        ValidataHtmlStr += item.FieldName + @": {
                    minlength: 1,
                    required: true
                },";
                    }
                    //主键
                    else if (item.FieldName == entity.PrimaryKey)
                    {

                        RowHtmlStr += "@Html.HiddenFor(x => x."+item.FieldName+")";
                    }

                }
                DetailsHtmlBase = DetailsHtmlBase.Replace("#RowHtmlStr#", RowHtmlStr);
                DetailsHtmlBase = DetailsHtmlBase.Replace("#ValidataHtmlStr#", ValidataHtmlStr);

                if (!Directory.Exists(GlobalProperty.ManagementWebClassInfo.FloderPath + "/Views/" + entity.ClassName))
                {
                    Directory.CreateDirectory(GlobalProperty.ManagementWebClassInfo.FloderPath + "/Views/" + entity.ClassName);
                }
                TextWriter dtw = new StreamWriter(new FileStream(GlobalProperty.ManagementWebClassInfo.FloderPath + "/Views/" + entity.ClassName + "/"+ entity.ClassName +"Info.cshtml", FileMode.Create, FileAccess.Write));
                dtw.WriteLine(DetailsHtmlBase);
                dtw.Close();
            }
        }

        public static void CopyFileToMVCWeb()
        {
            string FileStr = File.ReadAllText("ResourceFile/MVCTemplate/Global.asax.txt");
            FileStr = FileStr.Replace("#NameSpace#", GlobalProperty.ManagementWebClassInfo.NameSpace);
            TextWriter tw = new StreamWriter(new FileStream(GlobalProperty.ManagementWebClassInfo.FloderPath + "/Global.asax", FileMode.Create, FileAccess.Write));
            tw.WriteLine(FileStr);
            tw.Close();

            FileStr = File.ReadAllText("ResourceFile/MVCTemplate/Global.asax.cs.txt");
            FileStr = FileStr.Replace("#NameSpace#", GlobalProperty.ManagementWebClassInfo.NameSpace);
            tw = new StreamWriter(new FileStream(GlobalProperty.ManagementWebClassInfo.FloderPath + "/Global.asax.cs", FileMode.Create, FileAccess.Write));
            tw.WriteLine(FileStr);
            tw.Close();
            
            //无需修改内容的文件
            System.IO.File.Copy("ResourceFile/MVCTemplate/Web.config", GlobalProperty.ManagementWebClassInfo.FloderPath + "/Web.config", true);
            System.IO.File.Copy("ResourceFile/MVCTemplate/Views/Web.config", GlobalProperty.ManagementWebClassInfo.FloderPath + "/Views/Web.config", true);
            System.IO.File.Copy("ResourceFile/MVCTemplate/fav.ico", GlobalProperty.ManagementWebClassInfo.FloderPath + "/fav.ico", true);
            CommonFunction.CopyDir("ResourceFile/MVCTemplate/Scripts", GlobalProperty.ManagementWebClassInfo.FloderPath + "/Scripts");
            CommonFunction.CopyDir("ResourceFile/MVCTemplate/Css", GlobalProperty.ManagementWebClassInfo.FloderPath + "/Content/Css");
            CommonFunction.CopyDir("ResourceFile/MVCTemplate/fonts", GlobalProperty.ManagementWebClassInfo.FloderPath + "/Content/Fonts");
            CommonFunction.CopyDir("ResourceFile/MVCTemplate/Views", GlobalProperty.ManagementWebClassInfo.FloderPath + "/Views");
            CommonFunction.CopyDir("ResourceFile/MVCTemplate/Img", GlobalProperty.ManagementWebClassInfo.FloderPath + "/Content/Img");
            CommonFunction.CopyDir("ResourceFile/MVCTemplate/packages", GlobalProperty.SolutionFloderPath + "/packages");

            //修改_layout.cshtml模板，添加菜单
            FileStr = File.ReadAllText("ResourceFile/MVCTemplate/Views/Shared/_Layout.cshtml");
            string MenuStr = "";
            foreach (var item in GlobalProperty.entitysInfo)
            {
                MenuStr += "<li class=\"nav-item  \"><a href=\"javascript:ToURL('/" + item.ClassName + "');\" class=\"nav-link \">" +
                                "<i class=\"icon-docs\"></i><span class=\"title\">" + item.ClassName + "</span><span class=\"arrow\"></span></a></li>";           
            }
            FileStr = FileStr.Replace("#ContentMenus#",MenuStr);
            tw = new StreamWriter(new FileStream(GlobalProperty.ManagementWebClassInfo.FloderPath + "/Views/Shared/_Layout.cshtml", FileMode.Create, FileAccess.Write));
            tw.WriteLine(FileStr);
            tw.Close();
        }
        
    }
}
