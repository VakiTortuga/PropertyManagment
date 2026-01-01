using System;
using System.Collections.Generic;
using PropertyManagmentSystem.Application.DTOs;
using PropertyManagmentSystem.Application.Requests;

namespace PropertyManagmentSystem.Application.Interfaces
{
    public interface IAgreementService
    {
        // События для уведомления UI
        event Action AgreementsChanged;
        event Action RoomsChanged;

        AgreementDto GetAgreementById(int id);
        IEnumerable<AgreementDto> GetAllAgreements();
        void CreateAgreement(CreateAgreementRequest request);
        void UpdateAgreement(UpdateAgreementRequest request);
        void DeleteAgreement(int id);
        void SignAgreement(int id);
        void CancelAgreement(int id, string reason);
        void CompleteAgreement(int id);
        AgreementDto ProlongAgreement(ProlongAgreementRequest request);
        void AddRentedItemToAgreement(AddRentedItemRequest request);
        void RemoveRentedItemFromAgreement(int agreementId, int roomId);
        IEnumerable<AgreementDto> GetActiveAgreements();
        IEnumerable<AgreementDto> GetExpiringAgreements(DateTime withinDays);
        decimal CalculateTotalMonthlyRent(int agreementId);
        decimal CalculatePenalty(int agreementId);
        // Дополнительно: возможность привязать часы (реализуется внутри сервиса)
        void AttachClock(IClock clock);
        void ProcessExpirations(DateTime now);
    }
}
