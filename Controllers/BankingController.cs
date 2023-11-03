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
                Type = TransactionType.Deposit,
                TransactionTypeString = "Deposit",
                Date = DateTime.Now
            };

            _dbContext.Transactions.Add(transaction);
            await _dbContext.SaveChangesAsync();
            ViewBag.DepositAmount = depositAmount;
            // Redirect to the deposit confirmation page or any other desired action
            return RedirectToAction("DepositConfirmation", new { amount = depositAmount });
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
                FirstName = user.FirstName,
                Balance = user.Balance
            };

            return View(viewModel);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(string userId, decimal WithdrawAmount, WithdrawViewModel viewModel)
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
                bool isPinValid = BCrypt.Net.BCrypt.Verify(viewModel.PIN, user.PinHash);

                if (!isPinValid)
                {
                    ModelState.AddModelError("PIN", "Invalid PIN.");
                    return View(viewModel);
                }
                // Ensure the withdrawal amount is valid
                if (WithdrawAmount <= 0)
                {
                    ModelState.AddModelError("WithdrawAmount", "Invalid withdrawal amount.");

                }
                else if (WithdrawAmount > user.Balance)
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
                        Type = TransactionType.Withdraw,
                        TransactionTypeString = "Withdrawal",
                        Date = DateTime.Now

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
            viewModel.FirstName = user.FirstName;
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
                FirstName = user.FirstName,
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
            var accountNumber = user.AccountNumber;
            // Retrieve the user's account balance (you should replace this with your own logic)
            decimal balance = user.Balance;

            // Retrieve the user's recent transactions (you should replace this with your own logic)
            List<Transaction> recentTransactions = _dbContext.Transactions
                .Where(t => t.UserId == user.Id)
                .OrderByDescending(t => t.Date)
                .Take(10) // You can adjust the number of recent transactions to display
                .ToList();
            foreach (var transaction in recentTransactions)
            {
                string transactionTypeString;

                switch (transaction.Type)
                {
                    case TransactionType.Deposit:
                        transactionTypeString = "Deposit";
                        break;

                    case TransactionType.Withdraw:
                        transactionTypeString = "Withdrawal";
                        break;

                    case TransactionType.Receival:
                        transactionTypeString = "Receival";
                        break;

                    case TransactionType.Transfer:
                        transactionTypeString = "Transfer";
                        break;

                    // Handle other transaction types as needed
                    default:
                        transactionTypeString = "Unknown"; // Handle unexpected types gracefully
                        break;
                }

                transaction.TransactionTypeString = transactionTypeString;
            }
        
                // Create an IndexViewModel to pass data to the view
                var viewModel = new IndexViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Balance = balance,
                AccountNumber = accountNumber,
                RecentTransactions = recentTransactions
            };

            return View(viewModel);
        }
        public IActionResult Transfer()
        {
            //var transferViewModel = new TransferViewModel(); // Initialize an empty view model

            //return View(transferViewModel);
            var user = _userManager.GetUserAsync(User).Result;

            if (user == null)
            {
                // Handle the case where the user is not found or not authenticated
                return Redirect("/Identity/Account/Login");
            }
            var viewModel = new TransferViewModel
            {
                SourceAccountNumber = user.AccountNumber,
                SourceName = $"{user.FirstName} {user.LastName}"

            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Transfer(TransferViewModel transferViewModel)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the source and destination users based on account numbers
                var sourceUser = _dbContext.ApplicationUsers.SingleOrDefault(u => u.AccountNumber == transferViewModel.SourceAccountNumber);
                var destinationUser = _dbContext.ApplicationUsers.SingleOrDefault(u => u.AccountNumber == transferViewModel.DestinationAccountNumber);

                if (sourceUser != null && destinationUser != null)
                {
                    // Check if the source user has sufficient balance for the transfer
                    if (sourceUser.Balance >= transferViewModel.TransferAmount)
                    {
                        // Perform the funds transfer
                        sourceUser.Balance -= transferViewModel.TransferAmount;
                        destinationUser.Balance += transferViewModel.TransferAmount;
                        // Create a transaction record
                        var sourceTransaction = new Transaction
                        {
                            UserId = sourceUser.Id,
                            Type = TransactionType.Transfer,
                            TransactionTypeString = "Transfer",
                            Amount = transferViewModel.TransferAmount,
                            Date = DateTime.Now
                        };
                        var destinationTransaction = new Transaction
                        {
                            UserId = destinationUser.Id,
                            Type = TransactionType.Receival,
                            TransactionTypeString = "Receival",
                            Amount = transferViewModel.TransferAmount,
                            Date = DateTime.Now
                        };

                        _dbContext.Transactions.Add(sourceTransaction);
                        _dbContext.Transactions.Add(destinationTransaction);

                        _dbContext.SaveChanges();
                        var confirmViewModel = new ConfirmTransferViewModel
                        {
                            SourceAccountNumber = transferViewModel.SourceAccountNumber,
                            DestinationAccountNumber = transferViewModel.DestinationAccountNumber,
                            SourceName = transferViewModel.SourceName,
                            ReceiverName = transferViewModel.ReceiverName,
                            TransferAmount = transferViewModel.TransferAmount
                        };

                        // Redirect to the ConfirmTransfer view with the confirmation data
                        return RedirectToAction("ConfirmTransfer", confirmViewModel);

                    }
                    else
                    {
                        ModelState.AddModelError("TransferAmount", "Insufficient balance for the transfer.");
                    }
                }
                else
                {
                    ModelState.AddModelError("SourceAccountNumber", "Invalid source or destination account number.");
                }
            }

            // Return the view with validation errors, if any
            return View(transferViewModel);

        }

        public IActionResult ConfirmTransfer(ConfirmTransferViewModel confirmViewModel)
        {
            return View(confirmViewModel);
        }

        [HttpGet]
        [Route("Banking/GetReceiverName/{destinationAccountNumber}")]
        public JsonResult GetReceiverName(string destinationAccountNumber)
        {
            // Assuming you have an Entity Framework DbContext
            var dbContext = _dbContext;
            // Attempt to retrieve the user with the given destination account number
            var receiverUser = dbContext.ApplicationUsers.SingleOrDefault(u => u.AccountNumber == destinationAccountNumber);

                if (receiverUser != null)
                {
                    string receiverName = $"{receiverUser.FirstName} {receiverUser.LastName}";
                    return Json(receiverName); // Return the receiver's name as JSON
                }


            // If the destination account number is not found or the user doesn't exist, return an empty response or an appropriate message.
            return Json(null);
        }
    }
    }

