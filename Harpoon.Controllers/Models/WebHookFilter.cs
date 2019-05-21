﻿using System;

namespace Harpoon.Controllers.Models
{
    /// <inheritdoc />
    public class WebHookFilter : IWebHookFilter
    {
        /// <inheritdoc />
        public Guid Id { get; set; }
        /// <inheritdoc />
        public string Trigger { get; set; }

        /// <summary>Initializes a new instance of the <see cref="WebHookFilter"/> class.</summary>
        public WebHookFilter() { }
        /// <summary>Initializes a new instance of the <see cref="WebHookFilter"/> class.</summary>
        public WebHookFilter(IWebHookFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            Id = filter.Id;
            Trigger = filter.Trigger;
        }
    }
}