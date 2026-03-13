namespace MyBankApp.Core
{
    public class BankAccount
    {
        // Interest rate (STATIC)
        public static decimal InterestRate { get; private set; } = 0.05m;

        // PRIVATE
        public decimal Balance { get; private set; }

        // PUBLIC
        public string CustomerName { get; set; }

        public BankAccount(string name, decimal initialDeposit)
        {
            CustomerName = name;
            Balance = initialDeposit;
        }

        public void Deposit(decimal amount)
        {
            if (amount > 0)
            {
                Balance += amount;
                Console.WriteLine($"Deposited {amount}");
            }
        }
    }
}
