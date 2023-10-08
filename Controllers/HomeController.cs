#pragma warning disable CS8604
#pragma warning disable CS8603
#pragma warning disable CS8600

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeedingPlanner.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WeedingPlanner.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;
    private bool InSession
    {
        get { return HttpContext.Session.GetInt32("userId") != null; }
    }
    private User LoggedInUser
    {
        get { return _context.Users.Include(u => u.MyWeddings).FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("userId")); }
    }
    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {

        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    //_____Register________
    [HttpPost("/user/register")]
    public IActionResult Register(User newUser)
    {
        if (ModelState.IsValid)
        {
            if (_context.Users.Any(u => u.Email == newUser.Email))
            {
                ModelState.AddModelError("Email", "Email already taken");
                return View("Index");
            }
            //Hashing password
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            newUser.Password = hasher.HashPassword(newUser, newUser.Password);
            //Save to db
            _context.Add(newUser);
            _context.SaveChanges();
            TempData["Message"] = "Registered Successfully";
            return RedirectToAction("Index");
        }
        return View("Index");
    }
    //_____Login_________
    [HttpPost("/user/login")]
    public IActionResult Login(Login logUser)
    {
        if (ModelState.IsValid)
        {
            User? userInDb = _context.Users.FirstOrDefault(u => u.Email == logUser.LoginEmail);
            if (userInDb == null)
            {
                ModelState.AddModelError("LoginEmail", "Incorrect Validation");
                return View("Index");
            }
            //comparing pass
            PasswordHasher<Login> hasher = new PasswordHasher<Login>();
            var result = hasher.VerifyHashedPassword(logUser, userInDb.Password, logUser.LoginPassword);
            if (result == 0)
            {
                ModelState.AddModelError("LoginEmail", "Incorrect Validation");
                return View("Index");
            }
            HttpContext.Session.SetInt32("userId", userInDb.UserId);
            return RedirectToAction("Dashboard");
        }
        return View("Index");
    }
    //________logout__________
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");

    }


    //______DAshboard_______
    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        if (!InSession)
        {
            return View("Index");
        }
        ViewBag.User = LoggedInUser;
        var All = _context.Weddings
                        .Include(u => u.Guest)
                        .ThenInclude(w => w.User).ToList();
        return View(All);
    }
    //______WeedingPlan_________
    [HttpGet("wedding")]
    public IActionResult WeddingPlan()
    {
        if (!InSession)
        {
            return RedirectToAction("Index");
        }
        ViewBag.User = LoggedInUser;
        return View();
    }
    //______Add_New_Wedding_
    [HttpPost("/wedding/new")]
    public IActionResult AddWedding(Wedding newWedding)
    {
        if (!InSession)
        {
            return RedirectToAction("Index");
        }
        _context.Weddings.Add(newWedding);
        _context.SaveChanges();
        return RedirectToAction("Dashboard");
    }
    //______Delete_Wedding_____
    [HttpGet("/delete/{WeddingId}")]
    public IActionResult Delete(int WeddingId)
    {
        if (!InSession)
        {
            return View("Index");
        }
        Wedding? weddingToDelete = _context.Weddings.SingleOrDefault(w => w.WeddingId == WeddingId);
        if (weddingToDelete is not null)
        {
            _context.Weddings.Remove(weddingToDelete);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        return View("Dashboard");
    }
    //_______RSVP_______________
    [HttpPost("rsvp")]
    public IActionResult Rsvp(int Wedding_Id)
    {
        Invitation guest = new Invitation
        {
            UserId = LoggedInUser.UserId,
            WeddingId = Wedding_Id
        };
        _context.Invitations.Add(guest);
        _context.SaveChanges();
        return RedirectToAction("Dashboard");
    }
    //_________UN-RSVP_________
    [HttpPost("unrsvp")]
    public IActionResult UnRsvp(int Wedding_Id)
    {
        Invitation invit = _context.Invitations
                            .Where(i => i.WeddingId == Wedding_Id)
                            .FirstOrDefault(i => i.UserId == LoggedInUser.UserId);

        _context.Invitations.Remove(invit);
        _context.SaveChanges();

        return RedirectToAction("Dashboard");
    }
    //__________Wedding_List_View________
    [HttpGet("/wedding/{WeddingId}")]
    public IActionResult ListWedding(int WeddingId)
    {
        ViewBag.User = LoggedInUser;
        var Guests = _context.Weddings.Include(g=>g.Guest)
                                            .ThenInclude(u=>u.User)
                                            .FirstOrDefault(w=>w.WeddingId==WeddingId);

        return View(Guests);
    }

}
