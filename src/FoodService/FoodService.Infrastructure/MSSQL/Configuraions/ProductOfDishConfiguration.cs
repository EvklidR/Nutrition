﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FoodService.Domain.Entities;

namespace FoodService.Infrastructure.MSSQL.Configurations
{
    public class ProductOfDishConfiguration : IEntityTypeConfiguration<ProductOfDish>
    {
        public void Configure(EntityTypeBuilder<ProductOfDish> builder)
        {
            builder.HasKey(iod => new 
            {
                iod.DishId,
                iod.ProductId 
            });

            builder.HasOne<Dish>()
                .WithMany(d => d.Ingredients)
                .HasForeignKey(iod => iod.DishId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(iod => iod.Product)
                .WithMany()
                .HasForeignKey(iod => iod.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
