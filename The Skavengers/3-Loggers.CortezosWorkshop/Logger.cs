﻿
using _1_Domain.CortezosWorkshop.Interfaces;
using _2_Application.TheSkavengers.Services;
using _3_Data.CortezosWorkshop;
using _3_Data.CortezosWorkshop.Models;
using Microsoft.EntityFrameworkCore;

namespace _3_Loggers
{
    public class Logger : ILogger
    {
        private readonly AppDbContext _context;
        private readonly ConstantsConfigurationService _configuration;

        public Logger(AppDbContext context, 
            ConstantsConfigurationService configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IEnumerable<string>> GetLogEntriesAsync()
        {
            var logModelEntries = await _context.Logs.ToListAsync();

            var logEntries = new List<string>();

            foreach (var logEntry in logModelEntries) { logEntries.Add(logEntry.Entry); } 

            return logEntries;
        }

        public async Task WriteLogEntryAsync(string logEntry)
        {
            var logModel = new LogModel 
            { 
                Entry = logEntry,
                Date = DateTime.Now
            };

            await _context.Logs.AddAsync(logModel);

            await ManageDeleteLogEntriesAsync();

            await _context.SaveChangesAsync();
        }

        private async Task ManageDeleteLogEntriesAsync()
        {
            var logs = await _context.Logs.ToListAsync();

            var _LOG_MAX_ENTRIES = _configuration.GetInt("Constants:_LOG_MAX_ENTRIES");
            var _NUMBER_OF_ENTRIES_TO_DELETE = _configuration.GetInt("Constants:_LOG_DELETE_ENTRIES_WHEN_MAX_REACHED");

            if (logs.Count >= _LOG_MAX_ENTRIES)
            {
                var logsToDelete = logs.OrderBy(log => log.Date)
                                       .Take(_NUMBER_OF_ENTRIES_TO_DELETE);

               _context.Logs.RemoveRange(logsToDelete);
            }
        }

        // Genera un prefijo WRN para la linea de log si los datos nuevos superan un % de los datos actuales.
        public string GenerateLogWarning(string logEntry, decimal actualData, decimal newData)
        {
            var _DEVIATION_PERCENTAGE_FOR_LOG_WARNING = _configuration.GetInt("Constants:_DEVIATION_PERCENTAGE_FOR_LOG_WARNING");

            decimal deviationAllowed = actualData * _DEVIATION_PERCENTAGE_FOR_LOG_WARNING / 100;

            if (Math.Abs(actualData - newData) > deviationAllowed) { return $"WRN: {logEntry}"; }

            return logEntry;
        }
    }
}
