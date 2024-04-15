﻿namespace PersonalPodcast.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime FirstLogin { get; set; }
        public DateTime LastLogin { get; set; }
        public string ConnectingIp { get; set; }
        public DateTime? Birthdate { get; set; }
    }
}
