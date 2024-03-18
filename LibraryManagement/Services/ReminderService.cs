using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services
{
    public class ReminderService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ReminderService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var today = DateTime.UtcNow.Date;
                    var reminderDate = today.AddDays(1);

                    var borrowingsDueTomorrow = await context.Borrowings
                        .Include(b => b.Book)
                        .Where(b => b.DueDate.Date == reminderDate)
                        .ToListAsync(stoppingToken);

                    foreach (var borrowing in borrowingsDueTomorrow)
                    {
                        Console.WriteLine($"Pripomienka: Zajtra by mala byt vratena kniha '{borrowing.Book.Title}'.");
                        // Send via smtp.... 
                    }
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); 
            }
        }
    }
}
