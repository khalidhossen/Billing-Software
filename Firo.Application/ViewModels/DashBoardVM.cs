using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firo.Application.ViewModels
{
    public class DashBoardVM
    {
        public int TotalCustomers { get; set; }
        public int TodaysNewCustomers { get; set; }

        
        public decimal TotalSales { get; set; }
        public decimal TodaysSales { get; set; }

      
        public decimal TotalDue { get; set; }
        public decimal TodaysDue { get; set; }

        
        public List<string> DailyLabels { get; set; }
        public List<decimal> DailySales { get; set; }
        public List<int> DailyCustomers { get; set; }

        
        public List<string> MonthlyLabels { get; set; }
        public List<decimal> MonthlySales { get; set; }
        public List<int> MonthlyCustomers { get; set; }
    }
}
