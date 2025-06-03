using iServiceRepositories.Repositories.Models;

namespace iServiceServices.Services
{
    public class AppointmentFinderService
    {
        public List<TimeSpan> FindAvailableSlots(
            Schedule schedule,
            List<SpecialSchedule> specialSchedules,
            Service service,
            DateTime date,
            List<Appointment> existingAppointments,
            List<EstablishmentEmployee> establishmentEmployees)
        {
            List<TimeSpan> availableSlots = new List<TimeSpan>();
            var dayOfWeek = (int)date.DayOfWeek;
            var specialDay = specialSchedules.FirstOrDefault(sd => sd.Date.Date == date.Date);

            if (!schedule.Days.Contains(dayOfWeek.ToString()) && specialDay == null) return new List<TimeSpan>();

            var start = specialDay != null ? ParseTime(specialDay.Start) : ParseTime(schedule?.Start);
            var end = specialDay != null ? ParseTime(specialDay.End) : ParseTime(schedule?.End);
            var breakStart = specialDay != null ? ParseTime(specialDay.BreakStart) : ParseTime(schedule?.BreakStart);
            var breakEnd = specialDay != null ? ParseTime(specialDay.BreakEnd) : ParseTime(schedule?.BreakEnd);

            TimeSpan currentTime = start.GetValueOrDefault();

            if (date.Date == DateTime.Today)
            {
                DateTime now = DateTime.Now;
                int minutes = now.Minute;
                int roundedMinutes = ((minutes + 14) / 15) * 15;
                currentTime = roundedMinutes == 60 ?
                    new TimeSpan(now.Hour + 1, 0, 0) :
                    new TimeSpan(now.Hour, roundedMinutes, 0);
            }

            while (currentTime.Add(TimeSpan.FromMinutes(service.EstimatedDuration)) <= end)
            {
                var potentialEndTime = currentTime.Add(TimeSpan.FromMinutes(service.EstimatedDuration));

                if (breakStart.HasValue && breakEnd.HasValue && currentTime < breakEnd && potentialEndTime > breakStart)
                {
                    currentTime = breakEnd.Value > currentTime ? breakEnd.Value : currentTime.Add(TimeSpan.FromMinutes(15));
                    continue;
                }

                // Verifica se há pelo menos um funcionário disponível neste intervalo
                var isTimeSlotAvailable = establishmentEmployees.Any(emp =>
                    !existingAppointments.Any(app =>
                        app.EstablishmentEmployeeId == emp.EstablishmentEmployeeId &&
                        app.Start.Date == date.Date &&
                        (currentTime < app.End.TimeOfDay && potentialEndTime > app.Start.TimeOfDay)));

                if (isTimeSlotAvailable)
                {
                    availableSlots.Add(currentTime);
                }

                currentTime = currentTime.Add(TimeSpan.FromMinutes(15));
            }

            return availableSlots;
        }

        TimeSpan? ParseTime(string timeString)
        {
            if (string.IsNullOrEmpty(timeString))
                return null;

            TimeSpan parsedTime;
            if (TimeSpan.TryParse(timeString, out parsedTime))
                return parsedTime;

            return null;
        }
    }
}
