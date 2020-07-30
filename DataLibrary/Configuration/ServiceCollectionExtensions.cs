using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TobyMeehan.Com.Data.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static DataAccessLibraryBuilder AddDataAccessLibrary(this IServiceCollection services)
        {
            return new DataAccessLibraryBuilder(services);
        }
    }
}
