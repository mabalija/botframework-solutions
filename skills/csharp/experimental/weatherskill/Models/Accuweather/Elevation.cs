﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherSkill.Models
{
    public class Elevation
    {
        public Metric Metric { get; set; }

        public Imperial Imperial { get; set; }
    }
}
