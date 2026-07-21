using PetWalk.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PetWalk.Services
{
    public class ReportService
    {
        public string GenerateWalkReport(List<Walk> walks, User user)
        {
            var report = new System.Text.StringBuilder();

            report.AppendLine("========================================");
            report.AppendLine("         PetWalk - Walk Report          ");
            report.AppendLine("========================================");
            report.AppendLine();
            report.AppendLine($"User: {user.GetFullName()}");
            report.AppendLine($"Generated: {DateTime.Now:dd.MM.yyyy HH:mm}");
            report.AppendLine($"Total walks: {walks.Count}");
            report.AppendLine();

            report.AppendLine("--- Summary ---");
            report.AppendLine($"Completed: {walks.Count(w => w.Status == WalkStatus.Completed)}");
            report.AppendLine($"Scheduled: {walks.Count(w => w.Status == WalkStatus.Scheduled)}");
            report.AppendLine($"Cancelled: {walks.Count(w => w.Status == WalkStatus.Cancelled)}");
            report.AppendLine($"Total spent: {walks.Where(w => w.Status == WalkStatus.Completed).Sum(w => w.Price):C}");
            report.AppendLine();

            report.AppendLine("--- Walk Details ---");

            foreach (var walk in walks.OrderByDescending(w => w.ScheduledDate))
            {
                string dogName = walk.Dog?.Name ?? "N/A";
                string otherUser = walk.Walker?.GetFullName() ?? walk.Owner?.GetFullName() ?? "N/A";

                report.AppendLine($"  {walk.ScheduledDate:dd.MM.yyyy HH:mm} | {dogName} | {otherUser} | {walk.Duration} min | {walk.Status} | {walk.Price:C}");
            }

            report.AppendLine();
            report.AppendLine("========================================");

            return report.ToString();
        }

        public void SaveReportToFile(string reportContent, string filePath)
        {
            File.WriteAllText(filePath, reportContent);
        }
    }
}
