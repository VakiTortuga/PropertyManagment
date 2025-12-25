using PropertyManagmentSystem.Application.DTOs;
using PropertyManagmentSystem.Application.Requests;
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
        void CreateAgreement(CreateAgreementRequest request);
        void UpdateAgreement(UpdateAgreementRequest request);
        void DeleteAgreement(int id);

        // Управление статусами
        void SignAgreement(int id);
        void CancelAgreement(int id, string reason);
        void CompleteAgreement(int id);

        // Пролонгация (клонирование с новыми датами)
        AgreementDto ProlongAgreement(ProlongAgreementRequest request);

        // Управление арендуемыми помещениями в договоре
        void AddRentedItemToAgreement(AddRentedItemRequest request);
        void RemoveRentedItemFromAgreement(int agreementId, int roomId);

        // Отчетность
        IEnumerable<AgreementDto> GetActiveAgreements();
        IEnumerable<AgreementDto> GetExpiringAgreements(DateTime withinDays);
        decimal CalculateTotalMonthlyRent(int agreementId);
        decimal CalculatePenalty(int agreementId);
    }
}
