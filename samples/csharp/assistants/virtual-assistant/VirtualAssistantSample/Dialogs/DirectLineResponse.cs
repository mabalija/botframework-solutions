// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualAssistantSample.Dialogs
{
    public class DirectLineResponse
    {
        public string conversationId { get; set; }

        public string token { get; set; }

        public int expires_in { get; set; }
    }
}
