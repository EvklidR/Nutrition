﻿namespace PostService.BusinessLogic.Exceptions
{
    public class Unauthorized : Exception
    {
        public Unauthorized(string message) : base(message) { }
    }
}
