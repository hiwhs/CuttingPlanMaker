﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuttingPlanMaker
{
    /// <summary>
    /// Interface required by all packer implementations
    /// </summary>
    interface IPacker
    {
        /// <summary>
        /// A public method to start the packing of parts onto boards
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="boards"></param>
        /// <param name="sawkerf"></param>
        /// <param name="partLengthPadding"></param>
        /// <param name="partWidthPadding"></param>
        void Pack(Part[] parts, StockItem[] boards, double sawkerf = 3.2, double partLengthPadding = 0, double partWidthPadding = 0);

        /// <summary>
        /// A name for this algorithm implementation
        /// </summary>
        string Name { get; }
    }
}
