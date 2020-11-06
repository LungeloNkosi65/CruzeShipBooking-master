using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CruzeShipBooking.Models;
using Microsoft.AspNet.Identity;

namespace CruzeShipBooking.Controllers
{
    public class BookingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Bookings
        public ActionResult Index()
        {
            var userName = User.Identity.GetUserName();
            var bookings = db.Bookings.Include(b => b.BookingPackage);
            if (!User.IsInRole("Admin"))
            {
                return View(bookings.ToList().Where(x => x.CustomerEmail == userName));
            }
            else
            {
                return View(bookings.ToList());
            }
        }

        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // GET: Bookings/Create
        public ActionResult Create()
        {
            ViewBag.BookingPackageId = new SelectList(db.BookingPackages, "BookingPackageId", "BookingPackageDescription");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookingId,CustomerEmail,BookingPackageId,DateBookingFor,CheckInTime,BookingPrice,BookingStatus,DateBooked,ReferenceNumber")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                var userName = User.Identity.GetUserName();
                if (booking.CheckBookingDate() == false)
                {
                    var userID = User.Identity.GetUserId();
                    booking.DateBooked = DateTime.Now;
                    booking.BookingPrice = booking.GetBookingFee();
                    booking.BookingStatus = "Awaiting Approval";
                    booking.CustomerEmail = userName;
                    booking.ReferenceNumber = booking.GenerateReferenceNumber(userID);
                    db.Bookings.Add(booking);
                    db.SaveChanges();
                    EmailSender.SendBookingEmail(booking);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "You can not book for a date that has already passed");
                    ViewBag.BookingPackageId = new SelectList(db.BookingPackages, "BookingPackageId", "BookingPackageDescription", booking.BookingPackageId);
                    return View(booking);

                }
             
            }

            ViewBag.BookingPackageId = new SelectList(db.BookingPackages, "BookingPackageId", "BookingPackageDescription", booking.BookingPackageId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            ViewBag.BookingPackageId = new SelectList(db.BookingPackages, "BookingPackageId", "BookingPackageDescription", booking.BookingPackageId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookingId,CustomerEmail,BookingPackageId,DateBookingFor,CheckInTime,BookingPrice,BookingStatus")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BookingPackageId = new SelectList(db.BookingPackages, "BookingPackageId", "BookingPackageDescription", booking.BookingPackageId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        public ActionResult ConfirmBooking(int? id)
        {
            var dbRecord = db.Bookings.Find(id);
            dbRecord.BookingStatus = "Booking Confirmed";
            db.Entry(dbRecord).State = EntityState.Modified;
            db.SaveChanges();
            EmailSender.SendBookingEmail(dbRecord);
            return RedirectToAction("Index");
        }
        public ActionResult CheckInBooking(int? id)
        {
            var dbRecord = db.Bookings.Find(id);
            dbRecord.BookingStatus = "Booking Checked In";
            db.Entry(dbRecord).State = EntityState.Modified;
            db.SaveChanges();
            EmailSender.SendBookingEmail(dbRecord);
            return RedirectToAction("Index");
        }

        public ActionResult CancelBooking(int? id)
        {
            var dbRecord = db.Bookings.Find(id);
            dbRecord.BookingStatus = "Booking Canceled";
            dbRecord.DateBooked = DateTime.Now;
            db.Entry(dbRecord).State = EntityState.Modified;
            db.SaveChanges();
            EmailSender.SendBookingEmail(dbRecord);
            return RedirectToAction("Index");
        }
        public ActionResult CheckOutBooking(int? id)
        {
            var dbRecord = db.Bookings.Find(id);
            dbRecord.BookingStatus = "Booking Checked Out";
            dbRecord.DateBooked = DateTime.Now;
            db.Entry(dbRecord).State = EntityState.Modified;
            db.SaveChanges();
            EmailSender.SendBookingEmail(dbRecord);
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
