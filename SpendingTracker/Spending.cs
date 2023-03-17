using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendingTracker
{
    public class Spending
    {
        public int id;
        public int account_id;
        public string category;
        public int cost;

        public Spending(int id, int account_id, string category, int cost)
        {
            this.id = id;
            this.account_id = account_id;
            this.category = category;
            this.cost = cost;

        }
    }
}