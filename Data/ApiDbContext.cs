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



            modelBuilder.Entity<Job>(entity =>
            {
                entity.HasIndex(j => j.status);

                entity.Property(j => j.status)
                .HasConversion<string>();
            });
            




            modelBuilder.Entity<TeamMember>()
            .Property(tm => tm.role)
            .HasConversion<string>();


            // configurations for all aplications entities 

            modelBuilder.Entity<Application>(entity =>
            {
                entity.HasOne(a => a.job)
                    .WithMany( j => j.applications)
                    .HasForeignKey(a => a.jobId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(a => new {a.email , a.jobId}).IsUnique();

                entity.Property(a => a.current_stage)
                .HasConversion<string>();

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

                entity.Property(an => an.type)
                .HasConversion<string>();

                entity.HasIndex(an => an.applicationId);
                entity.HasIndex(an => an.created_by);
            });


            modelBuilder.Entity<ApplicationStageHistory>(entity =>
            {

                entity.HasOne<Application>()
                .WithMany(a => a.stage_history)
                .HasForeignKey(ash => ash.applicationId)
                .OnDelete(DeleteBehavior.Cascade);


                entity.HasOne(stage => stage.teamMember)
                .WithMany()
                .HasForeignKey(ash => ash.changed_by)
                .OnDelete(DeleteBehavior.NoAction);

                entity.Property(ash => ash.from_stage)
                .HasConversion<string>();

                entity.Property(ash => ash.to_stage)
                .HasConversion<string>();

                entity.HasIndex(ash => ash.applicationId);
                entity.HasIndex(ash => ash.changed_by);
                
            });


            modelBuilder.Entity<ApplicationScore>(entity =>
            {

                entity.HasOne(asc => asc.teamMember)
                .WithMany()
                .HasForeignKey(asc => asc.updatedBy)
                .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne<Application>()
                .WithMany(a => a.scores)
                .HasForeignKey(asc => asc.applicationId )
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.applicationId, e.dimension }).IsUnique();
                
                entity.Property(e => e.dimension).
                HasConversion<string>();
            });


            // seed a init team member

            modelBuilder.Entity<TeamMember>().HasData(new TeamMember
            {
                id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                name = "Noah Boye",
                email = "noahboye@mail.com" , 
                role = TeamMemberRole.hiring_manager
            });
            modelBuilder.Entity<TeamMember>().HasData(new TeamMember
            {
                id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                name = "Jane Doe",
                email = "janedoe@mail.com" , 
                role = TeamMemberRole.recruiter
            });
            modelBuilder.Entity<TeamMember>().HasData(new TeamMember
            {
                id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                name = "John Boye",
                email = "johnboye@mail.com" , 
                role = TeamMemberRole.hiring_manager
            });

            
        }
    }
}