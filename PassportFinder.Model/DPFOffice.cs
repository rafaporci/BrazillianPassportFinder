using System;
using System.Collections.Generic;
using System.Text;

namespace PassportFinder.Model
{
    public class DPFOffice
    {
        public string Id { get; set; }
        public string Name { get; set; }        
        public bool IsAppointmentMandatory { get; set; }
        public string Alerts { get; set; }
        public bool HaveAlerts
        { 
            get 
            { 
                return !String.IsNullOrEmpty(Alerts);
            }
        }

    }
}
