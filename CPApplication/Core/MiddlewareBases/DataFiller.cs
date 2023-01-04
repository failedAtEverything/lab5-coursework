using CPApplication.Controllers;
using CPApplication.Core.Models;
using CPApplication.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;

namespace CPApplication.Core.MiddlewareBases
{

    public class DataFiller
    {
        protected static string[] Genres { get; set; } = new[] 
        { 
            "Basic information", 
            "Action & adventure",
            "Cartoons/Anime",
            "Comedy",
            "Documentary",
            "Cooking",
            "Music"
        };

        protected static string[] Descriptions { get; set; } = new[]
        {
            "Actual news",
            "Films",
            "Children oriented content",
            "Comedy shows",
            "Teaching programs about wild life",
            "Cooking tutorial",
            "Music events reportages"
        };

        protected static string[] TvPrograms { get; set; } = new[]
        {
            "Breaking news",
            "Atlantis",
            "Tom & Jerry",
            "Stand Up",
            "Jungle Life",
            "Cooking with Butcher",
            "Music World",
        };

        protected static string[] Guests { get; set; } = new[]
        {
            "Crews P. Y.",
            "Ape H. K.",
            "Reiwse K. L.",
            "Polart D. M.",
            "Torgal P. W."
        };

        protected static string[] Surnames { get; set; } = new[]
        {
            "Price", 
            "Brown", 
            "Hunter", 
            "Vang", 
            "Osborn", 
            "Bentley", 
            "Lamb", 
            "Shea",
            "Goodman", 
            "Galvan", 
            "Rangel", 
            "Zamora", 
            "Dean", 
            "Davidson", 
            "Roach", 
            "Valentine", 
            "Soto", 
            "Gaines"
        };

        protected static string[] Letters { get; set; } = new[]
        { 
            "A.", "B.", "C.", "D.", "E.", "F.", "G.", "H.", "I.",
            "J.", "K.", "L.", "M.", "N.", "O.", "P.", "Q.", "R.",
            "S.", "T.", "U.", "V.", "W.", "X.", "Y.", "Z."
        };

        protected static string[] Positions { get; set; } = new[]
        {
            "Manager",
            "Intern showman",
            "Junior showman",
            "Middle showman",
            "Master showman"
        };

        protected static string[] Organizations { get; set; } = new[]
        {
            "Factory",
            "Clinic",
            "Trade point",
            "Company"
        };

        protected static string[] Purposes { get; set; } = new[]
        {
            "Claim",
            "Nottion",
            "Recomendation",
            "Question"
        };

        public static async Task ExecuteAsync(TvChannelContext context)
        {
            var random = new Random();

            //genres generation
            if (context.Genres.IsNullOrEmpty()) 
            {
                for (int i = 1; i <= Genres.Length; i++)
                {
                    var name = Genres[i - 1];
                    var description = Descriptions[i - 1];

                    var item = new Genre()
                    {
                        Id = i,
                        Name = name,
                        Description = description
                    };

                    context.Genres.Add(item);
                }
            }

            await context.SaveChangesAsync();

            //employees generation
            if (context.Employees.IsNullOrEmpty())
            {
                for (int i = 1; i <= 200; i++)
                {
                    var surname = Surnames[random.Next(Surnames.Length)];
                    var first = Letters[random.Next(Letters.Length)];
                    var second = Letters[random.Next(Letters.Length)];

                    var fullname = $"{surname} {first} {second}";

                    var position = Positions[random.Next(Positions.Length)];

                    var employee = new Employee()
                    {
                        Id = i,
                        FullName = fullname,
                        Position = position
                    };

                    context.Employees.Add(employee);
                }
            }

            await context.SaveChangesAsync();

            //TvPrograms generation
            if (context.TvPrograms.IsNullOrEmpty())
            {
                for (int i = 1; i <= 200; i++)
                {
                    var fk = random.Next(TvPrograms.Length) + 1;

                    var name = TvPrograms[fk - 1];

                    var lengthHours = random.Next(2);
                    var lengthMinutes = random.Next(60);

                    var length = $"{lengthHours}:{lengthMinutes}";

                    var rating = Math.Pow(random.NextDouble(), 0.3);

                    var genreid = fk;

                    var description = Descriptions[fk - 1];

                    var tvProgram = new TvProgram()
                    {
                        Id = i,
                        Name = name,
                        Length = length,
                        Rating = rating,
                        GenreId = genreid,
                        Description = description
                    };

                    context.TvPrograms.Add(tvProgram);
                }
            }

            await context.SaveChangesAsync();

            //Broadcasts generation
            if (context.Broadcasts.IsNullOrEmpty())
            {
                var startDate = DateTime.Now.AddDays(-1000);

                for (int i = 1; i <= 1000; i++)
                {
                    var currentDate = startDate.AddDays(i);

                    var weekNumber = random.Next(1, 5);
                    var weekMonth = currentDate.Month;
                    var weekYear = currentDate.Year;

                    var startHour = random.Next(15, 21);
                    var startMinute = random.Next(60);

                    var programPos = random.Next(context.TvPrograms.Count());
                    var programId = context.TvPrograms.AsEnumerable().ElementAt(programPos).Id;

                    var length = context.TvPrograms.FirstOrDefault(x => x.Id == programId)?.Length;
                    var lHour = int.Parse(length?.Split(":")[0]);
                    var lMinute = int.Parse(length?.Split(':')[1]);
                    var startTime = new TimeSpan(startHour, startMinute, 0);
                    var endTime = new TimeSpan(startHour + lHour, startMinute + lMinute, 0);

                    var employeePos = random.Next(context.Employees.Count());
                    var employeeId = context.Employees.AsEnumerable().ElementAt(employeePos)?.Id;

                    var guest = Guests[random.Next(Guests.Length)];

                    var broadcast = new Broadcast()
                    {
                        Id = i,
                        WeekNumber = weekNumber,
                        WeekMonth = weekMonth,
                        WeekYear = weekYear,
                        StartTime = startTime,
                        EndTime = endTime,
                        ProgramId = programId,
                        EmployeesId = employeeId.Value,
                        Guests = guest
                    };

                    context.Broadcasts.Add(broadcast);
                }
            }

            await context.SaveChangesAsync();

            //Appeals generation
            if (context.Appeals.IsNullOrEmpty())
            {
                for (int i = 1; i <= 5000; i++)
                {
                    var surname = Surnames[random.Next(Surnames.Length)];
                    var first = Letters[random.Next(Letters.Length)];
                    var second = Letters[random.Next(Letters.Length)];

                    var fullname = $"{surname} {first} {second}";

                    var organization = Organizations[random.Next(Organizations.Length)];

                    var appealPurpose = Purposes[random.Next(Purposes.Length)];

                    var broadcastId = context.Broadcasts.AsEnumerable().ElementAt(random.Next(context.Broadcasts.Count())).Id;

                    var appeal = new Appeal()
                    {
                        Id = i,
                        FullName = fullname,
                        Organization = organization,
                        AppealPurpose = appealPurpose,
                        BroadcastId = broadcastId,
                    };

                    context.Appeals.Add(appeal);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
