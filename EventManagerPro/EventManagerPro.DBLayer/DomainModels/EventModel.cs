using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace EventManagerPro.DBLayer.DomainModels
{
    public class EventModel
    {
        public static Event create(string matricId, string name, int venueId, DateTime start, DateTime end, int capacity, int budget = 0, string description = "", short viewAtLoginPage = 1)
        {
            using (var context = new EventContainer())
            {
                Event newEvent = new Event
                {
                    StudentMatricId = matricId,
                    Name = name,
                    Description = description,
                    Start = start,
                    End = end,
                    TimeCreated = DateTime.Now,
                    Capacity = capacity,
                    ViewAtLoginPage = viewAtLoginPage,
                };
                context.Events.Add(newEvent);
                context.SaveChanges();
                return newEvent;
            }
        }

        public static Event update(int id, string matricId, string name, int venueId, DateTime start, DateTime end, int capacity, int budget = 0, string description = "", short viewAtLoginPage = 1)
        {
            using (var context = new EventContainer())
            {
                Event updatedEvent = new Event
                {
                    Id = id,
                    StudentMatricId = matricId,
                    Name = name,
                    Description = description,
                    Start = start,
                    End = end,
                    TimeCreated = DateTime.Now,
                    Capacity = capacity,
                    ViewAtLoginPage = viewAtLoginPage,
                };
                context.Events.Attach(updatedEvent);
                context.Entry(updatedEvent).State = EntityState.Modified;
                context.SaveChanges();
                return updatedEvent;
            }
        }

        public static Event createObj(Event e)
        {
            using (var context = new EventContainer())
            {
                context.Events.Add(e);
                context.SaveChanges();
                return e;
            }
        }

        public static Event updateObj(Event e)
        {
            using (var context = new EventContainer())
            {
                context.Budgets.Attach(e.Budget);

                context.Entry(e.Budget).State = EntityState.Modified;
                context.Entry(e).State = EntityState.Modified;

                context.SaveChanges();

                return e;
            }
        }

        public static Boolean registerGuest(string matricId, int eventId)
        {
            using (var context = new EventContainer())
            {
                var runningEvent = (from e in context.Events.Include("Owner").Include("Guests")
                                    where e.Id == eventId
                                    select e).FirstOrDefault();
                var student = (from s in context.Students
                               where s.MatricId == matricId
                               select s).FirstOrDefault();
                if (runningEvent == null || student == null)
                {
                    return false;
                }
                else
                {
                    runningEvent.Guests.Add(student);
                    context.SaveChanges();
                    return true;
                }
            }
        }

        public static Boolean unregisterGuest(string matricId, int eventId)
        {
            using (var context = new EventContainer())
            {
                var runningEvent = (from e in context.Events.Include("Owner").Include("Guests")
                                    where e.Id == eventId
                                    select e).FirstOrDefault();
                var student = (from s in context.Students
                               where s.MatricId == matricId
                               select s).FirstOrDefault();
                if (runningEvent == null || student == null)
                {
                    return false;
                }
                else
                {
                    runningEvent.Guests.Remove(student);
                    context.SaveChanges();
                    return true;
                }
            }
        }

        public static void deleteById(int id)
        {
            using (var context = new EventContainer())
            {
                Event e = getByID(id);

                // Now we have SubEvents, delete them first to break relationships with Event object.
                List<SubEvent> subEvents = DomainModels.SubEventModel.getAllByEventID(id);

                foreach (SubEvent s in subEvents)
                    DomainModels.SubEventModel.deleteById(s.Id);

                // ...and BudgetItems too!
                List<BudgetItem> budgetItems = DomainModels.BudgetItemModel.getByBudgetId(e.Budget.Id);

                foreach (BudgetItem b in budgetItems)
                    DomainModels.BudgetItemModel.deleteById(b.Id);

                // Now we can delete both Event and Budget objects in peace.
                // Refresh the Event object by getting from the database again.
                e = getByID(id);

                context.Events.Attach(e);
                context.Budgets.Attach(e.Budget);

                context.Entry(e.Budget).State = EntityState.Deleted;
                context.Entry(e).State = EntityState.Deleted;

                context.SaveChanges();
            }
        }

        public static List<Event> getAll()
        {
            using (var context = new EventContainer())
            {
                IEnumerable<Event> events = from s in context.Events.Include("SubEvents").Include("Owner").Include("Guests").Include("Budget")
                                            orderby s.TimeCreated descending
                                            select s;
                return events.ToList();
            }
        }

        public static List<Event> getAllForLoginPage()
        {
            using (var context = new EventContainer())
            {
                IEnumerable<Event> events = from s in context.Events.Include("SubEvents").Include("Owner").Include("Guests").Include("Budget")
                                            where s.ViewAtLoginPage == 1
                                            orderby s.TimeCreated descending
                                            select s;
                return events.ToList();
            }
        }

        public static List<Event> getAllByMonth(int month)
        {
            using (var context = new EventContainer())
            {
                IEnumerable<Event> events = from s in context.Events.Include("SubEvents").Include("Owner").Include("Guests").Include("Budget")
                                            where s.ViewAtLoginPage == 1 && s.Start.Month == month
                                            orderby s.TimeCreated descending
                                            select s;
                return events.ToList();
            }
        }

        public static List<Event> getAllByYearMonth(DateTime date)
        {
            using (var context = new EventContainer())
            {
                IEnumerable<Event> events = from s in context.Events.Include("SubEvents").Include("Owner").Include("Guests").Include("Budget")
                                            where s.ViewAtLoginPage == 1 && s.Start.Year == date.Year && s.Start.Month == date.Month
                                            orderby s.TimeCreated descending
                                            select s;
                return events.ToList();
            }
        }

        public static List<Event> getNotByOwner(string matricId)
        {
            using (var context = new EventContainer())
            {
                IEnumerable<Event> events = from s in context.Events.Include("SubEvents").Include("Owner").Include("Guests").Include("Budget")
                                            where s.StudentMatricId != matricId
                                            orderby s.TimeCreated descending
                                            select s;
                return events.ToList();
            }
        }

        public static List<Event> getByOwner(string matricId)
        {
            using (var context = new EventContainer())
            {
                IEnumerable<Event> events = from s in context.Events.Include("SubEvents").Include("Owner").Include("Guests").Include("Budget")
                                            where s.StudentMatricId == matricId
                                            orderby s.TimeCreated descending
                                            select s;
                return events.ToList();
            }
        }

        public static Event getByID(int id)
        {
            using (var context = new EventContainer())
            {
                var events = from s in context.Events.Include("SubEvents").Include("Owner").Include("Guests").Include("Budget").Include("Budget.BudgetItems")
                             where s.Id == id
                             select s;
                return events.FirstOrDefault();
            }
        }
    }
}
