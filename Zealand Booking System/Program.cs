/*
 * Program.cs
 *
 * Purpose:
 * This file defines the application startup configuration for the Zealand Booking System.
 * It is responsible for configuring dependency injection, security-related services,
 * session handling, and the HTTP request pipeline.
 *
 * The configuration focuses on:
 * - Registering repositories and services to support clean separation of concerns
 * - Enabling session-based user interaction
 * - Ensuring secure handling of sensitive data through data protection
 */
using Microsoft.AspNetCore.DataProtection;
using Zealand_Booking_System_Library.Repository;
using Zealand_Booking_System_Library.Service;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages are used as the UI framework to support a page-focused architecture
builder.Services.AddRazorPages();

// The connection string is resolved once here to avoid repeated configuration lookups

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Repositories and services are registered with scoped lifetimes to ensure
// each HTTP request works with its own data context and avoids shared state issues
builder.Services.AddScoped<IRoomRepository>(provider => new RoomCollectionRepo(connectionString));
builder.Services.AddScoped<RoomService>();

builder.Services.AddScoped<IUserRepository>(provider => new UserCollectionRepo(connectionString));
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<INotificationRepository>(provider => new NotificationCollectionRepo(connectionString));
builder.Services.AddScoped<NotificationService>();

builder.Services.AddScoped<IBookingRepository>(provider => new BookingCollectionRepo(connectionString));
builder.Services.AddScoped<BookingService>();

// Data Protection is configured to persist keys outside the application directory
// to ensure encrypted data remains readable across restarts and deployments
builder.Services.AddDataProtection()
       .PersistKeysToFileSystem(new DirectoryInfo(@"C:\Dataprotection-Keys"))
       .SetApplicationName("ZealandBookingSystem");


// Session support is enabled to maintain user-specific state such as login status
// with a controlled lifetime to balance usability and security
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// HttpContextAccessor is registered to allow services to access request-specific context when necessary
builder.Services.AddHttpContextAccessor();

var app = builder.Build();


// A dedicated logout endpoint is mapped to explicitly clear session data,
// ensuring no residual user state remains after logout
app.MapPost("/logout", (HttpContext context) =>
{
    context.Session.Clear();
    return Results.Redirect("/Index");
});

// Production-specific middleware is applied to improve error handling
// and enforce strict transport security
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
// Middleware order is carefully structured to ensure correct request handling,
// session availability, and authorization enforcement
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
// Razor Pages are mapped last to ensure all required middleware is active beforehand
app.MapRazorPages();
app.Run();