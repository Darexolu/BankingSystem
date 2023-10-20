using BankingSystem.Data;
using BankingSystem.Models;
using BankingSystem.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Controllers
{
    public class BankingController : Controller
    {
        private readonly BankingDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public BankingController(BankingDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Deposit()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }
            else {
                var viewModel = new DepositViewModel
                {
                    UserId = user.Id
                };
                //TempData["UserId"] = userId;
                ViewData["UserId"] = viewModel;
                return View(viewModel);
                }
        }
        [HttpPost]
        public async Task<IActionResult> Deposit(string userId, decimal depositAmount)
        {
            //if (TempData["UserId"] is string userId)
            //{
                ApplicationUser user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound();
                }

                user.Balance += depositAmount;

                // Create a transaction record (assuming you have a Transaction model)
                var transaction = new Transaction
                {
                    UserId = userId,
                    Amount = depositAmount,
                    Date = DateTime.Now,
                    Type = TransactionType.Deposit
                };

                _dbContext.Transactions.Add(transaction);
                await _dbContext.SaveChangesAsync();
                ViewBag.DepositAmount = depositAmount;
                // Redirect to the deposit confirmation page or any other desired action
                return RedirectToAction("DepositConfirmation",new { amount = depositAmount });
            //}
            //else
            //{
            //    return NotFound();
            //}
        }
        [HttpGet]
        public IActionResult DepositConfirmation(decimal amount)
        {
            var viewModel = new DepositConfirmationViewModel
            {
                AmountDeposited = amount
            };
            return View(viewModel);
        }

        // Withdraw action
        [HttpGet]
        public async Task<IActionResult> Withdraw()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Redirect("/Identity/Account/Login"); // Redirect to the login page if the user is not logged in
            }

            var viewModel = new WithdrawViewModel
            {
                UserId = user.Id,
                Balance = user.Balance
            };

            return View(viewModel);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(string userId,decimal WithdrawAmount, WithdrawViewModel viewModel)
        {
            // Retrieve the user
            ApplicationUser user = await _userManager.FindByIdAsync(userId);


            if (user == null)
            {
                // Handle the case where the user is not found
                return NotFound();
            }

            // Validate the withdrawal amount
            if (ModelState.IsValid)
            {
                // Ensure the withdrawal amount is valid
                if (WithdrawAmount <= 0 )
                {
                    ModelState.AddModelError("WithdrawAmount", "Invalid withdrawal amount.");                 

                }
                else if( WithdrawAmount > user.Balance)
                {
                    ModelState.AddModelError("WithdrawAmount", "Insufficient Balance");
                }
                else
                {
                    // Deduct the withdrawal amount from the user's balance
                    user.Balance -= WithdrawAmount;



                    var transaction = new Transaction
                    {
                        UserId = user.Id,
                        Amount = -WithdrawAmount, // Negative amount for withdrawals
                        Date = DateTime.Now,
                        Type = TransactionType.Withdraw
                    };


                    // Save changes to the database
                    _dbContext.Transactions.Add(transaction);
                    _dbContext.SaveChanges();
                    var confirmationViewModel = new WithdrawConfirmationViewModel
                    {
                        WithdrawnAmount = WithdrawAmount,
                        NewBalance = user.Balance
                    };

                    // Redirect to a confirmation page or take other actions
                    return View("WithdrawConfirmation", confirmationViewModel);
                }
            }
            // If there are validation errors, redisplay the Withdraw view with validation messages

            viewModel.Balance = user.Balance;
            return View("Withdraw", viewModel);
        }
        public IActionResult WithdrawConfirmation()
        {
           
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> CheckBalance()
        {
            
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    // Handle the case where the user is not found
                    return Redirect("/Identity/Account/Login");
                }
                var viewModel = new CheckBalanceViewModel
                {
                    Balance = user.Balance
                };

                return View(viewModel);
            
            
        }
        public IActionResult Index()
        {
            // Retrieve the currently authenticated user
            var user = _userManager.GetUserAsync(User).Result;

            if (user == null)
            {
                // Handle the case where the user is not found or not authenticated
                return Redirect("/Identity/Account/Login");
            }

            // Retrieve the user's account balance (you should replace this with your own logic)
            decimal balance = user.Balance;

            // Retrieve the user's recent transactions (you should replace this with your own logic)
            List<Transaction> recentTransactions = _dbContext.Transactions
                .Where(t => t.UserId == user.Id)
                .OrderByDescending(t => t.Date)
                .Take(10) // You can adjust the number of recent transactions to display
                .ToList();
            // Create an IndexViewModel to pass data to the view
            var viewModel = new IndexViewModel
            {
                //UserName = user.UserName,
                Balance = balance,
                RecentTransactions = recentTransactions
            };

            return View(viewModel);
        }
    }
}