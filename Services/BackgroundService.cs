namespace DuyProject.API.Services;

public class BackgroundService
{
    public BackgroundService()
    {

    }

    public void RunBackgroundJob()
    {
        //RecurringJob.AddOrUpdate(() => _pointService.ResetCycleLoyaltyPoint(), "0 16 * * *", TimeZoneInfo.Utc);
    }
}