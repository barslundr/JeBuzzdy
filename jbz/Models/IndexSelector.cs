using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jbz.Models
{
    public class IndexSelector
    {
        private int _column;
        private int _row;

        public int Column
        {
            get { return _column; }
            set { _column = value; }
        }

        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }
    }
}
