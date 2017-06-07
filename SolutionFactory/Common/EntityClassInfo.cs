using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolutionFactory
{
    public class EntityClassInfo
    {
        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { set; get; }
        /// <summary>
        /// 实体类名称
        /// </summary>
        public string ModelClassName { set; get; }
        /// <summary>
        /// 对应的数据库表名
        /// </summary>
        public string TableName { set; get; }
        /// <summary>
        /// 主键字段
        /// </summary>
        public string PrimaryKey { set; get; }
        /// <summary>
        /// 字段信息
        /// </summary>
        public List<EntityFieldsInfo> Fields = new List<EntityFieldsInfo>();
        /// <summary>
        /// 是否有外键
        /// </summary>
        public bool HasFK { set; get; }
    }
}
