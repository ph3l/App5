using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace App5
{
    public class TideInformation
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Date { get; set; }
        public string firstLowTide { get; set; }
        public string firstHighTide { get; set; }
        public string secondLowTide { get; set; }
        public string secondHighTide { get; set; }
    }
}
