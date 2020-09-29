using HealthTrackerProcessor.Class;
using HealthTrackerProcessor.Models;
using HealthTrackerProcessorCore.Repositories;
using Microsoft.Extensions.Logging;
using System;

namespace HealthTrackerProcessor.Repositories
{
    public class HealthDataRepository : BaseRepository<HealthData>
    {
        private readonly ILogger _logger;
        private readonly ConfigContext _context;
        public HealthDataRepository(ConfigContext context, ILogger<HealthDataRepository> logger) : base(context, logger)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            string sErr = "";
            try
            {
                _logger = logger;
                _context = context;
            }
            catch
            {
                goto Get_Out;
            }

        Get_Out:

            if (sErr.Length > 0)
            {
                _logger.LogError(sErr);
            }
        }
    }
}
