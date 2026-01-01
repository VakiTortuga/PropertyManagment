using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyManagmentSystem.Application.DTOs;
using PropertyManagmentSystem.Application.Requests;

namespace PropertyManagmentSystem.Application.Interfaces
{
    public interface IContractorService
    {
        // Общие методы 
        ContractorDto GetContractorById(int id);
        IEnumerable<ContractorDto> GetAllContractors();

        // Добавление арендаторов
        void AddIndividualContractor(CreateIndividualContractorRequest request);
        void AddLegalEntityContractor(CreateLegalEntityContractorRequest request);

        // Обновление арендатора
        void UpdateContractorPhone(UpdateContractorPhoneRequest request);
        // Договора
        IEnumerable<AgreementDto> GetContractorAgreements(int contractorId);
        bool CanContractorCreateNewAgreement(int contractorId);

        // Событие, оповещающее об изменениях в списке арендаторов
        event Action ContractorsChanged;

        // Позволяет другим сервисам уведомить об изменениях
        void NotifyContractorsChanged();
    }
}
