﻿using MediatR;

namespace UserService.Application.UseCases.Queries
{
    public interface IQueryHandler<in TQuery, TResult> 
        : IRequestHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
    }
}
