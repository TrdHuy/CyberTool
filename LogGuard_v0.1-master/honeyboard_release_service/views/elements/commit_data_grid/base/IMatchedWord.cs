﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace honeyboard_release_service.views.elements.commit_data_grid.@base
{
    public interface IMatchedWord
    {
        string SearchWord { get; }
        int StartIndex { get; }
        string RawWord { get; }
        bool IsMatch { get; }
    }
}
