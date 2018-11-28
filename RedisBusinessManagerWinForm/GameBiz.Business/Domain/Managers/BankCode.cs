using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Business.Domain.Managers
{
    /// <summary>
    /// 中国工商银行	1	ICBC
    ///    招商银行	2	CMB
    //   中国建设银行  3	CCB
    //   中国农业银行  4	ABC
    //   中国银行    5	BOC
    //   交通银行    6	BCM
    //  中国民生银行  7	CMBC
    //  中信银行    8	ECC
    //   上海浦东发展银行    9	SPDB
    //  邮政储汇    10	PSBC
    //  中国光大银行  11	CEB
    // 平安银行 （原深圳发展银行）	12	PINGAN
    // 广发银行股份有限公司  13	CGB
    // 华夏银行    14	HXB
    // 福建兴业银行  15	CIB

    /// </summary>
    public enum BankCode : int
    {
        ICBC = 1,
        CMB = 2,
        CCB = 3,
        ABC = 4,
        BOC = 5,
        BCM = 6,
        CMBC = 7,
        ECC = 8,
        SPDB = 9,
        PSBC = 10,
        CEB = 11,
        PINGAN = 12,
        CGB = 13,
        HXB = 14,
        CIB = 15
    }
}
