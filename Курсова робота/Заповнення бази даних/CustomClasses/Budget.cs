namespace Лаба3
{
    public class Budget
    {
        private int income, expenses, profit;
        public string Team { set; get; }
        public int Cash { set; get; }
        public int TicketsSold { set; get; }
        public int TicketsIncome { get; set; }
        public int Advertising { get; set; }
        public int SellingPlayers { get; set; }
        public int Salary { get; set; }
        public int BuyingPlayers { get; set; }

        public int CalculateIncome()
        {
            income = TicketsIncome + Advertising + SellingPlayers;
            return income;
        }

        public int CalculateExpenses()
        {
            expenses = Salary + BuyingPlayers;
            return expenses;
        }

        public int CalculateProfit()
        {
            profit = CalculateIncome() - CalculateExpenses();
            return profit;
        }
    }
}
