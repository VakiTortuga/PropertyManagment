using PropertyManagmentSystem.Application.Interfaces;
using PropertyManagmentSystem.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Application.Services
{
    public class ContractorIdGenerator : IContractorIdGenerator
    {
        private int _currentId;

        public ContractorIdGenerator(
            IIndividualContractorRepository individualRepo,
            ILegalEntityContractorRepository legalRepo)
        {
            var maxIndividual = individualRepo.GetAll().Select(c => c.Id).DefaultIfEmpty(0).Max();
            var maxLegal = legalRepo.GetAll().Select(c => c.Id).DefaultIfEmpty(0).Max();
            _currentId = Math.Max(maxIndividual, maxLegal);
        }

        public int GetNextId()
            => Interlocked.Increment(ref _currentId);
    }
}
