using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace EventManagerPro.DBLayer.DomainModels
{
    public class SubEventModel
    {

        public static SubEvent create(int eventId, int venueId, DateTime start, DateTime end, string name="")
        {
            using (var context = new EventContainer())
            {
                SubEvent newSubEvent = new SubEvent
                {
                    Name = name,
                    EventId = eventId,
                    Start = start,
                    End = end,
                    VenueId = venueId,
                };
                context.SubEvents.Add(newSubEvent);
                context.SaveChanges();
                return newSubEvent;
            }
        }

        public static SubEvent update(int id, int eventId, int venueId, DateTime start, DateTime end, string name="")
        {
            using (var context = new EventContainer())
            {
                SubEvent subEvent = new SubEvent
                {
                    Id = id,
                    Name = name,
                    EventId = eventId,
                    Start = start,
                    End = end,
                    VenueId = venueId,
                };
                context.SubEvents.Attach(subEvent);
                context.Entry(subEvent).State = EntityState.Modified;
                context.SaveChanges();
                return subEvent;
            }
        }

        public static SubEvent createObj(SubEvent e)
        {
            using (var context = new EventContainer())
            {
                context.SubEvents.Add(e);
                context.SaveChanges();
                return e;
            }
        }

        public static SubEvent updateObj(SubEvent e)
        {
            e.Venue = DBLayer.DomainModels.VenueModel.getByID(e.VenueId);

            using (var context = new EventContainer())
            {
                context.Entry(e).State = EntityState.Modified;
                context.SaveChanges();

                return e;
            }
        }

        public static void deleteById(int id)
        {
            using (var context = new EventContainer())
            {
                SubEvent e = getByID(id);
                context.SubEvents.Attach(e);
                context.SubEvents.Remove(e);
                context.SaveChanges();
            }
        }

        public static SubEvent getByID(int id)
        {
            using (var context = new EventContainer())
            {
                var subevents = from s in context.SubEvents.Include("Venue")
                                where s.Id == id
                                select s;
                return subevents.FirstOrDefault();
            }
        }

        public static List<SubEvent> getAllByEventID(int id)
        {
            using (var context = new EventContainer())
            {
                IEnumerable<SubEvent> subevents = from s in context.SubEvents.Include("Venue").Include("Event")
                                where s.EventId == id
                                orderby s.Start ascending
                                select s;
                return subevents.ToList();
            }
        }

        public static List<SubEvent> getAllByVenueIDAndTime(int id, DateTime start, DateTime end, int eventId)
        {
            using (var context = new EventContainer())
            {
                IEnumerable<SubEvent> subevents = from s in context.SubEvents.Include("Venue")
                                                  where s.VenueId == id && s.Start <= end && s.End >= start && s.EventId != eventId
                                                  orderby s.Start ascending
                                                  select s;
                return subevents.ToList();
            }
        }

        public static List<SubEvent> getAll()
        {
            using (var context = new EventContainer())
            {
                IEnumerable<SubEvent> subevents = from s in context.SubEvents.Include("Venue")
                                                  orderby s.Start descending
                                                  select s;
                return subevents.ToList();
            }
        }



    }
}
