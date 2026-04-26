using Microsoft.EntityFrameworkCore;
using AihrlyApi.Entities;


namespace AihrlyApi.Data
{


public class ApiDbContext(DbContextOptions<ApiDbContext> options) : DbContext(options)
    {

        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Job> Jobs { get; set; }

        //application related entities         
        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationNote> ApplicationNotes { get; set; }
        public DbSet<ApplicationStageHistory> ApplicationStageHistories { get; set; }
        


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);



            modelBuilder.HasPostgresEnum<ApplicationStages>("application_stages");
            modelBuilder.HasPostgresEnum<Status>("job_status");
            modelBuilder.HasPostgresEnum<TeamMemberRole>("team_member_role");



            modelBuilder.Entity<Job>()
            .HasIndex(j => j.status);
           
        


            // configurations for all aplications entities 

            modelBuilder.Entity<Application>(entity =>
            {
                entity.HasOne(a => a.job)
                    .WithMany( j => j.applications)
                    .HasForeignKey(a => a.jobId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(a => new {a.email , a.jobId}).IsUnique();

                entity.HasIndex(a => a.current_stage);
                entity.HasIndex(a => a.jobId);


            });


        
            modelBuilder.Entity<ApplicationNote>(entity =>
            {
                
                entity.HasOne( an => an.teamMember) 
                    .WithMany() 
                    .HasForeignKey(an => an.created_by)
                    .OnDelete(DeleteBehavior.NoAction); 

                entity.HasOne<Application>()
                .WithMany(a => a.notes)
                .HasForeignKey(an => an.applicationId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(an => an.applicationId);
                entity.HasIndex(an => an.created_by);
            });


            modelBuilder.Entity<ApplicationStageHistory>(entity =>
            {

                entity.HasOne<Application>()
                .WithMany(a => a.stage_history)
                .HasForeignKey(ash => ash.applicationId)
                .OnDelete(DeleteBehavior.Cascade);


                entity.HasOne<TeamMember>()
                .WithMany()
                .HasForeignKey(ash => ash.changed_by)
                .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(ash => ash.applicationId);
                entity.HasIndex(ash => ash.changed_by);
                
            });

            
        }
    }
}