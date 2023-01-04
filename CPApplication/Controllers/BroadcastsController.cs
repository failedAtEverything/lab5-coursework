using CPApplication.Core.Models;
using CPApplication.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using X.PagedList;

namespace CPApplication.Controllers
{
    [Authorize(Roles ="Admin")]
    public class BroadcastsController : Controller
    {
        private TvChannelContext _context;
        private int pageSize = 10;

        public BroadcastsController(TvChannelContext context)
        {
            _context = context;
        }

        // GET: BroadcastsController
        public async Task<ActionResult> Index(int? page,
            string? sort, 
            string? Genre,
            int? MinLength,
            int? MaxLength,
            int? WeekYear,
            int? WeekMonth,
            string? Employee,
            string? Guest
            )
        {
            try
            {
                var broadcasts = _context.Broadcasts.ToList();
                var programs = _context.TvPrograms.ToList();
                var employees = _context.Employees.ToList();
                var genres = _context.Genres.ToList();

                var currentPage = page ?? 1;

                var views = (from broadcast in broadcasts
                             from program in programs
                             from employee in employees
                             from genre in genres
                             where broadcast.ProgramId == program.Id
                                & broadcast.EmployeesId == employee.Id
                                & program.GenreId == genre.Id
                             select new
                             {
                                 Id = broadcast.Id,
                                 WeekNumber = broadcast.WeekNumber,
                                 WeekMonth = broadcast.WeekMonth,
                                 WeekYear = broadcast.WeekYear,
                                 StartTime = broadcast.StartTime,
                                 EndTime = broadcast.EndTime,
                                 Program = program.Name,
                                 Genre = genre.Name,
                                 Length = program.Length,
                                 Employee = employee.FullName,
                                 Guests = broadcast.Guests
                             }).ToList();

                if (Genre is not null & Genre != string.Empty)
                {
                    ViewBag.Genre = Genre;
                    views = views.Where(x => x.Genre.ToString().ToLower().Contains(Genre.ToLower())).ToList();
                }
                if (MinLength is not null)
                {
                    ViewBag.MinLength = MinLength;

                    views = views.Where(x =>
                    {
                        var viewLength = x.Length;
                        var length = int.Parse(viewLength.Split(":")[0]) * 60 + int.Parse(viewLength.Split(":")[1]);

                        return MinLength <= length;
                    }).ToList();
                }
                if (MaxLength is not null)
                {
                    ViewBag.MaxLength = MaxLength;  
                    views = views.Where(x =>
                    {
                        var viewLength = x.Length;
                        var length = int.Parse(viewLength.Split(":")[0]) * 60 + int.Parse(viewLength.Split(":")[1]);

                        return MaxLength >= length;
                    }).ToList();
                }
                if (WeekYear is not null)
                {
                    ViewBag.WeekYear = WeekYear;
                    views = views.Where(x => x.WeekYear == WeekYear).ToList();
                }
                if (WeekMonth is not null)
                {
                    ViewBag.WeekMonth = WeekMonth;
                    views = views.Where(x => x.WeekMonth == WeekMonth).ToList();
                }
                if (Employee is not null & Employee != string.Empty)
                {
                    ViewBag.Employee = Employee;
                    views = views.Where(x => x.Employee.ToLower().Contains(Employee.ToLower())).ToList();
                }
                if (Guest is not null & Guest != string.Empty)
                {
                    ViewBag.Guest = Guest;
                    views = views.Where(x => x.Guests.ToLower().Contains(Guest.ToLower())).ToList();
                }

                ViewBag.Sort = sort;

                switch (sort)
                {
                    case "Program":
                        views = views.OrderBy(x => x.Program).ToList();
                        break;
                    case "WeekNumber":
                        views = views.OrderBy(x => x.WeekNumber).ToList();
                        break;
                    case "WeekMonth":
                        views = views.OrderBy(x => x.WeekMonth).ToList();
                        break;
                    case "WeekYear":
                        views = views.OrderBy(x => x.WeekYear).ToList();
                        break;
                    default:
                        views = views.OrderBy(x => x.Id).ToList();
                        break;
                }

                return View(await views.ToPagedListAsync(currentPage, pageSize));
            }
            catch (Exception ex)
            {
                return View("Exception", ex);
            }
        }

