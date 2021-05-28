using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentTracker.Models
{
    public class PieChart
    {
        public List<string> labels { get; set; }
        public List<PieChartDataSet> datasets { get; set; }

        public PieChart()
        {
            labels = new List<string>();
            datasets = new List<PieChartDataSet>(){ new PieChartDataSet()};
        }
    }

    public class PieChartDataSet
    {
        public List<string> backgroundColor { get; set; }
        public List<int> data { get; set; }

        public PieChartDataSet()
        {
            backgroundColor = new List<string>();
            data = new List<int>();
        }

        public PieChartDataSet(List<string> backgroundColor, List<int> data)
        {
            this.backgroundColor = backgroundColor;
            this.data = data;
        }
    }
}
