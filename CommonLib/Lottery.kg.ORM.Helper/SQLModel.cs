using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Lottery.Kg.ORM.Helper
{

  
    public class SQLModel
    {

        /// <summary>
        /// 唯一标志
        /// </summary>
        public string Key { get; set; }


        /// <summary>
        /// 功能描述
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// 参数描述 
        /// </summary>
        public string ParamDesc { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string Director { get; set; }
        /// <summary>
        /// SQL
        /// </summary>
        public string SQL { get; set; }

        public string Make { get; set; }


    }
}
