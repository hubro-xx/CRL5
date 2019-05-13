/**
* CRL 快速开发框架 V5
* Copyright (c) 2019 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL5
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Sharding.DB
{
    /// <summary>
    /// 自增编号
    /// 主数据自增编号(存在于索引库)
    /// </summary>
    public class DataSequence : CRL.IModelBase
    {
        /// <summary>
        /// 源表名
        /// </summary>
        public string TableName
        {
            get;
            set;
        }
        /// <summary>
        /// 自增编号
        /// </summary>
        public long Sequence
        {
            get;
            set;
        }
    }
}
