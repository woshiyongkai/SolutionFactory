using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolutionFactory
{
    /// <summary>
    /// 类库信息
    /// </summary>
    public class LibraryClassInfo
    {
        public string NameSpace { set; get; }
        public string FloderPath { set; get; }
        public string Guid { set; get; }
        public string AssemblyFloderPath { set; get; }
        public string[] Classes { set; get; }
    }
}
