using SharedModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicketMobile.Services
{
    public interface IStudentService
    {
        Task<Student> SearchStudent(string searchText);


    }


    public class StudentService : IStudentService
    {
        public Task<Student> SearchStudent(string searchText)
        {
            throw new NotImplementedException();
        }
    }
}
