﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyber_base.utils
{
    public interface ICancelable
    {
        void OnCancel();
    }
}