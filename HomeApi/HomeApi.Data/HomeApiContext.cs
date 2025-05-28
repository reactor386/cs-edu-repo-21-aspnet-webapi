﻿//-
using System;

using Microsoft.EntityFrameworkCore;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

using HomeApi.Data.Models;


namespace HomeApi.Data;

public class HomeApiContext : DbContext
{
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Device> Devices { get; set; }
    
    public HomeApiContext(DbContextOptions<HomeApiContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Room>().ToTable("Rooms");
        builder.Entity<Device>().ToTable("Devices");
    }
}
