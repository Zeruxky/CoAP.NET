﻿/*
 * Copyright (c) 2011-2014, Longxiang He <helongxiang@smeshlink.com>,
 * SmeshLink Technology Co.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY.
 * 
 * This file is part of the CoAP.NET, a CoAP framework in C#.
 * Please see README for more information.
 */

namespace WorldDirect.CoAP.Deduplication
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using Log;
    using Net;

    class SweepDeduplicator : IDeduplicator
    {
        static readonly ILogger log = LogManager.GetLogger(typeof(SweepDeduplicator));

        private ConcurrentDictionary<Exchange.KeyID, Exchange> _incommingMessages
            = new ConcurrentDictionary<Exchange.KeyID, Exchange>();
        private Timer _timer;
        private ICoapConfig _config;

        public SweepDeduplicator(ICoapConfig config)
        {
            _config = config;
        }

        private void Sweep(object state)
        {
            if (log.IsDebugEnabled)
                log.Debug("Start Mark-And-Sweep with " + _incommingMessages.Count + " entries");

            DateTime oldestAllowed = DateTime.Now.AddMilliseconds(-_config.ExchangeLifetime);
            List<Exchange.KeyID> keysToRemove = new List<Exchange.KeyID>();
            foreach (KeyValuePair<Exchange.KeyID, Exchange> pair in _incommingMessages)
            {
                if (pair.Value.Timestamp < oldestAllowed)
                {
                    if (log.IsDebugEnabled)
                        log.Debug("Mark-And-Sweep removes " + pair.Key);
                    keysToRemove.Add(pair.Key);
                }
            }
            if (keysToRemove.Count > 0)
            {
                Exchange ex;
                foreach (Exchange.KeyID key in keysToRemove)
                {
                    _incommingMessages.TryRemove(key, out ex);
                }
            }
        }

        /// <inheritdoc/>
        public void Start()
        {
            _timer = new Timer(Sweep, null, TimeSpan.FromMilliseconds(_config.MarkAndSweepInterval), TimeSpan.FromMilliseconds(_config.MarkAndSweepInterval));
        }

        /// <inheritdoc/>
        public void Stop()
        {
            Dispose();
            Clear();
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _incommingMessages.Clear();
        }

        /// <inheritdoc/>
        public Exchange FindPrevious(Exchange.KeyID key, Exchange exchange)
        {
            Exchange prev = null;
            _incommingMessages.AddOrUpdate(key, exchange, (k, v) =>
            {
                prev = v;
                return exchange;
            });
            return prev;
        }

        /// <inheritdoc/>
        public Exchange Find(Exchange.KeyID key)
        {
            Exchange prev;
            _incommingMessages.TryGetValue(key, out prev);
            return prev;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
