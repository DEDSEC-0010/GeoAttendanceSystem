using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoAttendance.Common.Models;
public class AttendanceRecord
{
    public int Id { get; set; }
    public DateTime TimeStamp { get; set; }
    public bool IsPresent { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int EmployeeId { get; set; }
}
