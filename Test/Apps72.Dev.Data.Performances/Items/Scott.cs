using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps72.Dev.Data.Performances
{
    /// <summary />
    public partial class EMP
    {
        /// <summary />
        public virtual Int32 EMPNO { get; set; }
        /// <summary />
        public virtual String ENAME { get; set; }
        /// <summary />
        public virtual String JOB { get; set; }
        /// <summary />
        public virtual Int32? MGR { get; set; }
        /// <summary />
        public virtual DateTime? HIREDATE { get; set; }
        /// <summary />
        public virtual Int32? SAL { get; set; }
        /// <summary />
        public virtual Int32? COMM { get; set; }
        /// <summary />
        public virtual Int32? DEPTNO { get; set; }
    }
}
