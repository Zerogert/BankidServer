using Bankid.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Bankid.Data.Mappings {
	internal class CourseMap : IEntityTypeConfiguration<Course> {
		public void Configure(EntityTypeBuilder<Course> builder) {
			builder.ToTable("Courses");

			builder.HasKey(x => x.Id);
			builder.Property(x => x.Id).ValueGeneratedOnAdd();
			builder.Property(x => x.AuthorId).IsRequired(true);
			builder.Property(x => x.CreatedDate)
				.HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .HasDefaultValueSql("(DATETIME('now'))");

            builder.HasOne(p => p.Author).WithMany(b => b.Courses).HasForeignKey(x => x.AuthorId);
		}
	}
}
