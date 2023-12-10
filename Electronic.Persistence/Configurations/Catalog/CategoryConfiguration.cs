using Electronic.Domain.Models.Catalog;
using Electronic.Domain.Models.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Electronic.Persistence.Configurations.Catalog;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasMany(c => c.ChildCategories)
            .WithOne(c => c.ParentCategory)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.ThumbnailImage)
            .WithOne()
            .HasForeignKey<Category>(c => c.ThumbnailImageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasData(new List<Category>
        {
            new Category
            {
                CategoryId = 1,
                Name = "Điện tử, điện lạnh",
                Slug = "dien-tu-dien-lanh",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
            },
            new Category
            {
                CategoryId = 2,
                Name = "Tủ lạnh",
                Slug = "tu-lanh",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
                ParentCategoryId = 1
            },
            new Category
            {
                CategoryId = 3,
                Name = "Máy giặt",
                Slug = "may-giat",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
                ParentCategoryId = 1
            },
            new Category
            {
                CategoryId = 4,
                Name = "Máy sấy",
                Slug = "may-say",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
                ParentCategoryId = 1
            },
            new Category
            {
                CategoryId = 5,
                Name = "Máy nóng lạnh",
                Slug = "may-nong-lanh",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
                ParentCategoryId = 1
            },
            new Category
            {
                CategoryId = 6,
                Name = "Máy lọc nước",
                Slug = "may-loc-nuoc",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
                ParentCategoryId = 1
            },
            new Category
            {
                CategoryId = 7,
                Name = "Gia dụng nhà bếp",
                Slug = "gia-dung-nha-bep",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
            },

            new Category
            {
                CategoryId = 8,
                Name = "Bếp hồng ngoại",
                Slug = "bep-hong-ngoai",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
                ParentCategoryId = 7
            },
            new Category
            {
                CategoryId = 9,
                Name = "Bếp Gas",
                Slug = "bep-gas",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
                ParentCategoryId = 7
            },
            new Category
            {
                CategoryId = 10,
                Name = "Lò nướng",
                Slug = "lo-nuong",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
                ParentCategoryId = 7
            },
            new Category
            {
                CategoryId = 11,
                Name = "Lò vi sóng",
                Slug = "lo-vi-song",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
                ParentCategoryId = 7
            },
            new Category
            {
                CategoryId = 12,
                Name = "Nồi cơm điện",
                Slug = "noi-com-dien",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
                ParentCategoryId = 7
            },
            new Category
            {
                CategoryId = 13,
                Name = "Nồi chiên không dầu",
                Slug = "noi-chien-khong-dau",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
                ParentCategoryId = 7
            },
            new Category
            {
                CategoryId = 14,
                Name = "Bình Gas",
                Slug = "binh-gas",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
                ParentCategoryId = 7
            },
            new Category
            {
                CategoryId = 15,
                Name = "Chăm sóc nhà cửa",
                Slug = "cham-soc-nha-cua",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
            },
            new Category
            {
                CategoryId = 16,
                Name = "Quạt",
                Slug = "quat",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
                ParentCategoryId = 15
            },
            new Category
            {
                CategoryId = 17,
                Name = "Quạt Mini",
                Slug = "quat-mini",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
                ParentCategoryId = 15
            },
            new Category
            {
                CategoryId = 18,
                Name = "Điện tử",
                Slug = "dien-tu",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
            },
            new Category
            {
                CategoryId = 19,
                Name = "Điện thoại",
                Slug = "dien-thoai",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
                ParentCategoryId = 18
            },
            new Category
            {
                CategoryId = 20,
                Name = "Laptop",
                Slug = "latop",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
                ParentCategoryId = 18
            },
            new Category
            {
                CategoryId = 21,
                Name = "Màn hình máy tính",
                Slug = "man-hinh-may-tinh",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
                ParentCategoryId = 18
            },
            new Category
            {
                CategoryId = 22,
                Name = "Máy in",
                Slug = "may-in",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1,
                ParentCategoryId = 18
            },
            new Category
            {
                CategoryId = 23,
                Name = "Dụng cụ sửa chữa, vệ sinh",
                Slug = "dien-tu",
                Description = "TEST",
                IsPublished = true,
                IncludeInMenu = true,
                IsDeleted = false,
                DisplayOrder = 1
            },
        });
    }
}