﻿using Microsoft.EntityFrameworkCore;

using PrsCSharpServer.Models;

namespace PrsCSharpServer.Data;

public class PrsDbContext : DbContext {

    public DbSet<User> Users { get; set; } = default!;
    public DbSet<Vendor> Vendors { get; set; } = default!;
    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<Request> Requests { get; set; } = default!;
    public DbSet<Requestline> Requestlines { get; set; } = default!;

    public PrsDbContext(DbContextOptions<PrsDbContext> options) : base(options) { }

    public PrsDbContext() { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if(!optionsBuilder.IsConfigured) {
            optionsBuilder.UseSqlServer("server=localhost\\sqlexpress;database=PrsDbC40;trusted_connection=true;trustServerCertificate=true;");
        }
    }
}
