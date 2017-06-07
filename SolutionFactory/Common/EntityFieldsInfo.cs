using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolutionFactory
{
    public class EntityFieldsInfo
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { set; get; }
        /// <summary>
        /// 数据库对应的列名
        /// </summary>
        public string TableColumnName { set; get; }
        /// <summary>
        /// 类型
        /// </summary>
        public string TypeName { set; get; }
        /// <summary>
        /// 是否是外键
        /// </summary>
        public bool IsFK { set; get; }
    }
}
