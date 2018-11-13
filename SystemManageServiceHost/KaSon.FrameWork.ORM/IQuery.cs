using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    public interface IQuery
    {

        IQuery SetString(string name, string value);
        IQuery SetInt(string name, int value);
        IQuery SetDecimal(string name, decimal value);

        //IQuery OutInt(string name,out int Num);

        //IQuery OutString(string name, out string Num);

        //IQuery OutDecimal(string name, out decimal Num);

        /// <summary>
        /// 是否一直保持连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isLink"></param>
        /// <returns></returns>
        IList<T> List<T>(bool isLink=false);

        int Excute();

       T First<T>();
     
        IQuery SetEntity(Type type);

    }
}
