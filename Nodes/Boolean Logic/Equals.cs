﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Nodes.Boolean_Logic
{
    [NodeName("Equals")]
    [NodeCategory("Boolean Logic")]
    public class Equals : NodeBase
    {
        [ExposedInput(LabelDisplay.Hidden)]
        public DataTypes.Numeric inputOne;

        [ExposedInput(LabelDisplay.Hidden)]
        public DataTypes.Numeric inputTwo;

        [ExposedOutput(LabelDisplay.Hidden)]
        public DataTypes.Boolean output;

        public override void NodeFunction()
        {
            output = new DataTypes.Boolean(inputOne.value == inputTwo.value);
        }
    }
}
