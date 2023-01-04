using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CPApplication.Core.Models;
using CPApplication.Infrastructure;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;

namespace CPApplication.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private readonly TvChannelContext _context;
        private string[] months = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        private static int pageSize = 10;

        public EmployeesController(TvChannelContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index(int? page, string? sort, IFormCollection filters)
        {
            var views = _context.Employees.ToList();
            var currentPage = page ?? 1;

            switch (sort)
            {
                case "Name":
                    views = views.OrderBy(x => x.FullName).ToList();
                    break;
                case "Position":
                    views = views.OrderBy(x => x.Position).ToList();
                    break;
                default:
                    views = views.OrderBy(x => x.Id).ToList();
                    break;
            }

            if (filters["Name"].ToString() != null)
            {
                views = views.Where(x => x.FullName.ToLower().Contains(filters["Name"].ToString().ToLower())).ToList();
            }

            if (filters["Pos"].ToString() != null)
            {
                views = views.Where(x => x.Position.ToLower().Contains(filters["Pos"].ToString().ToLower())).ToList();
            }

            ViewBag.Sort = sort;
            ViewBag.Name = filters["Name"].ToString();
            ViewBag.Pos = filters["Pos"].ToString();

            return View(await views.ToPagedListAsync(currentPage, pageSize));
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        public IActionResult Telecast(int id)
        {
            var employees = _context.Employees.ToList();
            var broadcasts = _context.Broadcasts.ToList();
            var programs = _context.TvPrograms.ToList();

            var employee = employees.FirstOrDefault(x => x.Id == id);

            var statistics = new Dictionary<int, Dictionary<string, Pair<double, List<string>>>>();

            var years = broadcasts.Where(x => x.EmployeesId == employee.Id).Select(y => y.WeekYear).Distinct().ToList();

            foreach (var year in years)
            {
                var yearMonths = broadcasts.Where(x => x.WeekYear == year & x.EmployeesId == employee.Id).Select(y => y.WeekMonth).ToList();
                var yearBroadcasts = broadcasts.Where(x => x.WeekYear == year & x.EmployeesId == employee.Id);
                var yearStatistics = new Dictionary<string, Pair<double, List<string>>>();

                foreach (var month in yearMonths)
                {
                    var monthStatistics = new Pair<double, List<string>>();

                    var monthBroadcasts = yearBroadcasts.Where(x => x.WeekMonth == month);
                    var employeeYearAndMonthsBroadcastsData = (from mb in monthBroadcasts
                                                               from program in programs
                                                               where program.Id == mb.ProgramId
                                                               select new
                                                               {
                                                                   Id = mb.Id,
                                                                   Length = program.Length,
                                                                   Program = program.Name
                                                               }).ToList();

                    var currentPrograms = employeeYearAndMonthsBroadcastsData.Select(x => x.Program).Distinct().ToList();
                    var currentWork = 0.0;

                    employeeYearAndMonthsBroadcastsData.ForEach(x =>
                    {
                        currentWork += double.Parse(x.Length.Split(":")[0]) * 60;
                        currentWork += double.Parse(x.Length.Split(":")[1]);
                    });

                    monthStatistics.First = currentWork / 60;
                    monthStatistics.Second = currentPrograms;

                    var currentMonth = months[month.Value - 1];

                    yearStatistics.Add(currentMonth, monthStatistics);
                }

                statistics.Add(year.Value, yearStatistics);
            }

            var employeeStatistics = new Pair<Employee, Dictionary<int, Dictionary<string, Pair<double, List<string>>>>>(employee, statistics);

            return View(employeeStatistics);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Position")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Position")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.ChangeTracker.Clear();
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'TvChannelContext.Employees'  is null.");
            }
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
          return _context.Employees.Any(e => e.Id == id);
        }
    }

    public class Pair<T, Y>
    {
        public T First { get; set; }
        public Y Second { get; set; }

        public Pair() { }

        public Pair(T first, Y second)
        {
            this.First = first;
            this.Second = second;
        }
    }
}
