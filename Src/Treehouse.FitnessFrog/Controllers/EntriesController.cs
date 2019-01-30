using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        public ActionResult Add()
        {
            var entry = new Entry()
            {
                Date = DateTime.Today
            };
            SetupActivitiesSelectListItems();

            return View(entry);
        }

        

        [HttpPost]
        public ActionResult Add(Entry entry)
        {
            ValidateEntry(entry);

            if (ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);

                return RedirectToAction("Index");
            }

            SetupActivitiesSelectListItems();

            return View(entry);
        }

        
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //TODO get request from repo
            Entry entry = _entriesRepository.GetEntry((int)id);
            //TODO return status of not found if entry not found
            if(entry == null)
            {
                return HttpNotFound();
            }
            //TODO populate the activityies select list items ViewBag property
            SetupActivitiesSelectListItems();
            //TODO pass the entry in the view
            return View(entry);
        }

        [HttpPost]
        public ActionResult Edit(Entry entry)
        {

            //TODO validate the entry
            ValidateEntry(entry);

            //TODO if the entry  is valid... 
                // 1.use the repo to update the entry
                // 2.redirect the user to the "entries" list page
                if(ModelState.IsValid)
            {
                _entriesRepository.UpdateEntry(entry);

                return RedirectToAction("Index");
            }
            //TODO populate the activities selevt list items ViewBag property
            SetupActivitiesSelectListItems();

            return View(entry);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //TODO reteive entry for the provided  id parameter value 
            Entry entry = _entriesRepository.GetEntry((int)id);

            //TODO return not found if an entry wasnt found
            if (entry == null)
            {
                return HttpNotFound();
            }

            //TODO pass the entry to the view
            return View(entry);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            //TODO delete the entry
            _entriesRepository.DeleteEntry(id);

            //TODO redirect to the enties list page

            return RedirectToAction("Index");
        }

        private void ValidateEntry(Entry entry)
        {
            //this is a global error message
            //ModelState.AddModelError("", "This is a test for global string");

            //no duration field validation errors, make sure that duration is greater than 0
            //server side validation
            if (ModelState.IsValidField("Duration") && entry.Duration <= 0)
            {
                ModelState.AddModelError("Duration", "Duration field must be greater than '0'");
            }
        }
        private void SetupActivitiesSelectListItems()
        {
            ViewBag.ActivitiesSelectListItems = new SelectList(Data.Data.Activities, "Id", "Name");
        }
    }
}

