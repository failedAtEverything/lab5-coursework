using CPApplication.Core.Models;
using CPApplication.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace CPApplication.Controllers
{
    public class TvProgramsController : Controller
    {
        private readonly TvChannelContext _context;
        private int pageSize = 10;

        public TvProgramsController(TvChannelContext context)
        {
            _context = context;
        }

        // GET: TvProgramsController
        public async Task<ActionResult> Index(int? page, string? sort)
        {
            var programs = await _context.TvPrograms.ToListAsync();
            var genres = await _context.Genres.ToListAsync();

            var views = await (from pgs in programs
                               from gns in genres
                               where gns.Id == pgs.GenreId
                               select new 
                               { 
                                   Id = pgs.Id,
                                   Name = pgs.Name,
                                   Length = pgs.Length.Split(":")[0] + "h-" + pgs.Length.Split(":")[1] + "m",
                                   Rating = pgs.Rating,
                                   GenreName = gns.Name,
                                   Description = pgs.Description,
                               }
                               ).ToListAsync();

            var currentPage = page ?? 1;


            return View(await views.ToPagedListAsync(currentPage, pageSize));
        }

        // GET: TvProgramsController/Details/5
        public ActionResult Details(int id)
        {
            var programs = _context.TvPrograms.ToList();
            var genres = _context.Genres.ToList();

            var view = (from pgs in programs
                              from gns in genres
                              where gns.Id == pgs.GenreId && pgs.Id == id
                              select new
                              {
                                  Id = pgs.Id,
                                  Name = pgs.Name,
                                  Length = pgs.Length.Split(":")[0] + "h-" + pgs.Length.Split(":")[1] + "m",
                                  Rating = pgs.Rating,
                                  GenreName = gns.Name,
                                  Description = pgs.Description,
                              }).FirstOrDefault();

            return View(view);
        }


        // GET: TvProgramsController/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Genres = _context.Genres.Select(x => x.Name).ToList();

            return View();
        }

        // POST: TvProgramsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            try
            {
                var genreId = _context.Genres.FirstOrDefault(x => x.Name == collection["Genre"].ToString()).Id;
                var length = collection["Length"].ToString();

                if (!length.Contains('h') | !length.Contains('m'))
                {
                    throw new Exception("Length should be specified at format \"<hours>h-<minutes>m\"");
                }
                length = length.Split("-")[0].TrimEnd('h') + ":" + length.Split("-")[1].TrimEnd('m');

                var id = _context.TvPrograms.Select(x => x.Id).Max() + 1;

                var program = new TvProgram()
                {
                    Id = id,
                    Name = collection["Name"].ToString(),
                    Length = length,
                    Rating = double.Parse(collection["Rating"].ToString()),
                    GenreId = genreId,
                    Description = collection["Description"].ToString(),
                };

                await _context.TvPrograms.AddAsync(program);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TvProgramsController/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            ViewBag.Genres = _context.Genres.Select(x => x.Name).ToList();
            var programs = _context.TvPrograms.ToList();
            var genres = _context.Genres.ToList();
            var view = (from pgs in programs
                        from gns in genres
                        where gns.Id == pgs.GenreId && pgs.Id == id
                        select new
                        {
                            Id = pgs.Id,
                            Name = pgs.Name,
                            Length = pgs.Length.Split(":")[0] + "h-" + pgs.Length.Split(":")[1] + "m",
                            Rating = pgs.Rating,
                            GenreName = gns.Name,
                            Description = pgs.Description,
                        }).FirstOrDefault();

            return View(view);
        }

        // POST: TvProgramsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                var genreId = _context.Genres.FirstOrDefault(x => x.Name == collection["Genre"].ToString()).Id;
                var length = collection["Length"].ToString();

                _context.ChangeTracker.Clear();

                if (!length.Contains('h') | !length.Contains('m'))
                {
                    throw new Exception("Length should be specified at format \"<hours>h-<minutes>m\"");
                }
                length = length.Split("-")[0].TrimEnd('h') + ":" + length.Split("-")[1].TrimEnd('m');
                var program = new TvProgram()
                    {
                        Id = id,
                        Name = collection["Name"].ToString(),
                        Length = length,
                        Rating = double.Parse(collection["Rating"].ToString()),
                        GenreId = genreId,
                        Description = collection["Description"].ToString(),
                    };

                _context.TvPrograms.Update(program);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View("Exception", ex);
            }
        }

        // POST: TvProgramsController/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var item = _context.TvPrograms.FirstOrDefault(x => x.Id == id);
                _context.Remove(item!);
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
