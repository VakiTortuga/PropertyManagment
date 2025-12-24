using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Enums
{
    public enum AgreementStatus
    {
        Draft,      // Черновик
        Active,     // Действующий
        Completed,  // Завершён (срок истёк)
        Cancelled   // Расторгнут досрочно
    }

}
