﻿namespace NemetschekEventManagerBackend.Models.JSON
{
    public class Field
    {
        public int Id { get; set; }
        public string? Type { get; set; }
        public string? Name { get; set; }
        public bool Required { get; set; } = false;
        public IList<string>? Options { get; set; }
    }
}
