﻿using System.ComponentModel.DataAnnotations;

namespace TobyMeehan.Com.Data.Models;

public class UserDto
{
    public required string Id { get; set; }
    public required string DisplayName { get; set; }
    public required string Username { get; set; }
    public required string HashedPassword { get; set; }
    public required double Balance { get; set; }
    public string? Description { get; set; }
}