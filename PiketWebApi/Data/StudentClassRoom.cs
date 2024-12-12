using SharedModel;
using SharedModel.Models;

namespace PiketWebApi.Data
{
    public class StudentClassRoom
    {
        public int Id { get; set; }
        public string? NIS { get; set; }
        public string? NISN { get; set; }
        public string? Name { get; set; }
        public Gender Gender { get; set; }
        public string? Photo { get; set; }
        public int? ClassRoomId{ get; set; }
        public string? ClassRoomName { get; set; }
        public int? DepartmenId{ get; internal set; }
        public string? DepartmenName { get; internal set; }
    }
}
