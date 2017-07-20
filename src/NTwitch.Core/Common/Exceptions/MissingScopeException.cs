﻿using System;
using System.Collections.Generic;

namespace NTwitch
{
    public class MissingScopeException : ArgumentException
    {
        public MissingScopeException(string scope)
            : this(new[] { scope }) { }
        public MissingScopeException(IEnumerable<string> scopes)
            : base($"Missing required scopes: {string.Join(", ", scopes)}") { }
    }
}