        // GET: BroadcastsController/Details/5
        public ActionResult Details(int id)
        {
            var broadcasts = _context.Broadcasts.ToList();
            var programs = _context.TvPrograms.ToList();
            var employees = _context.Employees.ToList();

            var view = (from broadcast in broadcasts
                                    from program in programs
                                    from employee in employees
                                    where broadcast.ProgramId == program.Id
                                        & broadcast.EmployeesId == employee.Id
                                        & broadcast.Id == id
                                    select new
                                    {
                                        Id = broadcast.Id,
                                        WeekNumber = broadcast.WeekNumber,
                                        WeekMonth = broadcast.WeekMonth,
                                        WeekYear = broadcast.WeekYear,
                                        StartTime = broadcast.StartTime,
                                        EndTime = broadcast.EndTime,
                                        Program = program.Name,
                                        Employee = employee.FullName,
                                        Guests = broadcast.Guests
                                    }).FirstOrDefault();

            return View(view);
        }

        // GET: BroadcastsController/Create
        public ActionResult Create()
        {
            var programs = _context.TvPrograms.ToList();
            var employees = _context.Employees.ToList();

            ViewBag.Programs = _context.TvPrograms.Select(x => x.Name).ToList();
            ViewBag.Employees = _context.Employees.Select(x => x.FullName).ToList();

            return View();
        }

        // POST: BroadcastsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var id = _context.Broadcasts.Select(x => x.Id).Max() + 1;

                var year = collection["WeekYear"].ToString();
                var month = collection["WeekMonth"].ToString();
                var week = collection["WeekNumber"].ToString();

                var startTime = TimeSpan.Parse(collection["StartTime"].ToString());
                var endTime = TimeSpan.Parse(collection["EndTime"].ToString());

                var program = collection["Program"].ToString();
                var employee = collection["Employee"].ToString();

                var programId = _context.TvPrograms.FirstOrDefault(x => x.Name == program)?.Id;
                var employeeId = _context.Employees.FirstOrDefault(x => x.FullName == employee)?.Id;

                var guests = collection["Guests"].ToString();

                var item = new Broadcast()
                {
                    Id = id,
                    WeekNumber = int.Parse(week),
                    WeekMonth = int.Parse(month),
                    WeekYear = int.Parse(year),

                    StartTime = startTime,
                    EndTime = endTime,

                    ProgramId = programId.Value,
                    EmployeesId = employeeId.Value,

                    Guests = guests
                };

                _context.Add(item);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View("Exception", ex);
            }
        }

        // GET: BroadcastsController/Edit/5
        public ActionResult Edit(int id)
        {
            var broadcasts = _context.Broadcasts.ToList();
            var programs = _context.TvPrograms.ToList();
            var employees = _context.Employees.ToList();

            ViewBag.Programs = _context.TvPrograms.Select(x => x.Name).ToList();
            ViewBag.Employees = _context.Employees.Select(x => x.FullName).ToList();

            var view = (from broadcast in broadcasts
                        from program in programs
                        from employee in employees
                        where broadcast.ProgramId == program.Id
                            & broadcast.EmployeesId == employee.Id
                            & broadcast.Id == id
                        select new
                        {
                            Id = broadcast.Id,
                            WeekNumber = broadcast.WeekNumber,
                            WeekMonth = broadcast.WeekMonth,
                            WeekYear = broadcast.WeekYear,
                            StartTime = broadcast.StartTime,
                            EndTime = broadcast.EndTime,
                            Program = program.Name,
                            Employee = employee.FullName,
                            Guests = broadcast.Guests
                        }).FirstOrDefault();

            return View(view);
        }

        // POST: BroadcastsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                _context.ChangeTracker.Clear();
                var year = collection["WeekYear"];
                var month = collection["WeekMonth"];
                var week = collection["WeekNumber"];

                var startTime = TimeSpan.Parse(collection["StartTime"].ToString());
                var endTime = TimeSpan.Parse(collection["EndTime"].ToString());

                if (startTime >= endTime)
                {
                    throw new Exception("Start time can't be more or equals end time");
                }

                var program = collection["Program"].ToString();
                var employee = collection["Employee"].ToString();

                var programId = _context.TvPrograms.FirstOrDefault(x => x.Name == program)?.Id;
                var employeeId = _context.Employees.FirstOrDefault(x => x.FullName == employee)?.Id;

                var guests = collection["Guests"].ToString();

                var item = new Broadcast()
                {
                    Id = id,
                    WeekNumber = int.Parse(week),
                    WeekMonth = int.Parse(month),
                    WeekYear = int.Parse(year),

                    StartTime =  startTime,
                    EndTime = endTime,

                    ProgramId = programId.Value,
                    EmployeesId = employeeId.Value,

                    Guests = guests
                };

                _context.Update(item);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View("Exception", ex);
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var item = _context.Broadcasts.FirstOrDefault(x => x.Id == id);
                _context.Broadcasts.Remove(item);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View("Exception", ex);
            }
        }
    }
}
