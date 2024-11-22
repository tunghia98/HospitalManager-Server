using EHospital.DTO;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Libs.Filters;

namespace HospitalManagementSystem.QueryObjects
{
    public class InvoiceQuery : PaginationQuery, IFilterObject<Invoice>
    {

        public NumberFilter<int> InvoiceId { get; set; } = new NumberFilter<int>();

        public NumberFilter<int> PatientId { get; set; } = new NumberFilter<int>();

        public NumberFilter<int> AppointmentId { get; set; } = new NumberFilter<int>();

        public DateTimeFilter InvoiceDate { get; set; } = new DateTimeFilter();

        public NumberFilter<decimal> TotalAmount { get; set; } = new NumberFilter<decimal>();

        public StringFilter Status { get; set; } = new StringFilter();

        public IQueryable<Invoice> ApplyFilter(IQueryable<Invoice> query)
        {
            query = InvoiceId.ApplyFilter(query, x => x.InvoiceId);
            query = PatientId.ApplyFilter(query, x => x.PatientId);
            query = AppointmentId.ApplyFilter(query, x => x.AppointmentId!.Value);
            query = InvoiceDate.ApplyFilter(query, x => x.InvoiceDate);
            query = TotalAmount.ApplyFilter(query, x => x.TotalAmount);
            query = Status.ApplyFilter(query, x => x.Status);
            return query;
        }
    }
}