using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Zealand_Booking_System_Library.Models;
using Zealand_Booking_System_Library.Repository;
using Zealand_Booking_System_Library.Service;

namespace Zealand_Booking_System.Pages
{
    /// <summary>
    /// Responsibility:
    /// - Lets users search for available rooms.
    /// - Allows users to create a booking.
    ///
    /// Why this page exists:
    /// - To give users a simple way to find and book rooms.
    /// - To keep room availability and booking logic in one place.
    /// </summary>
    public class FindRoomsModel : PageModel
    {
     

        /// <summary>
        /// Handles booking logic and room availability.
        /// </summary>
        private readonly BookingService _bookingService;

        /// <summary>
        /// Message shown to the user after actions.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Sets up repositories and the booking service.
        /// </summary>
        public FindRoomsModel(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Selected date from the filter form.
        /// </summary>
        [BindProperty]
        public DateTime SelectedDate { get; set; }

        /// <summary>
        /// Selected time slot from the filter form.
        /// </summary>
        [BindProperty]
        public TimeSlot SelectedTimeSlot { get; set; }

        /// <summary>
        /// Optional room type filter.
        /// </summary>
        [BindProperty]
        public RoomType? SelectedRoomType { get; set; }

        /// <summary>
        /// Optional smartboard filter.
        /// </summary>
        [BindProperty]
        public bool? SelectedSmartBoard { get; set; }

        /// <summary>
        /// Room selected for booking.
        /// </summary>
        [BindProperty]
        public int RoomID { get; set; }

        /// <summary>
        /// List of rooms with availability info.
        /// </summary>
        public List<RoomAvailability> Rooms { get; private set; } = new();

        /// <summary>
        /// Loads the page with default filter values.
        /// </summary>
        public void OnGet()
        {
            SelectedDate = DateTime.Today;
            SelectedTimeSlot = TimeSlot.Slot08_10;
            SelectedRoomType = null;

            LoadRooms();
        }

        /// <summary>
        /// Updates the room list based on selected filters.
        /// </summary>
        public void OnPost()
        {
            if (SelectedDate == DateTime.MinValue)
            {
                SelectedDate = DateTime.Today;
            }

            LoadRooms();
            Message = "The list has been updated.";
        }

        /// <summary>
        /// Creates a booking for the selected room.
        /// </summary>
        public IActionResult OnPostBook()
        {
            int? accountId = HttpContext.Session.GetInt32("AccountID");

            if (accountId == null)
            {
                TempData["Message"] = "You need to be logged in to make a booking.";
                return RedirectToPage("/BookingList");
            }

            Booking newBooking = new Booking
            {
                RoomID = RoomID,
                BookingDate = SelectedDate,
                TimeSlot = SelectedTimeSlot,
                AccountID = accountId.Value
            };

            try
            {
                _bookingService.Add(newBooking);
                TempData["Message"] = "Booking made!";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error: " + ex.Message;
            }

            return RedirectToPage(new
            {
                SelectedDate,
                SelectedTimeSlot,
                SelectedRoomType
            });
        }

        /// <summary>
        /// Loads room availability from the service.
        /// </summary>
        private void LoadRooms()
        {
            Rooms = _bookingService.GetRoomAvailability(
                SelectedDate,
                SelectedTimeSlot,
                SelectedRoomType,
                SelectedSmartBoard
            );
        }
    }
}
