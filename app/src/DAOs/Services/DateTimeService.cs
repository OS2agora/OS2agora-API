using System;
using BallerupKommune.Operations.Common.Interfaces;

namespace BallerupKommune.DAOs.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}