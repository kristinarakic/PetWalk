using PetWalk.Data;
using PetWalk.Helpers;
using PetWalk.Models;
using PetWalk.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using System.Linq;

namespace PetWalk.ViewModels
{
    public class WalkerDashboardViewModel : BaseViewModel
    {
        private readonly Walker _walker;
        private readonly WalkService _walkService;
        private readonly SerializationService _serializationService;
        private readonly ReportService _reportService;

        private ObservableCollection<Walk> _pendingWalks = new();
        private ObservableCollection<Walk> _walkHistory = new();
        private Walk? _selectedPendingWalk;
        private Walk? _selectedHistoryWalk;
        private bool _isAvailable;
        private string _statusMessage = string.Empty;
        private ObservableCollection<AvailabilitySlot> _availabilitySlots = new();
        private AvailabilitySlot? _selectedSlot;
        private DateTime _slotDate = DateTime.Now.AddDays(1);
        private string _startTime = "08:00";
        private string _endTime = "16:00";

        public WalkerDashboardViewModel(Walker walker)
        {
            _walker = walker;
            _walkService = new WalkService();
            _serializationService = new SerializationService();
            _reportService = new ReportService();
            _isAvailable = walker.IsAvailable;

            AcceptWalkCommand = new RelayCommand(ExecuteAcceptWalk, _ => SelectedPendingWalk != null);
            DeclineWalkCommand = new RelayCommand(ExecuteDeclineWalk, _ => SelectedPendingWalk != null);
            StartWalkCommand = new RelayCommand(ExecuteStartWalk, CanStartWalk);
            CompleteWalkCommand = new RelayCommand(ExecuteCompleteWalk, CanCompleteWalk);
            ExportJsonCommand = new RelayCommand(_ => ExportToJson());
            ExportXmlCommand = new RelayCommand(_ => ExportToXml());
            GenerateReportCommand = new RelayCommand(_ => GenerateReport());
            LogoutCommand = new RelayCommand(_ => OnLogout());
            AddSlotCommand = new RelayCommand(ExecuteAddSlot, _ => !string.IsNullOrWhiteSpace(StartTime) && !string.IsNullOrWhiteSpace(EndTime));
            RemoveSlotCommand = new RelayCommand(ExecuteRemoveSlot, _ => SelectedSlot != null);

            LoadData();
        }

        public string WelcomeMessage => $"Welcome, {_walker.GetFullName()}!";
        public string RatingDisplay => $"Rating: {_walker.CalculateAverageRating():F1} / 5.0";

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

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand AcceptWalkCommand { get; }
        public ICommand DeclineWalkCommand { get; }
        public ICommand StartWalkCommand { get; }
        public ICommand CompleteWalkCommand { get; }
        public ICommand ExportJsonCommand { get; }
        public ICommand ExportXmlCommand { get; }
        public ICommand GenerateReportCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand AddSlotCommand { get; }
        public ICommand RemoveSlotCommand { get; }

        public event Action? LogoutRequested;

        private void LoadData()
        {
            LoadPendingWalks();
            LoadWalkHistory();
            LoadAvailabilitySlots();
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

        private void ExportToJson()
        {
            var walks = _walkService.GetWalksByWalkerId(_walker.Id);
            _serializationService.ExportToJson(walks, "walker_walks_export.json");
            StatusMessage = "Data exported to walker_walks_export.json";
        }

        private void ExportToXml()
        {
            var walks = _walkService.GetWalksByWalkerId(_walker.Id);
            _serializationService.ExportToXml(walks, "walker_walks_export.xml");
            StatusMessage = "Data exported to walker_walks_export.xml";
        }

        private void GenerateReport()
        {
            var walks = _walkService.GetWalksByWalkerId(_walker.Id);
            string report = _reportService.GenerateWalkReport(walks, _walker);
            _reportService.SaveReportToFile(report, "walker_report.txt");
            StatusMessage = "Report generated: walker_report.txt";
        }

        private void OnLogout()
        {
            AuthService.GetInstance().Logout();
            LogoutRequested?.Invoke();
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

        public string StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value);
        }

        public string EndTime
        {
            get => _endTime;
            set => SetProperty(ref _endTime, value);
        }

        private void ExecuteAddSlot(object? parameter)
        {
            if (!TimeSpan.TryParse(StartTime, out var start) ||
                !TimeSpan.TryParse(EndTime, out var end))
            {
                StatusMessage = "Invalid time format. Use HH:mm";
                return;
            }

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
    }
}
