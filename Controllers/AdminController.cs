using BankingSystem.Data;
using BankingSystem.Models;
using BankingSystem.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.Controllers
{
    
    public class AdminController : Controller
    {
        private readonly BankingDbContext _dbContext;

        public AdminController(BankingDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Admin()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Deposit()
        {
            var viewModel = new DepositViewModel();
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Deposit(DepositViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var accountNumber = viewModel.AccountNumber;
                var depositAmount = viewModel.DepositAmount;

                var user = _dbContext.ApplicationUsers.FirstOrDefault(u => u.AccountNumber == accountNumber);

                if (user != null)
                {
                    var transaction = new Transaction
                    {
                        Type = TransactionType.Deposit,
                        Amount = depositAmount,
                        Date = DateTime.Now,
                        UserId = user.Id,
                        TransactionTypeString = "Deposit"
                    };
                    user.Balance += depositAmount;
                    _dbContext.Transactions.Add(transaction);
                    _dbContext.SaveChanges();

                    return RedirectToAction("AdminDepositConfirmation");
                }
                else
                {
                    ModelState.AddModelError("AccountNumber", "User not found with the provided account number.");
                }

            }
            return View(viewModel);
        }

    public IActionResult AdminDepositConfirmation()
        {
            return View();
        }
        [HttpGet]
        [Route("Admin/GetUserName")]
        public JsonResult GetUserName(string accountNumber)
        {
            
            // Retrieve the user's name based on the account number from the database
            var user = _dbContext.ApplicationUsers.FirstOrDefault(u => u.AccountNumber == accountNumber);
            if (user != null)
            {
                string userName = $"{user.FirstName} {user.LastName}";
                return Json(userName);
            }
            else
            {
                return Json(null);
            }
        }
    }
}
