﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSender.Core;
using Equinor.ProCoSys.BusSender.Infrastructure.Data;
using Equinor.ProCoSys.BusSenderWorker.Core;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Equinor.ProCoSys.BusSender.Infrastructure.Repositories
{
    public class BusEventRepository : IBusEventRepository
    {
        private readonly int _messageChunkSize;
        private readonly DbSet<BusEvent> _busEvents;

        public BusEventRepository(BusSenderServiceContext context, IConfiguration configuration)
        {
            _messageChunkSize = int.Parse(configuration["MessageChunkSize"]);
            _busEvents = context.BusEvents;
        }

        public async Task<List<BusEvent>> GetEarliestUnProcessedEventChunk()
        {
            var events = await _busEvents.Where(e => e.Sent == Status.UnProcessed).OrderBy(e => e.Created).Take(_messageChunkSize).ToListAsync();

            return events;
        }
    }
}
