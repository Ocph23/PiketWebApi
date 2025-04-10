﻿using ErrorOr;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PiketWebApi.Abstractions;
using PiketWebApi.Data;
using PiketWebApi.Validators;
using SharedModel.Models;
using SharedModel.Requests;
using SharedModel.Responses;
using System.Net.Sockets;
using System.Security.Claims;

namespace PiketWebApi.Services
{
    public interface IPicketService
    {
        Task<ErrorOr<LateAndGoHomeEarlyResponse>> AddStudentToLateComeHomeSoEarly(StudentToLateAndEarlyRequest late);
        Task<ErrorOr<bool>> RemoveStudentToLateComeHomeSoEarly(int id);
        Task<ErrorOr<PicketResponse>> CreateNewPicket();
        Task<ErrorOr<PicketResponse>> GetPicketToday();
        Task<ErrorOr<bool>> UpdatePicket(int id, PicketRequest picket);
        Task<ErrorOr<PicketResponse>> GetById(int id);
        Task<ErrorOr<PaginationResponse<PicketResponse>>> GetWithPaginate(PaginationRequest req);
        Task<ErrorOr<bool>> RemoveDailyJournal(int model);
        Task<ErrorOr<DailyJournalResponse>> AddDailyJournal(DailyJournalRequest model);
        Task<ErrorOr<DailyJournalResponse>> EditDailyJournal(int id, DailyJournalRequest model);
    }


    public class PicketService : IPicketService
    {
        private readonly IHttpContextAccessor http;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly IStudentService studentService;
        private static PicketResponse picketToday;

        public PicketService(IHttpContextAccessor _http, UserManager<ApplicationUser> _userManager,
            ApplicationDbContext _dbContext, IStudentService _studentService)
        {
            http = _http;
            userManager = _userManager;
            dbContext = _dbContext;
            studentService = _studentService;
        }

        public async Task<ErrorOr<PicketResponse>> CreateNewPicket()
        {
            try
            {
                DateOnly dateNow = DateOnly.FromDateTime(DateTime.Now);
                var userClaim = await http.IsTeacherPicket(userManager, dbContext);
                if (!userClaim.Item1)
                    return Error.Unauthorized("Picket", "Maaf, Anda tidak sedang piket/anda tidak memiliki akses !");

                var ppicketToday = dbContext.Picket.SingleOrDefault(x => x.Date == dateNow);
                if (ppicketToday == null)
                {
                    ppicketToday = Picket.Create(userClaim.Item2);
                    dbContext.Entry(ppicketToday.CreatedBy).State = EntityState.Unchanged;
                    dbContext.Picket.Add(ppicketToday);
                    dbContext.SaveChanges();
                }
                return await GetPicketToday();
            }
            catch (Exception)
            {
                return Error.Failure("Maaf , Terjadi Kesalahan!");
            }
        }

