using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    public class CTZQMatchInfo_Collection : Page
    {

        public CTZQMatchInfo_Collection()
        {
            ListInfo = new List<CTZQMatchInfo>();
        }
        public List<CTZQMatchInfo> ListInfo { get; set; }
    } 
}
