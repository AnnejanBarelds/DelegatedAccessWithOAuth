﻿namespace DTO
{
    public class TodoResult
    {
        public string Token { get; set; }

        public IEnumerable<Todo> Todos { get; set;}
    }
}
