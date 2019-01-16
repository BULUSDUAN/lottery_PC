namespace KaSon.FrameWork.ORM.Dal
{
    using KaSon.FrameWork.Services.ORM;
    using System;
    using System.Runtime.CompilerServices;

    internal class LambdaContext : Context
    {
        private readonly DbParameterCollection _parameters;
        private int _paramIndex;

        public LambdaContext(object host) : base(host)
        {
            this._parameters = new DbParameterCollection();
        }

        public string CountText { get; set; }

        public string DeleteText { get; set; }

        public EntityInfo Entity { get; set; }

        public DbParameterCollection Parameters
        {
            get
            {
                return this._parameters;
            }
        }

        public int ParamIndex
        {
            get
            {
                return this._paramIndex++;
            }
        }

        public string SelectText { get; set; }

        public string SumText { get; set; }

        public string UpdateText { get; set; }

        public string WhereText { get; set; }
    }
}

