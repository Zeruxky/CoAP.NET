// Copyright (c) World-Direct eBusiness solutions GmbH. All rights reserved.

namespace WorldDirect.CoAP.V1.Options
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WorldDirect.CoAP.Common;

    /// <summary>
    /// Represents the registry, that holds a set of <see cref="ContentFormat"/>s.
    /// </summary>
    /// <seealso cref="WorldDirect.CoAP.Common.Registry{WorldDirect.CoAP.V1.Options.ContentFormat}" />
    public class ContentFormatRegistry : Registry<ContentFormat>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentFormatRegistry"/> class.
        /// </summary>
        /// <param name="contentFormats">The set of <see cref="ContentFormat"/>s that should be stored in that <see cref="ContentFormatRegistry"/>.</param>
        public ContentFormatRegistry(IEnumerable<ContentFormat> contentFormats)
            : base(contentFormats)
        {
        }
    }
}
