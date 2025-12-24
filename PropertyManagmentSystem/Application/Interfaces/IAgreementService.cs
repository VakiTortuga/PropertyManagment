using PropertyManagmentSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Application.Interfaces
{
    public interface IAgreementService
    {
        // Основные CRUD операции
        AgreementDto GetAgreementById(int id);
        IEnumerable<AgreementDto> GetAllAgreements();
        void CreateAgreement(AgreementDto agreementDto);
        void UpdateAgreement(AgreementDto agreementDto);
        void DeleteAgreement(int id);

        // Управление статусами
        void SignAgreement(int id);
        void CancelAgreement(int id, string reason);
        void CompleteAgreement(int id);

        // Пролонгация (клонирование с новыми датами)
        AgreementDto ProlongAgreement(int existingAgreementId, DateTime newStartDate, DateTime newEndDate);

        // Управление арендуемыми помещениями в договоре
        void AddRentedItemToAgreement(int agreementId, RentedItemDto rentedItemDto);
        void RemoveRentedItemFromAgreement(int agreementId, int roomId);

        // Отчетность
        IEnumerable<AgreementDto> GetActiveAgreements();
        IEnumerable<AgreementDto> GetExpiringAgreements(DateTime withinDays);
        decimal CalculateTotalMonthlyRent(int agreementId);
        decimal CalculatePenalty(int agreementId);
    }
}
