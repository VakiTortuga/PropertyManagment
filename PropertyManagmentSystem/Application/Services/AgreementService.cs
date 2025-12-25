using PropertyManagmentSystem.Application.DTOs;
using PropertyManagmentSystem.Application.Interfaces;
using PropertyManagmentSystem.Application.Requests;
using PropertyManagmentSystem.Domains;
using PropertyManagmentSystem.Infrastructure.Interfaces;
using PropertyManagmentSystem.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Application.Services
{
    public class AgreementService : IAgreementService
    {
        private readonly AgreementRepository _agreementRepo;
        private readonly RoomRepository _roomRepo;
        private readonly IIndividualContractorRepository _individualRepo;
        private readonly ILegalEntityContractorRepository _legalRepo;

        public AgreementService(
            AgreementRepository agreementRepo,
            RoomRepository roomRepo,
            IIndividualContractorRepository individualRepo,
            ILegalEntityContractorRepository legalRepo)
        {
            _agreementRepo = agreementRepo;
            _roomRepo = roomRepo;
            _individualRepo = individualRepo;
            _legalRepo = legalRepo;
        }

        public AgreementDto GetAgreementById(int id)
        {
            var agreement = _agreementRepo.GetById(id);
            return agreement == null ? null : MapAgreement(agreement);
        }

        public IEnumerable<AgreementDto> GetAllAgreements()
            => _agreementRepo.GetAll().Select(MapAgreement);

        public void CreateAgreement(CreateAgreementRequest request)
        {
            var id = _agreementRepo.GetNextAvailableId();

            var agreement = new Agreement(
                id,
                request.RegistrationNumber,
                request.StartDate,
                request.EndDate,
                request.PaymentFrequency,
                request.ContractorId,
                request.PenaltyRate);

            if (!_agreementRepo.Add(agreement))
                throw new InvalidOperationException("Не удалось создать договор");
        }

        public void UpdateAgreement(UpdateAgreementRequest request)
        {
            var agreement = _agreementRepo.GetById(request.AgreementId)
                ?? throw new ArgumentException("Договор не найден");

            agreement.Extend(request.EndDate, request.PenaltyRate);
            _agreementRepo.Update(agreement);
        }

        public void DeleteAgreement(int id)
        {
            if (!_agreementRepo.Delete(id))
                throw new InvalidOperationException("Не удалось удалить договор");
        }

        public void SignAgreement(int id)
        {
            var agreement = _agreementRepo.GetById(id);
            agreement.Sign();

            foreach (var item in agreement.RentedItems)
            {
                var room = _roomRepo.GetById(item.RoomId);
                room.Rent(agreement.Id);
                _roomRepo.Update(room);
            }

            _agreementRepo.Update(agreement);
        }

        public void CancelAgreement(int id, string reason)
        {
            var agreement = _agreementRepo.GetById(id);
            agreement.Cancel(reason);

            foreach (var item in agreement.RentedItems)
            {
                var room = _roomRepo.GetById(item.RoomId);
                room.Release();
                _roomRepo.Update(room);
            }

            _agreementRepo.Update(agreement);
        }

        public void CompleteAgreement(int id)
        {
            var agreement = _agreementRepo.GetById(id);
            agreement.Complete();
            _agreementRepo.Update(agreement);
        }

        public AgreementDto ProlongAgreement(ProlongAgreementRequest request)
        {
            var old = _agreementRepo.GetById(request.ExistingAgreementId);

            var newAgreement = new Agreement(
                _agreementRepo.GetNextAvailableId(),
                $"{old.RegistrationNumber}-P",
                request.NewStartDate,
                request.NewEndDate,
                old.PaymentFrequency,
                old.ContractorId,
                old.PenaltyRate);

            _agreementRepo.Add(newAgreement);
            return MapAgreement(newAgreement);
        }

        public void AddRentedItemToAgreement(AddRentedItemRequest request)
        {
            var agreement = _agreementRepo.GetById(request.AgreementId);
            var room = _roomRepo.GetById(request.RoomId);

            if (!room.CanBeRented())
                throw new InvalidOperationException("Комната недоступна");

            agreement.AddRentedItem(
                new RentedItem(
                    request.RoomId,
                    request.Purpose,
                    request.RentUntil,
                    request.RentAmount
                    )
                );

            _agreementRepo.Update(agreement);
        }

        public void RemoveRentedItemFromAgreement(int agreementId, int roomId)
        {
            var agreement = _agreementRepo.GetById(agreementId);
            agreement.RemoveRentedItem(roomId);
            _agreementRepo.Update(agreement);
        }

        public IEnumerable<AgreementDto> GetActiveAgreements()
            => _agreementRepo.GetActiveAgreements().Select(MapAgreement);

        public IEnumerable<AgreementDto> GetExpiringAgreements(DateTime withinDays)
            => _agreementRepo.GetExpiringAgreements(withinDays).Select(MapAgreement);

        public decimal CalculateTotalMonthlyRent(int agreementId)
            => _agreementRepo.GetById(agreementId).CalculateTotalMonthlyRent();

        public decimal CalculatePenalty(int agreementId)
            => _agreementRepo.GetById(agreementId).CalculatePenalty(DateTime.Now);

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
    }

}
