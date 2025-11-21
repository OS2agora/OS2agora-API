using System;
using Agora.Operations.Common.Interfaces;

namespace Agora.DAOs.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}