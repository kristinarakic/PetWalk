using PetWalk.Data;
using PetWalk.Helpers;
using PetWalk.Models;
using PetWalk.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace PetWalk.ViewModels
{
    public class WalkerDashboardViewModel : BaseDashboardViewModel
    {
        private readonly Walker _walker;

        private ObservableCollection<Walk> _pendingWalks = new();
        private ObservableCollection<Walk> _walkHistory = new();
        private Walk? _selectedPendingWalk;
        private Walk? _selectedHistoryWalk;
        private bool _isAvailable;

        private ObservableCollection<AvailabilitySlot> _availabilitySlots = new();
        private AvailabilitySlot? _selectedSlot;
        private DateTime _slotDate = DateTime.Now.AddDays(1);
        private int _startHour = 8;
        private int _startMinute = 0;
        private int _endHour = 16;
        private int _endMinute = 0;

        private decimal _profileHourlyRate;

        public WalkerDashboardViewModel(Walker walker)
        {
            _walker = walker;
            _isAvailable = walker.IsAvailable;

            AcceptWalkCommand = new RelayCommand(ExecuteAcceptWalk, _ => SelectedPendingWalk != null);
            DeclineWalkCommand = new RelayCommand(ExecuteDeclineWalk, _ => SelectedPendingWalk != null);
            StartWalkCommand = new RelayCommand(ExecuteStartWalk, CanStartWalk);
            CompleteWalkCommand = new RelayCommand(ExecuteCompleteWalk, CanCompleteWalk);
            AddSlotCommand = new RelayCommand(ExecuteAddSlot);
            RemoveSlotCommand = new RelayCommand(ExecuteRemoveSlot, _ => SelectedSlot != null);

            LoadData();
        }

        // PROPERTIES 

        public override string WelcomeMessage => $"Welcome, {_walker.GetFullName()}!";
        public string RatingDisplay => $"Rating: {_walker.CalculateAverageRating().ToString("F1", System.Globalization.CultureInfo.InvariantCulture)} / 5.0";

        public ObservableCollection<Walk> PendingWalks
        {
            get => _pendingWalks;
            set => SetProperty(ref _pendingWalks, value);
        }

        public ObservableCollection<Walk> WalkHistory
        {
            get => _walkHistory;
            set => SetProperty(ref _walkHistory, value);
        }

        public Walk? SelectedPendingWalk
        {
            get => _selectedPendingWalk;
            set => SetProperty(ref _selectedPendingWalk, value);
        }

        public Walk? SelectedHistoryWalk
        {
            get => _selectedHistoryWalk;
            set => SetProperty(ref _selectedHistoryWalk, value);
        }

        public bool IsAvailable
        {
            get => _isAvailable;
            set
            {
                SetProperty(ref _isAvailable, value);
                UpdateAvailability();
            }
        }

        public ObservableCollection<AvailabilitySlot> AvailabilitySlots
        {
            get => _availabilitySlots;
            set => SetProperty(ref _availabilitySlots, value);
        }

        public AvailabilitySlot? SelectedSlot
        {
            get => _selectedSlot;
            set => SetProperty(ref _selectedSlot, value);
        }

        public DateTime SlotDate
        {
            get => _slotDate;
            set => SetProperty(ref _slotDate, value);
        }

        public int StartHour
        {
            get => _startHour;
            set => SetProperty(ref _startHour, value);
        }

        public int StartMinute
        {
            get => _startMinute;
            set => SetProperty(ref _startMinute, value);
        }

        public int EndHour
        {
            get => _endHour;
            set => SetProperty(ref _endHour, value);
        }

        public int EndMinute
        {
            get => _endMinute;
            set => SetProperty(ref _endMinute, value);
        }

        public List<int> SlotHours { get; } = Enumerable.Range(0, 24).ToList();
        public List<int> SlotMinutes { get; } = new List<int> { 0, 15, 30, 45 };

        public decimal ProfileHourlyRate
        {
            get => _profileHourlyRate;
            set => SetProperty(ref _profileHourlyRate, value);
        }

        // COMMANDS

        public ICommand AcceptWalkCommand { get; }
        public ICommand DeclineWalkCommand { get; }
        public ICommand StartWalkCommand { get; }
        public ICommand CompleteWalkCommand { get; }
        public ICommand AddSlotCommand { get; }
        public ICommand RemoveSlotCommand { get; }

        // LOAD DATA

        private void LoadData()
        {
            LoadPendingWalks();
            LoadWalkHistory();
            LoadAvailabilitySlots();
            LoadProfile(_walker);
            ProfileHourlyRate = _walker.HourlyRate;
        }

        private void LoadPendingWalks()
        {
            var walks = _walkService.GetScheduledWalksByWalkerId(_walker.Id);
            PendingWalks = new ObservableCollection<Walk>(walks);
        }

        private void LoadWalkHistory()
        {
            var walks = _walkService.GetWalksByWalkerId(_walker.Id);
            WalkHistory = new ObservableCollection<Walk>(walks);
        }

        private void LoadAvailabilitySlots()
        {
            using var context = new PetWalkDbContext();
            var slots = context.AvailabilitySlots
                .Where(a => a.WalkerId == _walker.Id)
                .ToList()
                .Where(a => a.Date >= DateTime.Now.Date)
                .OrderBy(a => a.Date)
                .ThenBy(a => a.StartTime)
                .ToList();
            AvailabilitySlots = new ObservableCollection<AvailabilitySlot>(slots);
        }

        // ACTIONS

        private void ExecuteAcceptWalk(object? parameter)
        {
            if (SelectedPendingWalk == null) return;
            _walkService.AcceptWalk(SelectedPendingWalk.Id);
            LoadData();
            StatusMessage = "Walk accepted!";
        }

        private void ExecuteDeclineWalk(object? parameter)
        {
            if (SelectedPendingWalk == null) return;
            _walkService.DeclineWalk(SelectedPendingWalk.Id);
            LoadData();
            StatusMessage = "Walk declined.";
        }

        private bool CanStartWalk(object? parameter)
        {
            return SelectedHistoryWalk != null &&
                   SelectedHistoryWalk.Status == WalkStatus.Accepted;
        }

        private void ExecuteStartWalk(object? parameter)
        {
            if (SelectedHistoryWalk == null) return;
            _walkService.StartWalk(SelectedHistoryWalk.Id);
            LoadData();
            StatusMessage = "Walk started!";
        }

        private bool CanCompleteWalk(object? parameter)
        {
            return SelectedHistoryWalk != null &&
                   SelectedHistoryWalk.Status == WalkStatus.InProgress;
        }

        private void ExecuteCompleteWalk(object? parameter)
        {
            if (SelectedHistoryWalk == null) return;
            _walkService.CompleteWalk(SelectedHistoryWalk.Id);
            LoadData();
            StatusMessage = "Walk completed!";
        }

        private void ExecuteAddSlot(object? parameter)
        {
            var start = new TimeSpan(StartHour, StartMinute, 0);
            var end = new TimeSpan(EndHour, EndMinute, 0);

            if (start >= end)
            {
                StatusMessage = "Start time must be before end time.";
                return;
            }

            if (SlotDate.Date < DateTime.Now.Date)
            {
                StatusMessage = "Cannot add slots in the past.";
                return;
            }

            var slot = new AvailabilitySlot
            {
                WalkerId = _walker.Id,
                Date = SlotDate.Date,
                StartTime = start,
                EndTime = end
            };

            using var context = new PetWalkDbContext();
            context.AvailabilitySlots.Add(slot);
            context.SaveChanges();

            LoadAvailabilitySlots();
            StatusMessage = $"Slot added: {slot.Display}";
        }

        private void ExecuteRemoveSlot(object? parameter)
        {
            if (SelectedSlot == null) return;

            using var context = new PetWalkDbContext();
            var slot = context.AvailabilitySlots.Find(SelectedSlot.Id);
            if (slot != null)
            {
                context.AvailabilitySlots.Remove(slot);
                context.SaveChanges();
            }

            LoadAvailabilitySlots();
            StatusMessage = "Slot removed.";
        }

        private void UpdateAvailability()
        {
            using var context = new PetWalkDbContext();
            var walker = context.Walkers.Find(_walker.Id);
            if (walker != null)
            {
                walker.IsAvailable = IsAvailable;
                context.SaveChanges();
            }
        }

        // OVERRIDES 

        protected override void ExecuteSaveProfile(object? parameter)
        {
            using var context = new PetWalkDbContext();
            var user = context.Walkers.Find(_walker.Id);
            if (user != null)
            {
                user.FirstName = ProfileFirstName;
                user.LastName = ProfileLastName;
                user.Phone = ProfilePhone;
                user.Location = ProfileLocation;
                user.HourlyRate = ProfileHourlyRate;
                context.SaveChanges();

                _walker.FirstName = ProfileFirstName;
                _walker.LastName = ProfileLastName;
                _walker.Phone = ProfilePhone;
                _walker.Location = ProfileLocation;
                _walker.HourlyRate = ProfileHourlyRate;

                OnPropertyChanged(nameof(WelcomeMessage));
                OnPropertyChanged(nameof(RatingDisplay));
                StatusMessage = "Profile updated!";
            }
        }

        protected override void ExportToJson()
        {
            var walks = _walkService.GetWalksByWalkerId(_walker.Id);
            _serializationService.ExportToJson(walks, "walker_walks_export.json");
            StatusMessage = "Data exported to walker_walks_export.json";
        }

        protected override void ExportToXml()
        {
            var walks = _walkService.GetWalksByWalkerId(_walker.Id);
            _serializationService.ExportToXml(walks, "walker_walks_export.xml");
            StatusMessage = "Data exported to walker_walks_export.xml";
        }

        protected override void GenerateReport()
        {
            var walks = _walkService.GetWalksByWalkerId(_walker.Id);
            string report = _reportService.GenerateWalkReport(walks, _walker);
            _reportService.SaveReportToFile(report, "walker_report.txt");
            StatusMessage = "Report generated: walker_report.txt";
        }
    }
}