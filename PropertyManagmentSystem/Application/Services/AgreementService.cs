using System;
using System.Collections.Generic;
using System.Linq;
using PropertyManagmentSystem.Application.DTOs;
using PropertyManagmentSystem.Application.Interfaces;
using PropertyManagmentSystem.Application.Requests;
using PropertyManagmentSystem.Domains;
using PropertyManagmentSystem.Infrastructure.Interfaces;
using PropertyManagmentSystem.Infrastructure.Repositories;

namespace PropertyManagmentSystem.Application.Services
{
    public class AgreementService : IAgreementService
    {
        private readonly AgreementRepository _agreementRepo;
        private readonly RoomRepository _roomRepo;
        private readonly IIndividualContractorRepository _individualRepo;
        private readonly ILegalEntityContractorRepository _legalRepo;
        private readonly IContractorService _contractorService;

        private IClock _clock;

        public AgreementService(
            AgreementRepository agreementRepo,
            RoomRepository roomRepo,
            IIndividualContractorRepository individualRepo,
            ILegalEntityContractorRepository legalRepo,
            IContractorService contractorService)
        {
            _agreementRepo = agreementRepo;
            _roomRepo = roomRepo;
            _individualRepo = individualRepo;
            _legalRepo = legalRepo;
            _contractorService = contractorService;
        }

        // События для оповещения UI
        public event Action AgreementsChanged;
        public event Action RoomsChanged;

        // Привязать часы (если нужно)
        public void AttachClock(IClock clock)
        {
            if (_clock != null)
                _clock.TimeChanged -= OnClockTimeChanged;

            _clock = clock;

            if (_clock != null)
                _clock.TimeChanged += OnClockTimeChanged;
        }

        private void OnClockTimeChanged(DateTime now)
        {
            try
            {
                ProcessExpirations(now);
            }
            catch
            {
                // лог при необходимости
            }
        }

        public void ProcessExpirations(DateTime now)
        {
            var toComplete = _agreementRepo.GetActiveAgreements()
                .Where(a => a.EndDate <= now)
                .Select(a => a.Id)
                .ToList();

            foreach (var id in toComplete)
            {
                try
                {
                    CompleteAgreement(id);
                }
                catch
                {
                    // продолжаем обработку следующих
                }
            }

            // после пакетной обработки оповестим UI
            AgreementsChanged?.Invoke();
            RoomsChanged?.Invoke();
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

            try
            {
                Contractor contractor = _individualRepo.GetById(request.ContractorId) as Contractor
                    ?? _legalRepo.GetById(request.ContractorId) as Contractor;

                if (contractor == null)
                {
                    _agreementRepo.Delete(id);
                    throw new ArgumentException($"Арендатор с ID {request.ContractorId} не найден");
                }

                contractor.AddAgreement(id);

                if (contractor is IndividualContractor indiv)
                    _individualRepo.Update(indiv);
                else if (contractor is LegalEntityContractor legal)
                    _legalRepo.Update(legal);

                try { _contractorService.NotifyContractorsChanged(); } catch { }
            }
            catch
            {
                if (_agreementRepo.Exists(id))
                    _agreementRepo.Delete(id);
                throw;
            }

            // оповестим UI о новом договоре
            AgreementsChanged?.Invoke();
        }

        public void UpdateAgreement(UpdateAgreementRequest request)
        {
            var agreement = _agreementRepo.GetById(request.AgreementId)
                ?? throw new ArgumentException("Договор не найден");

            agreement.Extend(request.EndDate, request.PenaltyRate);
            _agreementRepo.Update(agreement);

            AgreementsChanged?.Invoke();
        }

        public void DeleteAgreement(int id)
        {
            if (!_agreementRepo.Delete(id))
                throw new InvalidOperationException("Не удалось удалить договор");

            AgreementsChanged?.Invoke();
        }

        public void SignAgreement(int id)
        {
            var now = _clock?.Now ?? DateTime.Now;
            var agreement = _agreementRepo.GetById(id);
            agreement.SignAt(now);

            foreach (var item in agreement.RentedItems)
            {
                var room = _roomRepo.GetById(item.RoomId);
                room.Rent(agreement.Id);
                _roomRepo.Update(room);
            }

            _agreementRepo.Update(agreement);

            // Оповещаем UI — договоры и комнаты изменились
            AgreementsChanged?.Invoke();
            RoomsChanged?.Invoke();
        }

        public void CancelAgreement(int id, string reason)
        {
            var now = _clock?.Now ?? DateTime.Now;
            var agreement = _agreementRepo.GetById(id);
            agreement.CancelAt(reason, now);

            foreach (var item in agreement.RentedItems)
            {
                var room = _roomRepo.GetById(item.RoomId);
                room.Release();
                _roomRepo.Update(room);
            }

            _agreementRepo.Update(agreement);

            AgreementsChanged?.Invoke();
            RoomsChanged?.Invoke();
        }

        public void CompleteAgreement(int id)
        {
            var now = _clock?.Now ?? DateTime.Now;
            var agreement = _agreementRepo.GetById(id);
            agreement.CompleteAt(now);

            foreach (var item in agreement.RentedItems)
            {
                var room = _roomRepo.GetById(item.RoomId);
                if (room != null && room.CurrentAgreementId.HasValue && room.CurrentAgreementId.Value == agreement.Id)
                {
                    room.Release();
                    _roomRepo.Update(room);
                }
            }

            _agreementRepo.Update(agreement);

            AgreementsChanged?.Invoke();
            RoomsChanged?.Invoke();
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

            AgreementsChanged?.Invoke();

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

            AgreementsChanged?.Invoke();
        }

        public void RemoveRentedItemFromAgreement(int agreementId, int roomId)
        {
            var agreement = _agreementRepo.GetById(agreementId);
            agreement.RemoveRentedItem(roomId);
            _agreementRepo.Update(agreement);

            AgreementsChanged?.Invoke();
            RoomsChanged?.Invoke();
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
