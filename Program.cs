using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Transaction
{
    public int CorrelationId { get; set; }
    public string DateTime { get; set; }
    public ulong OriginCc { get; set; }
    public ulong DestinyCc { get; set; }
    public decimal Value { get; set; }
}

class AccountBalance
{
    public AccountBalance(ulong account, decimal balance)
    {
        this.account = account;
        this.balance = balance;
    }
    public ulong account { get; set; }
    public decimal balance { get; set; }
}


//Obs: Voce é livre para implementar na linguagem de sua preferência, desde que respeite as funcionalidades e saídas existentes, além de aplicar os conceitos solicitados.

namespace FinancialTransaction
{
    class Program
    {

        static void Main(string[] args)
        {
            var Transaction = new Transaction[]
                {
                    new Transaction { CorrelationId = 1, DateTime = "09/09/2023 14:15:00", OriginCc = 938485762, DestinyCc = 2147483649, Value = 150 },
                    new Transaction { CorrelationId = 2, DateTime = "09/09/2023 14:15:05", OriginCc = 2147483649, DestinyCc = 210385733, Value = 149 },
                    new Transaction { CorrelationId = 3, DateTime = "09/09/2023 14:15:29", OriginCc = 347586970, DestinyCc = 238596054, Value = 1100 },
                    new Transaction { CorrelationId = 4, DateTime = "09/09/2023 14:17:00", OriginCc = 675869708, DestinyCc = 210385733, Value = 5300 },
                    new Transaction { CorrelationId = 5, DateTime = "09/09/2023 14:18:00", OriginCc = 238596054, DestinyCc = 674038564, Value = 1489 },
                    new Transaction { CorrelationId = 6, DateTime = "09/09/2023 14:18:20", OriginCc = 573659065, DestinyCc = 563856300, Value = 49 },
                    new Transaction { CorrelationId = 7, DateTime = "09/09/2023 14:19:00", OriginCc = 938485762, DestinyCc = 2147483649, Value = 44 },
                    new Transaction { CorrelationId = 8, DateTime = "09/09/2023 14:19:01", OriginCc = 573659065, DestinyCc = 675869708, Value = 150 },
                };

            ExecFinancialTransaction executor = new ExecFinancialTransaction();
            foreach (var item in Transaction)
            {
                executor.transfer(item.CorrelationId, item.OriginCc, item.DestinyCc, item.Value);
            }

        }
    }

    class ExecFinancialTransaction : DataAccess
    {
        public void transfer(int CorrelationId, ulong OriginCc, ulong DestinyCc, decimal Value)
        {
            AccountBalance originBalance = getBalance<AccountBalance>(OriginCc);
            AccountBalance destinyBalance = getBalance<AccountBalance>(DestinyCc);
            if (originBalance.balance < Value)
            {
                Console.WriteLine("Transacao numero {0} foi cancelada por falta de saldo", CorrelationId);
            }
            else
            {
                if (updateCcs(originBalance, destinyBalance, Value))
                {
                    Console.WriteLine("Transacao numero {0} foi efetivada com sucesso! Novos saldos: Conta Origem:{1} | Conta Destino: {2}", CorrelationId, originBalance.balance, destinyBalance.balance);
                }
                else
                {
                    Console.WriteLine("Houve uma falha em sua transação", CorrelationId);
                }
            }
        }
    }

    class DataAccess
    {
        Dictionary<ulong, decimal> Balances { get; set; }
        private List<AccountBalance> BalanceTable;
        public DataAccess()
        {
            BalanceTable = new List<AccountBalance>();
            BalanceTable.Add(new AccountBalance(938485762, 180));
            BalanceTable.Add(new AccountBalance(347586970, 1200));
            BalanceTable.Add(new AccountBalance(2147483649, 0));
            BalanceTable.Add(new AccountBalance(675869708, 4900));
            BalanceTable.Add(new AccountBalance(238596054, 478));
            BalanceTable.Add(new AccountBalance(573659065, 787));
            BalanceTable.Add(new AccountBalance(210385733, 10));
            BalanceTable.Add(new AccountBalance(674038564, 400));
            BalanceTable.Add(new AccountBalance(563856300, 1200));


            Balances = new Dictionary<ulong, decimal>();
            this.Balances.Add(938485762, 180);

        }
        public T getBalance<T>(ulong id)
        {
            return (T)Convert.ChangeType(BalanceTable.Find(x => x.account == id), typeof(T));
        }
        public bool updateCcs(AccountBalance origin, AccountBalance destiny, decimal value)
        {
            try
            {
                origin.balance -= value;
                destiny.balance += value;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

    }
}