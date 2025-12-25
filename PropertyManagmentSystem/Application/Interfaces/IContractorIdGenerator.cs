using PropertyManagmentSystem.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Application.Interfaces
{
    public interface IContractorIdGenerator
    {
        int GetNextId();
    }
}
