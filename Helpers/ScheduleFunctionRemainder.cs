using System.Threading;
using System;
using log4net;
using WebApi.Entities;
using Microsoft.EntityFrameworkCore;
using WebApi.Services;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Google;
using System.Configuration;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Security.Principal;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore.Storage;

namespace WebApi.Helpers
{
    public class ScheduleFunctionRemainder// : IScheduleFunctionRemainderService
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static TimeSpan THREE_DAYS_TIMEOUT = new TimeSpan(3, 0, 0, 0);   // Three days time span
        public static TimeSpan WEEK_TIMEOUT = new TimeSpan(7, 0, 0, 0);         // Week time span
        public static TimeSpan HOUR_TIMEOUT = new TimeSpan(0, 1, 0, 0);         // Week time span

        private static bool terminate = false;
        private static DataContext _context;
        private static IEmailService _emailService;
        private readonly IServiceScope scope;
        static readonly object lockObject = new object();

        public ScheduleFunctionRemainder(IServiceProvider provider)
        {
            scope = provider.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<DataContext>();
            _emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        }

        // This method is called by the timer delegate.
        public static void CheckStatus(/*Object stateInfo*/)
        {
            DateTime prevDate = DateTime.Now;

            Console.WriteLine("Checking status {0}.", DateTime.Now.ToString("h:mm:ss.fff"));


            while (!terminate) // Check if the caller requested cancellation. 
            {
                DateTime now = DateTime.Now;
                // Calculate the interval between the two dates.  
                TimeSpan ts = now - prevDate;

                if (ts.TotalMilliseconds > HOUR_TIMEOUT.TotalMilliseconds) // Send e-mail every hour
                {
                    prevDate = DateTime.Now;
                    SendRemindingEmail4Functions();
                }
                Thread.Sleep(500);
            }

            if (terminate)
            {
                // Reset the counter and signal the waiting thread.
                _context.Dispose();
                log.InfoFormat("Timer reminding the function to attend has exited gracefully. {0} ", "");
            }
        }
        public void Terminate()
        {
            terminate = true;
        }
        public static void SendRemindingEmail4Functions()
        {
            log.Info("\n");
            var accountAll = _context.Accounts.Include(x => x.UserFunctions).Include(x => x.Schedules).ToList();

            IEnumerable<Account> query = accountAll.TakeWhile((a) => a.UserFunctions != null);
            foreach (var a in accountAll)
            {

                foreach (var s in a.Schedules)
                {
                    using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            if ((DateTime.Now - s.Date) > WEEK_TIMEOUT && a.NotifiedWeekBefore == false)
                            {
                                string message = $@"<i>{a.FirstName} {a.LastName}</i> is scheduled to attend their duties.";
                                string subject = $@"Reminder: {a.FirstName} {a.LastName} is {s.UserFunction} on s.Date.ToString(""yyyy-MM-dd HH:mm"")";
                                _emailService.Send(
                                    to: a.Email,
                                    subject: subject,
                                    html: message
                                );
                                a.NotifiedWeekBefore = true;
                                _context.Accounts.Update(a);
                                _context.SaveChanges();

                                transaction.Commit();
                                log.InfoFormat("Schedule ready for week ahead of reminder for an account is: {0} {1} {2}", a.FirstName, a.LastName, a.Email);
                            }
                            else if ((DateTime.Now - s.Date) > THREE_DAYS_TIMEOUT && a.NotifiedThreeDaysBefore == false)
                            {
                                string message = $@"<i>{a.FirstName} {a.LastName}</i> is scheduled to attend their duties.";
                                string subject = $@"Reminder: {a.FirstName} {a.LastName} is {s.UserFunction} on s.Date.ToString(""yyyy-MM-dd HH:mm"")";
                                _emailService.Send(
                                    to: a.Email,
                                    subject: subject,
                                    html: message
                                );
                                a.NotifiedThreeDaysBefore = true;
                                _context.Accounts.Update(a);
                                _context.SaveChanges();

                                transaction.Commit();
                                log.InfoFormat("Schedule ready for 3 days ahead of reminder for an account is: {0} {1} {2}", a.FirstName, a.LastName, a.Email);
                            } else
                            {
                                log.InfoFormat("Schedule not ready for sending reminder for an account is: {0} {1} {2}", a.FirstName, a.LastName, a.Email);
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine(Thread.CurrentThread.Name + "Error occurred.");
                            throw ex;
                        }
                        finally
                        {
                            Monitor.Exit(lockObject);
                            Console.WriteLine(Thread.CurrentThread.Name + " Exit from critical section");
                            log.Info("MoveSchedule2Pool after locking");
                        }
                    }
                }
            }
        }
    }
}
