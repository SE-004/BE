using MyBankApp.Core;

var account1 = new BankAccount("Bob", 1000m);
var account2 = new BankAccount("Jenna", 5000m);

List<BankAccount> accounts = [account1, account2];

Console.WriteLine(account1.Balance);
