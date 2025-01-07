using PiketWebApi.Data;

namespace PiketWebApi.Abstractions
{
    public static class OrderColumnExtention
    {
        public static IQueryable<Student> GetStudentOrder(this IQueryable<Student> query, string columnName, string sortOrder)
        {
            if (sortOrder?.ToLower() == "desc")
            {
                return columnName?.ToLower() switch
                {
                    "name" => query.OrderByDescending(x => x.Name),
                    "nis" => query.OrderByDescending(x => x.NIS),
                    "nisn" => query.OrderByDescending(x => x.NISN),
                    _ => query.OrderByDescending(x => x.Id)
                };
            }
            else
            {
                return columnName?.ToLower() switch
                {
                    "name" => query.OrderBy(x => x.Name),
                    "nis" => query.OrderBy(x => x.NIS),
                    "nisn" => query.OrderBy(x => x.NISN),
                    _ => query.OrderBy(x => x.Id)
                };
            }
        }

        public static IQueryable<Picket> GetPicketOrder(this IQueryable<Picket> query, string columnName, string sortOrder)
        {
            if (sortOrder?.ToLower() == "desc")
            {
                return columnName?.ToLower() switch
                {
                    "createby" => query.OrderByDescending(x => x.CreatedBy.Name),
                    _ => query.OrderByDescending(x => x.Id)
                };
            }
            else
            {
                return columnName?.ToLower() switch
                {
                    "createby" => query.OrderBy(x => x.CreatedBy.Name),
                    _ => query.OrderBy(x => x.Id)
                };
            }
        }
    }
}
