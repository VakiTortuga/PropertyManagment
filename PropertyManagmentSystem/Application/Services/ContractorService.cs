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

        public event Action ContractorsChanged;

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
            var contractor = IndividualContractor.Create(
                _idGenerator.GetNextId(),
                request.Phone,
                request.FullName,
                PassportData.Create(
                    request.Passport.Series,
                    request.Passport.Number,
                    request.Passport.IssueDate,
                    request.Passport.IssuedBy));

            _individualRepo.Add(contractor);
            ContractorsChanged?.Invoke();
        }

        public void AddLegalEntityContractor(CreateLegalEntityContractorRequest request)
        {
            var contractor = LegalEntityContractor.Create(
                _idGenerator.GetNextId(),
                request.Phone,
                request.CompanyName,
                request.DirectorName,
                request.LegalAddress,
                request.TaxId,
                BankDetails.Create(
                    request.BankDetails.BankName,
                    request.BankDetails.AccountNumber
                    )
                );
            _legalRepo.Add(contractor);
            ContractorsChanged?.Invoke();
        }

        public void UpdateContractorPhone(UpdateContractorPhoneRequest request)
        {
            var contractor = GetDomainContractor(request.ContractorId);
            contractor.ChangePhone(request.Phone);
            Save(contractor);
            ContractorsChanged?.Invoke();
        }

        public void NotifyContractorsChanged() => ContractorsChanged?.Invoke();

        // Возвращаем полноценные DTO договоров, чтобы окно арендаторов могло их отображать
        public IEnumerable<AgreementDto> GetContractorAgreements(int contractorId)
        {
            var agreements = _agreementRepo.GetAgreementsByContractorId(contractorId);
            return agreements.Select(MapAgreement);
        }

        public bool CanContractorCreateNewAgreement(int contractorId)
        {
            var contractor = GetDomainContractor(contractorId);
            return contractor.CanCreateNewAgreement(
                _agreementRepo.GetAll()
                .ToDictionary(a => a.Id)
                );
        }

        private AgreementDto MapAgreement(Agreement a)
        {
            return new AgreementDto
            {
                Id = a.Id,
                RegistrationNumber = a.RegistrationNumber,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                Status = a.Status,
                ContractorId = a.ContractorId,
                PaymentFrequency = a.PaymentFrequency,
                PenaltyRate = a.PenaltyRate,
                TotalMonthlyRent = a.CalculateTotalMonthlyRent(),
                RentedItems = a.RentedItems.Select(ri => new RentedItemDto
                {
                    RoomId = ri.RoomId,
                    Purpose = ri.Purpose,
                    RentUntil = ri.RentUntil,
                    RentAmount = ri.RentAmount,
                    IsEarlyTerminated = ri.IsEarlyTerminated,
                    ActualVacationDate = ri.ActualVacationDate,
                    EarlyTerminationReason = ri.EarlyTerminationReason
                }).ToList()
            };
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
