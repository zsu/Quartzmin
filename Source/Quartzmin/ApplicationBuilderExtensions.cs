#if ( NETSTANDARD || NETCOREAPP || NET6 )

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Reflection;

namespace Quartzmin
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseQuartzmin( this IApplicationBuilder app, QuartzminOptions options, Action<Services> configure = null )
        {
            options = options ?? throw new ArgumentNullException( nameof( options ) );

            app.UseFileServer( options );

            var services = Services.Create( options );
            configure?.Invoke( services );

            app.Use( async ( context, next ) =>
             {
                 context.Items[typeof( Services )] = services;
                 await next.Invoke();
             } );

        }

        private static void UseFileServer( this IApplicationBuilder app, QuartzminOptions options )
        {
            IFileProvider fs;
            if ( string.IsNullOrEmpty( options.ContentRootDirectory ) )
                fs = new ManifestEmbeddedFileProvider( Assembly.GetExecutingAssembly(), "Content" );
            else
                fs = new PhysicalFileProvider( options.ContentRootDirectory );

            var fsOptions = new FileServerOptions()
            {
#if NETCOREAPP
                RequestPath = new PathString("/Quartzmin/Content" ),
#else
                RequestPath = new PathString("/Content"),
#endif
                EnableDefaultFiles = false,
                EnableDirectoryBrowsing = false,
                FileProvider = fs
            };

            app.UseFileServer( fsOptions );
        }

#if NETCOREAPP
        public static void AddQuartzmin( this IServiceCollection services )
        {
            services.AddControllers()
                .AddApplicationPart(Assembly.GetExecutingAssembly());
        }
#else
        public static void AddQuartzmin( this IServiceCollection services )
        {
            services.AddMvcCore()
                .AddApplicationPart( Assembly.GetExecutingAssembly() )
                .AddJsonFormatters();
        }
#endif

    }
}

#endif