        public async Task<ErrorOr<PicketResponse>> GetPicketToday()
        {
            try
            {
                DateOnly date = DateOnly.FromDateTime(DateTime.Now);

                var ppicketToday = dbContext.Picket
                    .Include(x => x.CreatedBy)
                    .Include(x => x.DailyJournals).ThenInclude(x => x.Teacher)
                    .Include(x => x.LateAndComeHomeEarly).ThenInclude(x => x.Student)
                    .Include(x => x.StudentAttendances).ThenInclude(x => x.Student)
                    .FirstOrDefault(x => x.Date == date);
                if (ppicketToday == null || date != ppicketToday.Date)
                {
                    return Error.Failure("Picket", "Piket Belum Di buka");
                }

                var response = await GeneratePicketResponse(ppicketToday);

                return picketToday = response.Value;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<ErrorOr<LateAndGoHomeEarlyResponse>> AddStudentToLateComeHomeSoEarly(StudentToLateAndEarlyRequest late)
        {
            try
            {
                var userClaim = await http.IsTeacherPicket(userManager, dbContext);
                if (!userClaim.Item1)
                    return Error.Unauthorized("Picket", "Maaf, Anda tidak sedang piket/anda tidak memiliki akses !");

                DateOnly date = DateOnly.FromDateTime(DateTime.Now);
                var picket = dbContext.Picket
                        .Include(x => x.LateAndComeHomeEarly)
                        .ThenInclude(x => x.Student)
                        .FirstOrDefault(x => x.Date == date);
                if (picket == null || DateOnly.FromDateTime(DateTime.Now) != picket.Date)
                {
                    return Error.Failure("Picket", "Piket Belum Di buka");
                }

                if (picket.LateAndComeHomeEarly.Any(x => x.Student.Id == late.StudentId && x.LateAndGoHomeEarlyStatus == late.LateAndGoHomeEarlyStatus))
                {
                    return Error.Failure("Picket", $"Siswa sudah di daftarkan !");
                }


                var student = dbContext.Students.Find(late.StudentId);
                if (student == null)
                {
                    return Error.Failure("Picket", "Data siswa tidak ditemukan");
                }


                var toLate = new LateAndGoHomeEarly
                {
                    Student = student,
                    CreatedBy = userClaim.Item2,
                    AttendanceStatus = late.AttendanceStatus,
                    LateAndGoHomeEarlyStatus = late.LateAndGoHomeEarlyStatus,
                    CreateAt = DateTime.Now.ToUniversalTime(),
                    Description = late.Description,
                    Time = late.AtTime
                };

                picket.LateAndComeHomeEarly.Add(toLate);
                dbContext.SaveChanges();
                var result = await GenerateLateAndGoHomeEarlyResponse(toLate);
                return result;
            }
            catch (Exception ex)
            {
                return Error.Conflict();
            }
        }
        public async Task<ErrorOr<bool>> RemoveStudentToLateComeHomeSoEarly(int id)
        {
            try
            {
                var userClaim = await http.IsTeacherPicket(userManager, dbContext);
                if (!userClaim.Item1)
                    return Error.Unauthorized("Picket", "Maaf, Anda tidak sedang piket/anda tidak memiliki akses !");

                var picket = dbContext.Picket
                      .Include(x => x.LateAndComeHomeEarly).Where(x => x.LateAndComeHomeEarly.Any(x => x.Id == id)).FirstOrDefault();

                var result = picket.LateAndComeHomeEarly.Where(x => x.Id == id).FirstOrDefault();
                if (result == null)
                    return Error.NotFound("Picket", "Data tidak ditemukan.");
                picket.LateAndComeHomeEarly.Remove(result);
                dbContext.SaveChanges();
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return Error.Conflict();
            }
        }

        private async Task<LateAndGoHomeEarlyResponse?> GenerateLateAndGoHomeEarlyResponse(LateAndGoHomeEarly x)
        {
            var studentClass = await studentService.GetStudentWithClass(x.Id);
            var sx = studentClass.Value;
            return new LateAndGoHomeEarlyResponse
            {
                Id = x.Id,
                LateAndGoHomeEarlyStatus = x.LateAndGoHomeEarlyStatus,
                AttendanceStatus = x.AttendanceStatus,
                CreateAt = x.CreateAt,
                Time = x.Time,
                TeacherId = x.CreatedBy.Id,
                TeacherName = x.CreatedBy.Name,
                TeacherPhoto = x.CreatedBy.Photo,
                Description = x.Description,
                StudentPhoto = x.Student.Photo,
                StudentId = x.Student.Id,
                StudentName = x.Student.Name,
                ClassRoomId = sx == null ? null : sx.ClassRoomId,
                ClassRoomName = sx == null ? null : sx.ClassRoomName,
                DepartmentId = sx == null ? null : sx.DepartmenId,
                DepartmentName = sx == null ? null : sx.DepartmenName
            };
        }

        private async Task<ErrorOr<PicketResponse>> GeneratePicketResponse(Picket response)
        {
            var result = new PicketResponse()
            {
                CreateAt = response.CreateAt,
                CreatedId = response.CreatedBy.Id,
                CreatedName = response.CreatedBy.Name,
                CreatedNumber = response.CreatedBy.RegisterNumber,
                Date = response.Date,
                EndAt = response.EndAt,
                Id = response.Id,
                StartAt = response.StartAt,
                Weather = response.Weather,
                StudentsLateAndComeHomeEarly = Enumerable.Empty<LateAndGoHomeEarlyResponse>().ToList()
            };

            var students = await studentService.GetAlStudentWithClass();

            if (!students.IsError && response.StudentAttendances.Any())
            {
                result.StudentAttendance = (from x in response.StudentAttendances
                                            join s in students.Value on x.Student.Id equals s.Id into sGroup
                                            from sx in sGroup.DefaultIfEmpty()
                                            select new StudentAttendanceResponse(
                                                x.Id, x.PicketId,
                                                x.Student.Id,
                                                x.Student.Name,
                                                sx == null ? "" : sx.ClassRoomName,
                                                sx == null ? "" : sx.DepartmenName,
                                                x.AttendanceStatus, x.TimeIn, x.TimeOut, x.Description, x.CreateAt
                                            )).ToList();

            }


            result.StudentsLateAndComeHomeEarly = (from x in response.LateAndComeHomeEarly
                                                   join s in students.Value on x.Student.Id equals s.Id into sGroup
                                                   from sx in sGroup.DefaultIfEmpty()
                                                   select
                                                   new LateAndGoHomeEarlyResponse
                                                   {
                                                       Id = x.Id,
                                                       LateAndGoHomeEarlyStatus = x.LateAndGoHomeEarlyStatus,
                                                       AttendanceStatus = x.AttendanceStatus,
                                                       Time = x.Time,
                                                       CreateAt = x.CreateAt,
                                                       TeacherId = x.CreatedBy.Id,
                                                       TeacherName = x.CreatedBy.Name,
                                                       TeacherPhoto = x.CreatedBy.Photo,
                                                       Description = x.Description,
                                                       StudentPhoto = x.Student.Photo,
                                                       StudentId = x.Student.Id,
                                                       StudentName = x.Student.Name,
                                                       ClassRoomId = sx == null ? null : sx.ClassRoomId,
                                                       ClassRoomName = sx == null ? null : sx.ClassRoomName,
                                                       DepartmentId = sx == null ? null : sx.DepartmenId,
                                                       DepartmentName = sx == null ? null : sx.DepartmenName

                                                   }).ToList();




            result.DailyJournal = (from x in response.DailyJournals
                                   select new DailyJournalResponse(x.Id, x.Title, x.Content, x.Teacher.Id, x.Teacher.Name, x.CreateAt)).ToList();



            return result;

        }

        public async Task<ErrorOr<bool>> UpdatePicket(int id, PicketRequest model)
        {
            try
            {
                var result = dbContext.Picket.Include(x => x.CreatedBy)
                    .SingleOrDefault(x => x.Id == id);

                if (result == null)
                    return Error.NotFound("Picket", "Data piket tidak ditemukan.");

                result.Weather = model.Weather;
                result.StartAt = model.StartAt;
                result.EndAt = model.EndAt;
                if (result.CreatedBy.Id != model.CreatedId)
                {
                    result.CreatedBy = new Teacher { Id = model.Id };
                    dbContext.Entry(result.CreatedBy).State = EntityState.Unchanged;
                }
                dbContext.SaveChanges();
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return Error.Conflict("Picket", ex.Message);
            }
        }

        public async Task<ErrorOr<PicketResponse>> GetById(int id)
        {
            try
            {
                var ppicketToday = dbContext.Picket
                    .Include(x => x.CreatedBy)
                    .Include(x => x.DailyJournals).ThenInclude(x => x.Teacher)
                    .Include(x => x.LateAndComeHomeEarly).ThenInclude(x => x.Student)
                    .Include(x => x.StudentAttendances).ThenInclude(x => x.Student)
                    .FirstOrDefault(x => x.Id == id);

                if (ppicketToday == null)
                    return Error.NotFound("Picket", "Data tidak ditemukan.");

                var response = await GeneratePicketResponse(ppicketToday);
                return picketToday = response.Value;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<ErrorOr<PaginationResponse<PicketResponse>>> GetWithPaginate(PaginationRequest req)
        {
            try
            {
                var validator = new PaginateRequestValidator();
                var validateResult = validator.Validate(req);
                if (!validateResult.IsValid)
                    return validateResult.GetErrors();

                IQueryable<Picket> iq = dbContext.Picket.Include(x => x.CreatedBy);

                iq = iq.GetPicketOrder(req.ColumnOrder, req.SortOrder);

                var totalData = await iq.CountAsync();

                var data = await iq.Skip((req.Page - 1) * req.PageSize)
                    .Take(req.PageSize)
                    .Select(x => new PicketResponse()
                    {
                        CreateAt = x.CreateAt,
                        CreatedId = x.CreatedBy.Id,
                        CreatedName = x.CreatedBy.Name,
                        CreatedNumber = x.CreatedBy.RegisterNumber,
                        Date = x.Date,
                        EndAt = x.EndAt,
                        Id = x.Id,
                        StartAt = x.StartAt,
                        Weather = x.Weather
                    }).ToListAsync();

                return new PaginationResponse<PicketResponse>(data, req.Page, req.PageSize, totalData);

            }
            catch (Exception ex)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<bool>> RemoveDailyJournal(int id)
        {
            try
            {
                var userClaim = await http.IsTeacherPicket(userManager, dbContext);
                if (!userClaim.Item1)
                    return Error.Unauthorized("Picket", "Maaf, Anda tidak sedang piket/anda tidak memiliki akses !");

                var picket = dbContext.Picket
                      .Include(x => x.DailyJournals).Where(x => x.DailyJournals.Any(x => x.Id == id)).FirstOrDefault();

                var result = picket.DailyJournals.Where(x => x.Id == id).FirstOrDefault();
                if (result == null)
                    return Error.NotFound("Picket", "Data tidak ditemukan.");

                if (result.Teacher.Id != userClaim.Item2.Id)
                    return Error.Unauthorized("Picket", $"Maaf,Anda tidak dapat menghapus data ini. dibuat oleh  {result.Teacher.Name}");

                picket.DailyJournals.Remove(result);
                dbContext.SaveChanges();
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return Error.Conflict();
            }
        }

        public async Task<ErrorOr<DailyJournalResponse>> AddDailyJournal(DailyJournalRequest model)
        {
            try
            {
                var userClaim = await http.IsTeacherPicket(userManager, dbContext);
                if (!userClaim.Item1)
                    return Error.Unauthorized("Picket", "Maaf, Anda tidak sedang piket/anda tidak memiliki akses !");

                DateOnly date = DateOnly.FromDateTime(DateTime.Now);
                var picket = dbContext.Picket
                        .Include(x => x.DailyJournals)
                        .ThenInclude(x => x.Teacher)
                        .FirstOrDefault(x => x.Date == date);
                if (picket == null || DateOnly.FromDateTime(DateTime.Now) != picket.Date)
                {
                    return Error.Failure("Picket", "Piket Belum Di buka");
                }
                var dailyJournal = new DailyJournal
                {
                    Teacher = userClaim.Item2,
                    CreateAt = model.CreateAt.ToUniversalTime(),
                    Content = model.Content,
                    Title = model.Title
                };
                picket.DailyJournals.Add(dailyJournal);
                dbContext.Entry(dailyJournal.Teacher).State = EntityState.Unchanged;
                dbContext.SaveChanges();
                var result = await GenerateDaeilyJournalResponse(dailyJournal);
                return result;
            }
            catch (Exception)
            {
                return Error.Conflict();
            }
        }

        private Task<DailyJournalResponse> GenerateDaeilyJournalResponse(DailyJournal dailyJournal)
        {
            var result = new DailyJournalResponse
                (dailyJournal.Id,
                  dailyJournal.Title,
                  dailyJournal.Content,
                  dailyJournal.Teacher == null ? 0 : dailyJournal.Teacher.Id,
                  dailyJournal.Teacher == null ? string.Empty : dailyJournal.Teacher.Name,
                  dailyJournal.CreateAt
                );
            return Task.FromResult(result);
        }

        public async Task<ErrorOr<DailyJournalResponse>> EditDailyJournal(int id, DailyJournalRequest model)
        {
            var journal = dbContext.Picket.Include(x => x.DailyJournals).ThenInclude(x => x.Teacher).SelectMany(x => x.DailyJournals).FirstOrDefault(x => x.Id == id);
            if (journal != null)
            {
                journal.Title = model.Title;
                journal.Content = model.Content;
                journal.CreateAt = model.CreateAt.ToUniversalTime();
                dbContext.SaveChanges();
                return new DailyJournalResponse(journal.Id,journal.Title, journal.Content, journal.Teacher.Id, journal.Teacher.Name, journal.CreateAt);
            }
            return Error.NotFound("Data tidak ditemukan !");
        }
    }
}
