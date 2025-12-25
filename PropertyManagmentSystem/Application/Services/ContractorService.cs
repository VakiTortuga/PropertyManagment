using PropertyManagmentSystem.Application.DTOs;
using PropertyManagmentSystem.Application.Interfaces;
using PropertyManagmentSystem.Application.Requests;
using PropertyManagmentSystem.Domains;
using PropertyManagmentSystem.Enums;
using PropertyManagmentSystem.Infrastructure.Interfaces;
using PropertyManagmentSystem.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Application.Services
{
    public class ContractorService : IContractorService
    {
        private readonly IIndividualContractorRepository _individualRepo;
        private readonly ILegalEntityContractorRepository _legalRepo;
        private readonly AgreementRepository _agreementRepo;
        private readonly IContractorIdGenerator _idGenerator;

        public ContractorService(
            IIndividualContractorRepository individualRepo,
            ILegalEntityContractorRepository legalRepo,
            AgreementRepository agreementRepo,
            IContractorIdGenerator idGenerator
            )
        {
            _individualRepo = individualRepo;
            _legalRepo = legalRepo;
            _agreementRepo = agreementRepo;
            _idGenerator = idGenerator;
        }

        public ContractorDto GetContractorById(int id)
        {
            Contractor individualResult = _individualRepo.GetById(id);
            Contractor legalEntityResult = _legalRepo.GetById(id);
            Contractor c = individualResult ?? legalEntityResult;

            return c == null ? null : MapContractor(c);
        }
        
        public IEnumerable<ContractorDto> GetAllContractors()
            => _individualRepo.GetAll().Cast<Contractor>().
            Concat(_legalRepo.GetAll()).
            Select(MapContractor);

        public void AddIndividualContractor(CreateIndividualContractorRequest request)
        {
            var contractor = new IndividualContractor(
                _idGenerator.GetNextId(),
                request.Phone,
                request.FullName,
                new PassportData(
                    request.Passport.Series,
                    request.Passport.Number,
                    request.Passport.IssueDate,
                    request.Passport.IssuedBy));

            _individualRepo.Add(contractor);
        }

        public void AddLegalEntityContractor(CreateLegalEntityContractorRequest request)
        {
            var contractor = new LegalEntityContractor(
                _legalRepo.GetNextAvailableId(),
                request.Phone,
                request.CompanyName,
                request.DirectorName,
                request.LegalAddress,
                request.TaxId,
                new BankDetails(
                    request.BankDetails.BankName,
                    request.BankDetails.AccountNumber
                    )
                );
            _legalRepo.Add(contractor);
        }

        public void UpdateContractorPhone(UpdateContractorPhoneRequest request)
        {
            var contractor = GetDomainContractor(request.ContractorId);
            contractor.ChangePhone(request.Phone);
            Save(contractor);
        }

        public IEnumerable<AgreementDto> GetContractorAgreements(int contractorId)
            => _agreementRepo.GetAgreementsByContractorId(contractorId)
            .Select(a => new AgreementDto { Id = a.Id });

        public bool CanContractorCreateNewAgreement(int contractorId)
        { 
            var contractor = GetDomainContractor(contractorId);
            return contractor.CanCreateNewAgreement(
                _agreementRepo.GetAll()
                .ToDictionary(a => a.Id)
                );
        }

        private Contractor GetDomainContractor(int id)
        {
            Contractor individualResult = _individualRepo.GetById(id);
            Contractor legalEntityResult = _legalRepo.GetById(id);
            Contractor c = individualResult ?? legalEntityResult;

            return c;
        }

        private void Save(Contractor contractor)
        {
            if (contractor is IndividualContractor i) _individualRepo.Update(i);
            else _legalRepo.Update((LegalEntityContractor)contractor);
        }

        private ContractorDto MapContractor(Contractor c)
            => new ContractorDto {
                Id = c.Id,
                Type = c.Type,
                DisplayName = c.GetDisplayName(), 
                Phone = c.Phone,
                IsActive = c.IsActive,
                RegistrationDate = c.RegistrationDate
            };
    }

}
