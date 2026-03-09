using FuelApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FuelApp.Data
{
    /// <summary>
    /// Entity Framework Core database context for FuelApp.
    /// </summary>
    /// <remarks>
    /// Exposes DbSet properties for all aggregate entities and centralizes
    /// model configuration (keys, constraints, and relationship behaviors)
    /// through the Fluent API in <see cref="OnModelCreating(ModelBuilder)"/>.
    /// </remarks>
    public class FuelAppContext : DbContext
    {
        public FuelAppContext(DbContextOptions<FuelAppContext> options)
            : base(options)
        {
        }

        public DbSet<Association> Associations => Set<Association>();
        public DbSet<Team> Teams => Set<Team>();
        public DbSet<Player> Players => Set<Player>();
        public DbSet<StaffMember> StaffMembers => Set<StaffMember>();
        public DbSet<Evaluation> Evaluations => Set<Evaluation>();

        /// <summary>
        /// Configures entity schema details and relationships for the domain model.
        /// </summary>
        /// <param name="modelBuilder">Builder used to define EF Core model mappings.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Association>(entity =>
            {
                entity.HasKey(a => a.AssociationID);

                entity.Property(a => a.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasMany(a => a.Teams)
                    .WithOne(t => t.Association)
                    .HasForeignKey(t => t.AssociationID)
                    // Keep teams when an association is removed; clear the FK instead.
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Index for Team.AssociationID foreign key lookups
            modelBuilder.Entity<Team>()
                .HasIndex(t => t.AssociationID)
                .HasDatabaseName("IX_Team_AssociationID");

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(t => t.TeamID);

                entity.Property(t => t.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasMany(t => t.Players)
                    .WithOne(p => p.Team)
                    .HasForeignKey(p => p.TeamID)
                    // Keep players when a team is removed; they can exist unassigned.
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Index for Player.TeamID foreign key lookups
            modelBuilder.Entity<Player>()
                .HasIndex(p => p.TeamID)
                .HasDatabaseName("IX_Player_TeamID");

            // Index for Player.AssociationID foreign key lookups
            modelBuilder.Entity<Player>()
                .HasIndex(p => p.AssociationID)
                .HasDatabaseName("IX_Player_AssociationID");

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.PlayerID);

                entity.Property(p => p.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(p => p.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(p => p.BirthDate)
                    .IsRequired();

                entity.Property(p => p.Position)
                    // Store enum values as short readable strings (e.g., "RW").
                    .HasConversion<string>()
                    .HasMaxLength(2);

                entity.HasOne(p => p.Association)
                    .WithMany()
                    .HasForeignKey(p => p.AssociationID)
                    // Preserve player records if association is removed.
                    .OnDelete(DeleteBehavior.SetNull);
                
            });

            modelBuilder.Entity<StaffMember>(entity =>
            {
                entity.HasKey(s => s.StaffMemberID);

                entity.Property(s => s.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(s => s.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(s => s.Role)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(s => s.Email)
                    .HasMaxLength(100);

                entity.HasOne(s => s.Team)
                    .WithMany()
                    .HasForeignKey(s => s.TeamID)
                    // Preserve staff records if team is removed.
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Index for StaffMember.TeamID foreign key lookups
            modelBuilder.Entity<StaffMember>()
                .HasIndex(s => s.TeamID)
                .HasDatabaseName("IX_StaffMember_TeamID");

            modelBuilder.Entity<Evaluation>(entity =>
            {
                entity.HasKey(e => e.EvaluationId);

                entity.Property(e => e.Summary)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.Date)
                    .IsRequired();

                entity.HasOne(e => e.Player)
                    .WithMany(p => p.Evaluations)
                    .HasForeignKey(e => e.PlayerId)
                    // Evaluations are dependent on players and are removed with them.
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.StaffMember)
                    .WithMany(s => s.Evaluations)
                    .HasForeignKey(e => e.StaffMemberId)
                    // Retain evaluations if staff member is removed; clear evaluator link.
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Index for Evaluation.PlayerId foreign key lookups
            modelBuilder.Entity<Evaluation>()
                .HasIndex(e => e.PlayerId)
                .HasDatabaseName("IX_Evaluation_PlayerId");

            // Index for Evaluation.StaffMemberId foreign key lookups
            modelBuilder.Entity<Evaluation>()
                .HasIndex(e => e.StaffMemberId)
                .HasDatabaseName("IX_Evaluation_StaffMemberId");

            // Composite index for Evaluation sorting by Date DESC, EvaluationId DESC
            // This matches the default query ORDER BY clause for optimal performance
            modelBuilder.Entity<Evaluation>()
                .HasIndex(e => new { e.Date, e.EvaluationId })
                .IsDescending(true, true)
                .HasDatabaseName("IX_Evaluation_Date_EvaluationId");
        }
    }
}
