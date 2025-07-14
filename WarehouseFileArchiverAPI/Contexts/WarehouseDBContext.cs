
using Microsoft.EntityFrameworkCore;
using WarehouseFileArchiverAPI.Models;

namespace WarehouseFileArchiverAPI.Contexts
{
    public class WarehouseDBContext : DbContext
    {
        public WarehouseDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<MediaType> MediaTypes { get; set; }
        public DbSet<FileVersion> FileVersions { get; set; }
        public DbSet<FileArchive> FileArchives { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<AccessLevel> AccessLevels { get; set; }
        public DbSet<RoleCategoryAccess> RoleCategoryAccesses { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasOne(u => u.User)
                                           .WithOne(ep => ep.Employee)
                                           .HasForeignKey<Employee>(ep => ep.Email)
                                           .HasConstraintName("FK_Employee_User")
                                           .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>().HasOne(u => u.Role)
                                        .WithMany(r => r.Users)
                                        .HasForeignKey(u => u.RoleId)
                                        .HasConstraintName("FK_User_Role")
                                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>().HasOne(r => r.AccessLevel)
                                        .WithMany(al => al.Roles)
                                        .HasForeignKey(r => r.AccessLevelId)
                                        .HasConstraintName("Fk_Role_Access")
                                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>().HasOne(r => r.AccessLevel)
                                        .WithMany(al => al.Categories)
                                        .HasForeignKey(r => r.AccessLevelId)
                                        .HasConstraintName("Fk_Category_Access")
                                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FileArchive>().HasOne(fa => fa.Category)
                                              .WithMany(c => c.FileArchives)
                                              .HasForeignKey(fa => fa.CategoryId)
                                              .HasConstraintName("FK_FileArchive_Category")
                                              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FileArchive>().HasOne(fa => fa.Employee)
                                              .WithMany(c => c.FileArchives)
                                              .HasForeignKey(fa => fa.UploadedById)
                                              .HasConstraintName("FK_FileArchive_Employee")
                                              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoleCategoryAccess>().HasOne(rc => rc.Category)
                                                .WithMany(c => c.RoleCategoryAccesses)
                                                .HasForeignKey(rc => rc.CategoryId)
                                                .HasConstraintName("FK_RoleCategory_Category")
                                                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FileVersion>().HasOne(fa => fa.FileArchive)
                                               .WithMany(fv => fv.FileVersions)
                                               .HasForeignKey(fv => fv.FileArchiveId)
                                               .HasConstraintName("FK_FileVersion_Archive")
                                               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FileVersion>().HasOne(f => f.Created)
                                              .WithMany(fv => fv.fileVersions)
                                              .HasForeignKey(f => f.CreatedBy)
                                              .HasConstraintName("FK_FileVersion_Employee")
                                              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FileVersion>().HasOne(f => f.ContentType)
                                              .WithMany(fv => fv.FileVersions)
                                              .HasForeignKey(f => f.ContentTypeId)
                                              .HasConstraintName("FK_FileVersion_MediaType")
                                              .OnDelete(DeleteBehavior.Restrict);

                            
            // Seed Data for Admin
            var adminAccessLevelId = new Guid("11111111-1111-1111-1111-111111111111");
            var adminRoleId = new Guid("22222222-2222-2222-2222-222222222222");
            var adminEmployeeId = new Guid("33333333-3333-3333-3333-333333333333");
            modelBuilder.Entity<AccessLevel>().HasData(new AccessLevel
            {
                Id = adminAccessLevelId,
                Access = "Admin",
                IsActive = true
            });



            modelBuilder.Entity<Role>().HasData(new Role
            {
                Id = adminRoleId,
                RoleName = "Admin",
                AccessLevelId = adminAccessLevelId
            });


            var adminUserId = "admin@gmail.com";
            var adminPasswordHash = "$2a$11$3kQN.ZcNLksh7Bp0surRlu.zvuZ3YDe7GPug4tTHfJTO817Al7ru2";
            modelBuilder.Entity<User>().HasData(new User
            {
                Username = adminUserId,
                PasswordHash = adminPasswordHash,
                RoleId = adminRoleId,
                IsDeleted = false
            });

            // Optional

            modelBuilder.Entity<Employee>().HasData(new Employee
            {
                Id = adminEmployeeId,
                FirstName = "Admin",
                LastName = "User",
                Email = adminUserId,
                IsActive = true
            });
        }
    }
}