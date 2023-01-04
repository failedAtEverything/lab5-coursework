using CPApplication.Core.Models;
using CPApplication.Data;
using CPApplication.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using X.PagedList;

namespace CPApplication.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AppealsController : Controller
    {
        private TvChannelContext _context;
        private int pageSize = 10;

        public AppealsController(TvChannelContext context)
        {
            _context = context;
        }

        // GET: AppealsController
        public async Task<ActionResult> Index(int? page)
        {
            var appeals = _context.Appeals;
            var broadcasts = _context.Broadcasts;
            var programs = _context.TvPrograms;

            var views = (from appeal in appeals
                         from broadcast in broadcasts
                         from tvProgram in programs
                         where appeal.BroadcastId == broadcast.Id && tvProgram.Id == broadcast.ProgramId
                         select new
                         {
                             Id = appeal.Id,
                             FullName = appeal.FullName,
                             Organization = appeal.Organization,
                             Purpose = appeal.AppealPurpose,
                             Program = tvProgram.Name + " " + broadcast.WeekNumber + "-" + broadcast.WeekMonth + "-" + broadcast.WeekYear,
                         }).ToList();
            
            var currentPage = page ?? 1;

            return View(await views.ToPagedListAsync(currentPage, pageSize));
        }

        // GET: AppealsController/Details/5
        public ActionResult Details(int id)
        {
            var appeals = _context.Appeals;
            var broadcasts = _context.Broadcasts;
            var programs = _context.TvPrograms;

            var view = (from appeal in appeals
                        from broadcast in broadcasts
                        from tvProgram in programs
                        where appeal.BroadcastId == broadcast.Id && tvProgram.Id == broadcast.ProgramId
                        select new
                        {
                            Id = appeal.Id,
                            FullName = appeal.FullName,
                            Organization = appeal.Organization,
                            Purpose = appeal.AppealPurpose,
                            Program = tvProgram.Name + " " + broadcast.WeekNumber + "-" + broadcast.WeekMonth + "-" + broadcast.WeekYear
                        }).FirstOrDefault(x => x.Id == id);

            return View(view);
        }

        // GET: AppealsController/Create
        public ActionResult Create()
        {
            var programs = _context.TvPrograms;

            ViewBag.Progrms = programs;

            return View();
        }

        // POST: AppealsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                if (!BroadcastData.Validate(collection["Program"].ToString()))
                {
                    throw new Exception("Program string should be at \"PROGRAMNAME WEEK-MONTH-YEAR\"");
                }

                var broadcastData = BroadcastData.FromString(collection["Program"].ToString());

                if (_context.TvPrograms.Where(x => x.Name == broadcastData.Program).IsNullOrEmpty())
                {
                    throw new Exception("There is no program record with specified name");
                }

                var broadcasts = _context.Broadcasts;
                var programs = _context.TvPrograms;

                if ((from broadcast in broadcasts
                     from program in programs
                     where broadcast.ProgramId == program.Id
                         & broadcast.WeekNumber == broadcastData.Week
                         & broadcast.WeekMonth == broadcastData.Month
                         & broadcast.WeekYear == broadcastData.Year
                         & program.Name == broadcastData.Program
                     select broadcast
                    ).IsNullOrEmpty())
                {
                    throw new Exception("There is no broadcast record at specified date");
                }

                var broadcastId = (from broadcast in broadcasts
                                   from program in programs
                                   where broadcast.ProgramId == program.Id
                                       & broadcast.WeekNumber == broadcastData.Week
                                       & broadcast.WeekMonth == broadcastData.Month
                                       & broadcast.WeekYear == broadcastData.Year
                                       & program.Name == broadcastData.Program
                                   select broadcast).FirstOrDefault()?.Id;

                var id = _context.Appeals.Select(x => x.Id).Max() + 1;

                var appeal = new Appeal()
                {
                    Id = id,
                    FullName = collection["FullName"].ToString(),
                    Organization = collection["Organization"].ToString(),
                    AppealPurpose = collection["Purpose"].ToString(),
                    BroadcastId = broadcastId.Value
                };

                _context.Appeals.Add(appeal);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View("Exception", ex);
            }
        }

        // GET: AppealsController/Edit/5
        public ActionResult Edit(int id)
        {
            var appeals = _context.Appeals;
            var broadcasts = _context.Broadcasts;
            var programs = _context.TvPrograms;

            ViewBag.Progrms = programs;

            var view = (from appeal in appeals
                        from broadcast in broadcasts
                        from tvProgram in programs
                        where appeal.BroadcastId == broadcast.Id && tvProgram.Id == broadcast.ProgramId
                        select new
                        {
                            Id = appeal.Id,
                            FullName = appeal.FullName,
                            Organization = appeal.Organization,
                            Purpose = appeal.AppealPurpose,
                            Program = new BroadcastData()
                            { 
                                Program = tvProgram.Name,
                                Week = broadcast.WeekNumber.Value,
                                Month = broadcast.WeekMonth.Value,
                                Year = broadcast.WeekYear.Value
                            }
                        }).FirstOrDefault(x => x.Id == id);

            return View(view);
        }

        // POST: AppealsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                _context.ChangeTracker.Clear();
                if (!BroadcastData.Validate(collection["Program"].ToString()))
                {
                    throw new Exception("Program string should be at \"PROGRAMNAME WEEK-MONTH-YEAR\"");
                }

                var broadcastData = BroadcastData.FromString(collection["Program"].ToString());

                if (_context.TvPrograms.Where(x => x.Name == broadcastData.Program).IsNullOrEmpty())
                {
                    throw new Exception("There is no program record with specified name");
                }

                var broadcasts = _context.Broadcasts;
                var programs = _context.TvPrograms;

                if ((from broadcast in broadcasts
                    from program in programs
                    where broadcast.ProgramId == program.Id 
                        & broadcast.WeekNumber == broadcastData.Week
                        & broadcast.WeekMonth == broadcastData.Month
                        & broadcast.WeekYear == broadcastData.Year
                        & program.Name == broadcastData.Program
                    select broadcast
                    ).IsNullOrEmpty())
                {
                    throw new Exception("There is no broadcast record at specified date");
                }

                var broadcastId = (from broadcast in broadcasts
                                  from program in programs
                                  where broadcast.ProgramId == program.Id
                                      & broadcast.WeekNumber == broadcastData.Week
                                      & broadcast.WeekMonth == broadcastData.Month
                                      & broadcast.WeekYear == broadcastData.Year
                                      & program.Name == broadcastData.Program
                                  select broadcast).FirstOrDefault()?.Id;

                var appeal = new Appeal()
                {
                    Id = id,
                    FullName = collection["FullName"].ToString(),
                    Organization = collection["Organization"].ToString(),
                    AppealPurpose = collection["Purpose"].ToString(),
                    BroadcastId = broadcastId.Value
                };

                _context.Appeals.Update(appeal);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                return View("Exception", ex);
            }
        }

        // POST: AppealsController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var item = _context.Appeals.FirstOrDefault(x => x.Id == id);
                _context.Remove(item);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
