using System;
using System.Collections.Generic;
using System.Text;

namespace Agora.DAOs.Esdh
{
    public interface IEsdhServiceOptions
    {
        bool IsMocked { get; }
    }
    
    public class EsdhServiceOptions : IEsdhServiceOptions
    {
        public EsdhServiceOptions(bool isMocked = false)
        {
            this.IsMocked = isMocked;
        }
        public bool IsMocked { get; }
    }
}
