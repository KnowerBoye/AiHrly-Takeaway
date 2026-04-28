using AihrlyApi.Data;
using AihrlyApi.Entities;



public class NotificationService(ApiDbContext context , ILogger<NotificationService> logger)
{

    public async Task SendStageNotification(Guid applicationId, string type)
    {
      
        // await Task.Delay(20000);

        logger.LogInformation($"Notification: {applicationId}, type: {type}");

        var notification = new Notification
        {
            application_id = applicationId ,
            type = type
        };

       
        context.Notifications.Add(notification);

        await context.SaveChangesAsync();


        
    }
}