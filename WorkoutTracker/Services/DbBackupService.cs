namespace WorkoutTracker.Services;

public class DbBackupService : IHostedService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<DbBackupService> _logger;
    private readonly string _dbFileName = "workout.db";

    public DbBackupService(IWebHostEnvironment env, ILogger<DbBackupService> logger)
    {
        _env = env;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var isAzure = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));
            var dataDir = isAzure ? @"D:\home\Data" : _env.ContentRootPath;
            var dbPath = Path.Combine(dataDir, _dbFileName);
            
            if (File.Exists(dbPath))
            {
                var backupDir = Path.Combine(dataDir, "Backups");
                if (!Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir);
                }

                var backupFileName = $"{Path.GetFileNameWithoutExtension(_dbFileName)}_{DateTime.Now:yyyyMMdd_HHmmss}{Path.GetExtension(_dbFileName)}";
                var backupPath = Path.Combine(backupDir, backupFileName);

                File.Copy(dbPath, backupPath, true);
                _logger.LogInformation($"Database backed up successfully to: {backupPath}");

                // Keep only the last 14 backups
                var oldBackups = Directory.GetFiles(backupDir, "*.db")
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.CreationTime)
                    .Skip(14)
                    .ToList();

                foreach (var oldBackup in oldBackups)
                {
                    oldBackup.Delete();
                    _logger.LogInformation($"Deleted old backup: {oldBackup.FullName}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while backing up the database.");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
