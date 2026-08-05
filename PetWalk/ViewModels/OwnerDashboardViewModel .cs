using PetWalk.Data;
using PetWalk.Helpers;
using PetWalk.Models;
using PetWalk.Repositories;
using PetWalk.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace PetWalk.ViewModels
{
    public class OwnerDashboardViewModel : BaseViewModel
    {
        private readonly Owner _owner;
        private readonly WalkService _walkService;
        private readonly DogRepository _dogRepository;
        private readonly UserRepository _userRepository;
        private readonly SerializationService _serializationService;
        private readonly ReportService _reportService;
        private readonly PetWalkDbContext _context;

        private ObservableCollection<Dog> _dogs = new();
        private Dog? _selectedDog;
        private string _newDogName = string.Empty;
        private string _newDogBreed = string.Empty;
        private int _newDogAge;
        private double _newDogWeight;
        private string _newDogNote = string.Empty;

        private ObservableCollection<Walker> _walkers = new();
        private Walker? _selectedWalker;
        private string _walkerSearchText = string.Empty;

        private ObservableCollection<Walk> _walkHistory = new();
        private Walk? _selectedWalk;
        private DateTime _walkDate = DateTime.Now.AddDays(1);
        private int _walkDuration = 30;

        private int _reviewRating = 5;
        private string _reviewComment = string.Empty;

        private string _statusMessage = string.Empty;
        private string _notificationMessage = string.Empty;

        private string _selectedWalkerDetails = string.Empty;

        private ObservableCollection<string> _availableTimeSlots = new();
        private string? _selectedTimeSlot;

        private OwnerObserver _observer;

        public OwnerDashboardViewModel(Owner owner)
        {
            _owner = owner;
            _context = new PetWalkDbContext();
            _walkService = new WalkService(_context);
            _dogRepository = new DogRepository(_context);
            _userRepository = new UserRepository(_context);
            _serializationService = new SerializationService();
            _reportService = new ReportService();
            _observer = new OwnerObserver(_owner);

            AddDogCommand = new RelayCommand(ExecuteAddDog, _ => !string.IsNullOrWhiteSpace(NewDogName));
            RemoveDogCommand = new RelayCommand(ExecuteRemoveDog, _ => SelectedDog != null);
            SearchWalkersCommand = new RelayCommand(_ => LoadWalkers());
            ScheduleWalkCommand = new RelayCommand(ExecuteScheduleWalk, CanScheduleWalk);
            LeaveReviewCommand = new RelayCommand(ExecuteLeaveReview, CanLeaveReview);
            ExportJsonCommand = new RelayCommand(_ => ExportToJson());
            ExportXmlCommand = new RelayCommand(_ => ExportToXml());
            GenerateReportCommand = new RelayCommand(_ => GenerateReport());
            LogoutCommand = new RelayCommand(_ => OnLogout());

            LoadData();
        }

        public string WelcomeMessage => $"Welcome, {_owner.GetFullName()}!";

        public ObservableCollection<Dog> Dogs
        {
            get => _dogs;
            set => SetProperty(ref _dogs, value);
        }

        public Dog? SelectedDog
        {
            get => _selectedDog;
            set => SetProperty(ref _selectedDog, value);
        }

        public string NewDogName
        {
            get => _newDogName;
            set => SetProperty(ref _newDogName, value);
        }

        public string NewDogBreed
        {
            get => _newDogBreed;
            set => SetProperty(ref _newDogBreed, value);
        }

        public int NewDogAge
        {
            get => _newDogAge;
            set => SetProperty(ref _newDogAge, value);
        }

        public double NewDogWeight
        {
            get => _newDogWeight;
            set => SetProperty(ref _newDogWeight, value);
        }

        public string NewDogNote
        {
            get => _newDogNote;
            set => SetProperty(ref _newDogNote, value);
        }

        public ObservableCollection<Walker> Walkers
        {
            get => _walkers;
            set => SetProperty(ref _walkers, value);
        }

        public Walker? SelectedWalker
        {
            get => _selectedWalker;
            set
            {
                SetProperty(ref _selectedWalker, value);
                LoadWalkerDetails();
                LoadAvailableTimeSlots();
            }
        }

        public string WalkerSearchText
        {
            get => _walkerSearchText;
            set => SetProperty(ref _walkerSearchText, value);
        }

        public ObservableCollection<Walk> WalkHistory
        {
            get => _walkHistory;
            set => SetProperty(ref _walkHistory, value);
        }

        public Walk? SelectedWalk
        {
            get => _selectedWalk;
            set => SetProperty(ref _selectedWalk, value);
        }

        public DateTime WalkDate
        {
            get => _walkDate;
            set
            {
                SetProperty(ref _walkDate, value);
                LoadAvailableTimeSlots();
            }
        }

        public int WalkDuration
        {
            get => _walkDuration;
            set
            {
                SetProperty(ref _walkDuration, value);
                LoadAvailableTimeSlots();
            }
        }

        public int ReviewRating
        {
            get => _reviewRating;
            set => SetProperty(ref _reviewRating, value);
        }

        public string ReviewComment
        {
            get => _reviewComment;
            set => SetProperty(ref _reviewComment, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public string NotificationMessage
        {
            get => _notificationMessage;
            set => SetProperty(ref _notificationMessage, value);
        }

        public string SelectedWalkerDetails
        {
            get => _selectedWalkerDetails;
            set => SetProperty(ref _selectedWalkerDetails, value);
        }
        public ObservableCollection<string> AvailableTimeSlots
        {
            get => _availableTimeSlots;
            set => SetProperty(ref _availableTimeSlots, value);
        }

        public string? SelectedTimeSlot
        {
            get => _selectedTimeSlot;
            set => SetProperty(ref _selectedTimeSlot, value);
        }

        public ICommand AddDogCommand { get; }
        public ICommand RemoveDogCommand { get; }
        public ICommand SearchWalkersCommand { get; }
        public ICommand ScheduleWalkCommand { get; }
        public ICommand LeaveReviewCommand { get; }
        public ICommand ExportJsonCommand { get; }
        public ICommand ExportXmlCommand { get; }
        public ICommand GenerateReportCommand { get; }
        public ICommand LogoutCommand { get; }

        public event Action? LogoutRequested;

        private void LoadData()
        {
            LoadDogs();
            LoadWalkers();
            LoadWalkHistory();
        }

        private void LoadDogs()
        {
            var dogs = _dogRepository.GetByOwnerId(_owner.Id);
            Dogs = new ObservableCollection<Dog>(dogs);
        }

        private void LoadWalkers()
        {
            var walkers = _userRepository.GetAvailableWalkers();

            if (!string.IsNullOrWhiteSpace(WalkerSearchText))
            {
                walkers = walkers.Where(w =>
                    w.GetFullName().Contains(WalkerSearchText, StringComparison.OrdinalIgnoreCase) ||
                    w.Location.Contains(WalkerSearchText, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            Walkers = new ObservableCollection<Walker>(walkers);
        }

        private void LoadWalkHistory()
        {
            var walks = _walkService.GetWalksByOwnerId(_owner.Id);
            WalkHistory = new ObservableCollection<Walk>(walks);
        }

        private void LoadWalkerDetails()
        {
            if (SelectedWalker == null)
            {
                SelectedWalkerDetails = string.Empty;
                return;
            }

            using var context = new PetWalkDbContext();
            var walker = context.Walkers.Find(SelectedWalker.Id);
            var reviews = context.Reviews
                .Where(r => r.WalkerId == SelectedWalker.Id)
                .ToList();
            var totalWalks = context.Walks
                .Where(w => w.WalkerId == SelectedWalker.Id && w.Status == WalkStatus.Completed)
                .Count();

            var details = new System.Text.StringBuilder();
            details.AppendLine($"Name: {SelectedWalker.GetFullName()}");
            details.AppendLine($"Location: {SelectedWalker.Location}");
            details.AppendLine($"Rate: {SelectedWalker.HourlyRate}€/hr");
            details.AppendLine($"Completed walks: {totalWalks}");
            details.AppendLine($"Total reviews: {reviews.Count}");

            if (reviews.Count > 0)
            {
                double avg = reviews.Average(r => r.Rating);
                details.AppendLine($"Average rating: {avg.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)}/5.0");
                details.AppendLine();
                details.AppendLine("--- Reviews ---");
                foreach (var review in reviews.OrderByDescending(r => r.Date))
                {
                    var owner = context.Owners.Find(review.OwnerId);
                    string ownerName = owner?.GetFullName() ?? "Unknown";
                    details.AppendLine($"★ {review.Rating}/5 by {ownerName} ({review.Date:dd.MM.yyyy})");
                    if (!string.IsNullOrWhiteSpace(review.Comment))
                    {
                        details.AppendLine($"  \"{review.Comment}\"");
                    }
                }
            }
            else
            {
                details.AppendLine("No reviews yet.");
            }

            SelectedWalkerDetails = details.ToString();
        }

        private void ExecuteAddDog(object? parameter)
        {
            var dog = new Dog
            {
                Name = NewDogName,
                Breed = NewDogBreed,
                Age = NewDogAge,
                Weight = NewDogWeight,
                Note = NewDogNote,
                OwnerId = _owner.Id
            };

            _dogRepository.Add(dog);
            LoadDogs();

            NewDogName = string.Empty;
            NewDogBreed = string.Empty;
            NewDogAge = 0;
            NewDogWeight = 0;
            NewDogNote = string.Empty;

            StatusMessage = $"Dog '{dog.Name}' added successfully!";
        }

        private void ExecuteRemoveDog(object? parameter)
        {
            if (SelectedDog != null)
            {
                string name = SelectedDog.Name;
                _dogRepository.Delete(SelectedDog.Id);
                LoadDogs();
                StatusMessage = $"Dog '{name}' removed.";
            }
        }

        private bool CanScheduleWalk(object? parameter)
        {
            return SelectedWalker != null &&
               SelectedDog != null &&
               SelectedTimeSlot != null &&
               SelectedTimeSlot != "No availability for this day" &&
               SelectedTimeSlot != "No available slots";
        }

        private void ExecuteScheduleWalk(object? parameter)
        {
            if (SelectedWalker == null || SelectedDog == null || SelectedTimeSlot == null) return;

            if (SelectedTimeSlot == "No availability for this day" ||
                SelectedTimeSlot == "No available slots")
            {
                StatusMessage = "Please select a valid time slot.";
                return;
            }

            var time = TimeSpan.Parse(SelectedTimeSlot);
            var scheduledDate = WalkDate.Date.Add(time);

            decimal price = SelectedWalker.HourlyRate * (WalkDuration / 60.0m);

            var walk = _walkService.ScheduleWalk(
                _owner.Id,
                SelectedWalker.Id,
                SelectedDog.Id,
                scheduledDate,
                WalkDuration,
                price
            );

            walk.Attach(_observer);
            walk.Notify();

            NotificationMessage = _observer.LastNotification;

            LoadWalkHistory();
            LoadAvailableTimeSlots();
            StatusMessage = $"Walk scheduled for {scheduledDate:dd.MM.yyyy HH:mm}!";
        }

        private void LoadAvailableTimeSlots()
        {
            AvailableTimeSlots.Clear();
            SelectedTimeSlot = null;

            if (SelectedWalker == null) return;

            using var context = new PetWalkDbContext();

            var selectedDate = WalkDate.Date;

            var slots = context.AvailabilitySlots
                .Where(a => a.WalkerId == SelectedWalker.Id)
                .ToList()
                .Where(a => a.Date.Date == selectedDate)
                .ToList();

            if (slots.Count == 0)
            {
                AvailableTimeSlots.Add("No availability for this date");
                return;
            }

            var existingWalks = context.Walks
                .Where(w => w.WalkerId == SelectedWalker.Id)
                .ToList()
                .Where(w => w.ScheduledDate.Date == selectedDate &&
                            (w.Status == WalkStatus.Scheduled ||
                             w.Status == WalkStatus.Accepted ||
                             w.Status == WalkStatus.InProgress))
                .ToList();

            var available = new List<string>();

            foreach (var slot in slots)
            {
                var current = slot.StartTime;
                while (current.Add(TimeSpan.FromMinutes(WalkDuration)) <= slot.EndTime)
                {
                    var slotStart = selectedDate.Add(current);
                    var slotEnd = slotStart.AddMinutes(WalkDuration);

                    bool isBusy = existingWalks.Any(w =>
                        slotStart < w.ScheduledDate.AddMinutes(w.Duration) &&
                        slotEnd > w.ScheduledDate
                    );

                    if (!isBusy && slotStart > DateTime.Now)
                    {
                        available.Add(current.ToString(@"hh\:mm"));
                    }

                    current = current.Add(TimeSpan.FromMinutes(30));
                }
            }

            if (available.Count == 0)
            {
                AvailableTimeSlots.Add("No available slots");
            }
            else
            {
                foreach (var time in available)
                {
                    AvailableTimeSlots.Add(time);
                }
            }
        }

        private bool CanLeaveReview(object? parameter)
        {
            return SelectedWalk != null &&
                   SelectedWalk.Status == WalkStatus.Completed &&
                   SelectedWalk.Review == null;
        }

        private void ExecuteLeaveReview(object? parameter)
        {
            if (SelectedWalk == null) return;

            var review = new Review
            {
                OwnerId = _owner.Id,
                WalkerId = SelectedWalk.WalkerId,
                WalkId = SelectedWalk.Id,
                Rating = ReviewRating,
                Comment = ReviewComment,
                Date = DateTime.Now
            };

            _context.Reviews.Add(review);
            _context.SaveChanges();

            LoadWalkHistory();
            LoadWalkers();
            ReviewComment = string.Empty;
            ReviewRating = 5;
            StatusMessage = $"Review submitted! Rating: {review.Rating}/5";
        }

        private void ExportToJson()
        {
            var walks = _walkService.GetWalksByOwnerId(_owner.Id);
            _serializationService.ExportToJson(walks, "walks_export.json");
            StatusMessage = "Data exported to walks_export.json";
        }

        private void ExportToXml()
        {
            var walks = _walkService.GetWalksByOwnerId(_owner.Id);
            _serializationService.ExportToXml(walks, "walks_export.xml");
            StatusMessage = "Data exported to walks_export.xml";
        }

        private void GenerateReport()
        {
            var walks = _walkService.GetWalksByOwnerId(_owner.Id);
            string report = _reportService.GenerateWalkReport(walks, _owner);
            _reportService.SaveReportToFile(report, "walk_report.txt");
            StatusMessage = "Report generated: walk_report.txt";
        }

        private void OnLogout()
        {
            AuthService.GetInstance().Logout();
            LogoutRequested?.Invoke();
        }
    }
}
